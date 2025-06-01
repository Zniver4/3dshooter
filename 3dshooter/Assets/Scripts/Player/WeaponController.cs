using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Lugar del cual saldra el RayCast del disparo")]
    [SerializeField] private Transform playerCamera;

    [Header("RayCast")]
    [Tooltip("Rango Maximo del Disparo")]
    [SerializeField] private float range = 15f;

    [Header("Damage")]
    [Tooltip("Daño del Disparo")]
    [SerializeField] private int damage = 25;

    [Header("LayerMask")]
    [Tooltip("Objetos a los que le Puedes Hacer Daño")]
    [SerializeField] private LayerMask layerMask;

    private void Update()
    {
        Shoot();
    }

    void Shoot()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward;

            RaycastHit hitInfo;

            if(Physics.Raycast(rayOrigin, rayDirection, out hitInfo, range, layerMask))
            {
                Debug.DrawRay(rayOrigin, rayDirection * hitInfo.distance, Color.green, 1f);

                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();

                if(damageable != null)
                {
                    damageable.ApplyDamage(damage);
                }

                else
                {
                    Debug.DrawLine(rayOrigin, rayDirection * range, Color.red, 1f);
                }
            }
        }
    }
}
