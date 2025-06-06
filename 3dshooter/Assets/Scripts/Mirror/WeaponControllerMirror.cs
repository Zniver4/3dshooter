using System;
using UnityEngine;
using Mirror;

public class WeaponControllerMirror : NetworkBehaviour
{
    public static Action<int, int> OnShoot;

    [Header("References")]
    [Tooltip("Lugar del cual saldra el RayCast del disparo")]
    [SerializeField] private Transform playerCamera;

    [Header("RayCast")]
    [Tooltip("Rango Maximo del Disparo")]
    [SerializeField] private float range = 15f;

    [Header("Damage")]
    [Tooltip("Daño del Disparo")]
    [SerializeField] private int minDamage = 18;
    [SerializeField] private int maxDamage = 25;

    [Header("LayerMask")]
    [Tooltip("Objetos a los que le Puedes Hacer Daño")]
    [SerializeField] private LayerMask layerMask;

    [Header("Fire Rate")]
    [Tooltip("Balas por segundo")]
    [SerializeField] private float fireRate = 5f;

    [Header("Ammo")]
    [Tooltip("Balas Maximas por cargador")]
    [SerializeField] private int magazineSize = 30;
    private int currentAmmo;

    private float nextFireTime = 0f;

    private void Start()
    {
        currentAmmo = magazineSize;

        OnShoot?.Invoke(currentAmmo, magazineSize);
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        Shoot();
        Reload();
    }


    void Shoot()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + (1f / fireRate);

            currentAmmo--;
            OnShoot?.Invoke(currentAmmo, magazineSize);

            Vector3 rayOrigin = playerCamera.position;
            Vector3 rayDirection = playerCamera.forward;

            RaycastHit hitInfo;

            if (Physics.Raycast(rayOrigin, rayDirection, out hitInfo, range, layerMask))
            {
                Debug.DrawRay(rayOrigin, rayDirection * hitInfo.distance, Color.green, 1f);

                IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();

                if (damageable != null)
                {
                    int damage = UnityEngine.Random.Range(minDamage, maxDamage);
                    damageable.ApplyDamage(damage);
                }
            }

            else
            {
                Debug.DrawLine(rayOrigin, rayDirection * range, Color.red, 1f);
            }
        }
    }

    void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentAmmo = magazineSize;

            OnShoot?.Invoke(currentAmmo, magazineSize);
        }
    }
}
