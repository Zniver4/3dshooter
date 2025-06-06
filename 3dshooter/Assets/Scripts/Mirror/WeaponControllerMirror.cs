using System;
using UnityEngine;
using Mirror;

public class WeaponControllerMirror : NetworkBehaviour
{
    public static Action<int, int> OnShoot;

    [Header("References")]
    [SerializeField] private Transform playerCamera;

    [Header("RayCast")]
    [SerializeField] private float range = 15f;

    [Header("Damage")]
    [SerializeField] private int minDamage = 18;
    [SerializeField] private int maxDamage = 25;

    [Header("LayerMask")]
    [SerializeField] private LayerMask layerMask;

    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 5f;

    [Header("Ammo")]
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

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + (1f / fireRate);
            currentAmmo--;
            OnShoot?.Invoke(currentAmmo, magazineSize);

            CmdShoot(playerCamera.position, playerCamera.forward);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentAmmo = magazineSize;
            OnShoot?.Invoke(currentAmmo, magazineSize);
        }
    }

    [Command]
    private void CmdShoot(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, range, layerMask))
        {
            Debug.DrawRay(origin, direction * hitInfo.distance, Color.green, 1f);

            IDamageable damageable = hitInfo.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int damage = UnityEngine.Random.Range(minDamage, maxDamage);
                damageable.ApplyDamage(damage);
            }
        }
        else
        {
            Debug.DrawRay(origin, direction * range, Color.red, 1f);
        }
    }
}
