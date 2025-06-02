using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI bulletText;

    private void OnEnable()
    {
        WeaponController.OnShoot += BulletHUD;
    }

    private void OnDisable()
    {
        WeaponController.OnShoot -= BulletHUD;
    }

    void BulletHUD(int currentBullets, int magBullets)
    {
        bulletText.text = (currentBullets + "/" + magBullets);
    }
}
