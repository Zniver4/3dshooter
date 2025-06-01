using UnityEngine;

public class HealthManager : MonoBehaviour, IDamageable
{
    [Header("Life Setting")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float shield = 50f;

    private void Update()
    {
        if(health <= 0)
        {
            gameObject.SetActive(false);
            Debug.Log(gameObject.name + ": Muerto");
        }
    }

    public void ApplyDamage(float damageTaken)
    {
        if (shield > 0)
        {
            shield -= damageTaken;
            Debug.Log(shield);
        }
        else
        {
            health -= damageTaken;
            Debug.Log(health);
        }
    }
}
