using System;
using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    public static Action OnEnemyKilled;
    public static Action OnPlayerKilled;

    [Header("Life Setting")]
    [SerializeField] private int health = 100;
    [SerializeField] private int shield = 50;

    [Header("Type")]
    [SerializeField] bool player = false;
    [SerializeField] bool enemy = false;

    private void Update()
    {
        if(health <= 0)
        {
            gameObject.SetActive(false);

            if (enemy)
            {
                OnEnemyKilled?.Invoke();
            }
            else
            {
                OnPlayerKilled?.Invoke();
            }
        }
    }

    public void ApplyDamage(int damageTaken)
    {
        if (shield > 0)
        {
            shield -= damageTaken;
        }
        else
        {
            health -= damageTaken;
        }
    }
}
