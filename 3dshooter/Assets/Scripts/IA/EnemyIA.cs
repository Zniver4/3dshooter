using Mirror.Examples.Common;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
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
    [SerializeField] Vector3 walkPoint;
    [SerializeField] float walkPointRange;
    private bool walkpointSet; 

    [Header("States")]
    [SerializeField] private float sightRange, attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrolling();
        if (playerInSightRange && !playerInAttackRange) Chasing();
        if (playerInSightRange && playerInAttackRange) Attacking();
    }

    void Patrolling()
    {
        if (!walkpointSet) SerchWalkPoint();

        if (walkpointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude <1) walkpointSet = false;
    }

    void SerchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) walkpointSet = true;
    }

    void Chasing()
    {
        agent.SetDestination(player.position);
    }

    void Attacking()
    {
        agent.SetDestination(transform.position);

        float rotationSpeed = 5f;
        Vector3 direction = player.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        if (!alreadyAttacked/* && Time.time >= nextFireTime*/)
        {
            nextFireTime = Time.time + (1f / fireRate);

            Vector3 rayOrigin = shootPoint.position;
            Vector3 rayDirection = shootPoint.forward;

            RaycastHit hitInfo;

            if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, range, damageable))
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
                Debug.DrawLine(rayOrigin, rayDirection * range, Color.red, 1f);
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
