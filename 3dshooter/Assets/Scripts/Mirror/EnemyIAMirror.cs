using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System.Linq;

public class EnemyIAMirror : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform shootPoint;

    [Header("Layer Mask")]
    [SerializeField] private LayerMask whatIsGround, WhatIsPlayer;
    [SerializeField] private LayerMask damageable;

    [Header("RayCast")]
    [SerializeField] private float range = 50f;

    [Header("Fire Rate")]
    [Tooltip("Balas por segundo")]
    [SerializeField] private float fireRate = 5f;
    private float nextFireTime = 0f;
    private bool alreadyAttacked;
    public float timeBetweenAttacks = 0.5f;

    [Header("Damage")]
    [Tooltip("Daño del Disparo")]
    [SerializeField] private int minDamage = 18;
    [SerializeField] private int maxDamage = 25;

    [Header("Patrol")]
    [SerializeField] private float walkPointRange = 10f;
    [SerializeField] private bool walkpointSet;
    private Vector3 walkPoint;
    
    [Header("States")]
    [SerializeField] private float sightRange = 15f;
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    private Transform nearestPlayer;

    private void Update()
    {
        if (!isServer || !gameObject.activeInHierarchy) return;

        FindClosestPlayer();

        if (!playerInSightRange && !playerInAttackRange)
            Patrolling();
        else if (playerInSightRange && !playerInAttackRange)
            Chasing();
        else if (playerInSightRange && playerInAttackRange)
            Attacking();
    }

    public void ActivateAndPatrol(Vector3 position)
    {
        this.gameObject.SetActive(true);
        transform.position = position;
        walkpointSet = false;
    }

    void FindClosestPlayer()
    {
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null) continue;

            Transform target = conn.identity.transform;
            if (target == transform) continue;

            float distance = Vector3.Distance(transform.position, target.position);

            if (distance < closestDistance && distance <= sightRange)
            {
                closestDistance = distance;
                closest = target;
            }
        }

        nearestPlayer = closest;
        playerInSightRange = (nearestPlayer != null);
        playerInAttackRange = (nearestPlayer != null && Vector3.Distance(transform.position, nearestPlayer.position) <= attackRange);
    }

    void Patrolling()
    {
        if (!walkpointSet) SearchWalkPoint();

        if (walkpointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 3f)
            walkpointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkpointSet = true;
    }

    void Chasing()
    {
        if (nearestPlayer != null)
            agent.SetDestination(nearestPlayer.position);
    }

    void Attacking()
    {
        agent.SetDestination(transform.position);

        if (nearestPlayer != null)
        {
            float rotationSpeed = 5f;
            Vector3 direction = nearestPlayer.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (!alreadyAttacked)
        {
            Vector3 rayOrigin = shootPoint.position;
            Vector3 rayDirection = shootPoint.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hitInfo, range, damageable))
            {
                Debug.DrawRay(rayOrigin, rayDirection * hitInfo.distance, Color.green, 1f);

                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int damage = Random.Range(minDamage, maxDamage);
                    damageable.ApplyDamage(damage);
                }
            }
            else
            {
                Debug.DrawLine(rayOrigin, rayOrigin + rayDirection * range, Color.red, 1f);
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
