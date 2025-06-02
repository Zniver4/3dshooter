using Mirror.Examples.MultipleAdditiveScenes;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI bulletText;
    public TextMeshProUGUI scoreText;

    int playerScore = 0;
    int enemyScore = 0;

    private void OnEnable()
    {
        WeaponController.OnShoot += BulletHUD;
        HealthManager.OnEnemyKilled += EnemyKiiled;
        HealthManager.OnPlayerKilled += PlayerKilled;
    }

    private void OnDisable()
    {
        WeaponController.OnShoot -= BulletHUD;
        HealthManager.OnEnemyKilled -= EnemyKiiled;
        HealthManager.OnPlayerKilled -= PlayerKilled;
    }

    void BulletHUD(int currentBullets, int magBullets)
    {
        bulletText.text = (currentBullets + "/" + magBullets);
    }

    void EnemyKiiled()
    {
        playerScore++;

        Score();
    }

    void PlayerKilled()
    {
        enemyScore++;

        Score();
    }
    
    void Score()
    {
        scoreText.text = (playerScore + " / " + enemyScore);
    }
}
