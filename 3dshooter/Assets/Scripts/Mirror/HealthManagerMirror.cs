using UnityEngine;
using Mirror;

public class HealthManagerMirror : NetworkBehaviour, IDamageable
{
    [Header("Life Settings")]
    [SyncVar] public int health = 100;
    [SyncVar] public int shield = 50;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    [ServerCallback]
    private void Update()
    {
        if (health <= 0)
        {
            RpcHandleDeath();
            this.enabled = false; // desactivar lógica de salud en el servidor si quieres
        }
    }

    [Server]
    public void ApplyDamage(int damageTaken)
    {
        if (shield > 0)
        {
            shield -= damageTaken;
            if (shield < 0)
            {
                health += shield; // shield es negativo, así que resta al health
                shield = 0;
            }
        }
        else
        {
            health -= damageTaken;
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
}
