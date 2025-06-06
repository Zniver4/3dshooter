using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class EnemySpawnerMirror : NetworkBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints;
    public int maxEnemies = 5;
    public float spawnDelay = 1.5f;
    public int Enemies = 0;

    [Header("Enemy Pool")]
    public List<GameObject> enemyPool;

    private float timer;

    private void OnEnable()
    {
        HealthManagerMirror.OnEnemyDeath += EnemyDead;
    }

    private void OnDisable()
    {
        HealthManagerMirror.OnEnemyDeath += EnemyDead;
    }

    void EnemyDead()
    {
        Enemies--;
    }

    private void Update()
    {
        if (!isServer) return;

        int activeEnemies = CountActiveEnemies();

        if (activeEnemies < maxEnemies)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                SpawnEnemyFromPool();
                timer = 0f;
            }
        }
    }

    int CountActiveEnemies()
    {
        int count = 0;
        foreach (var enemy in enemyPool)
        {
            if (enemy != null && enemy.activeInHierarchy)
                count++;
            Enemies = count;
        }

        return Enemies;
    }

    void SpawnEnemyFromPool()
    {
        GameObject enemyToActivate = enemyPool.Find(e => e != null && !e.activeInHierarchy);
        if (enemyToActivate == null) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Mueve al enemigo y lo activa
        enemyToActivate.GetComponent<EnemyIAMirror>().ActivateAndPatrol(spawnPoint.position);

        // Solo si nunca fue spawneado antes (1ra vez)
        if (!NetworkServer.spawned.ContainsValue(enemyToActivate.GetComponent<NetworkIdentity>()))
        {
            NetworkServer.Spawn(enemyToActivate);
        }
    }
}
