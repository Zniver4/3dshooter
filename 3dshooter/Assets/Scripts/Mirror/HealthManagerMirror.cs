using UnityEngine;
using Mirror;
using System;
using Unity.VisualScripting;

public class HealthManagerMirror : NetworkBehaviour, IDamageable
{
    public static Action OnEnemyDeath;

    [Header("Life Settings")]
    [SyncVar] public int health = 100;
    [SyncVar] public int shield = 50;

    public int actualHealth;
    public int actualShield;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [ServerCallback]
    private void Update()
    {
        if (actualHealth <= 0)
        {
            RpcHandleDeath();
            OnEnemyDeath?.Invoke();
        }
    }

    private void OnEnable()
    {
        OnEnemyDeath += Reset;
    }

    private void OnDisable()
    {
        OnEnemyDeath -= Reset;
    }

    [Server]
    public void ApplyDamage(int damageTaken)
    {
        if (actualShield > 0)
        {
            actualShield -= damageTaken;
        }
        else
        {
            actualHealth -= damageTaken;
        }
    }

    [ClientRpc]
    void RpcHandleDeath()
    {
        gameObject.SetActive(false);
    }

    [Server]
    public void Respawn(Vector3 newPosition, int newHealth = 100, int newShield = 50)
    {
        health = newHealth;
        shield = newShield;

        RpcRespawn(newPosition);
        this.enabled = true;
    }

    [ClientRpc]
    void RpcRespawn(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    void Reset()
    {
        actualHealth = health;
        actualShield = shield;
    }
}
