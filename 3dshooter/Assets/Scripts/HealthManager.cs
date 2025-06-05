using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    [Header("Life Setting")]
    [SerializeField] private int health = 100;
    [SerializeField] private int shield = 50;

    private void Update()
    {
        if(health <= 0)
        {
            gameObject.SetActive(false);
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
