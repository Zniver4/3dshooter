using UnityEngine;

public class WinConditionManager: MonoBehaviour
{
    public Canvas playerWinCanvas;
    public Canvas enemyWinCanvas;

    private void OnEnable()
    {
        HUDManager.OnPlayerWin += PlayerWin;
        HUDManager.OnEnemyWin += EnemyWin;
    }

    private void OnDisable()
    {
        HUDManager.OnPlayerWin -= PlayerWin;
        HUDManager.OnEnemyWin -= EnemyWin;
    }

    private void Start()
    {
        Time.timeScale = 1.0f;

        playerWinCanvas.enabled = false;
        enemyWinCanvas.enabled = false;
    }

    void PlayerWin()
    {
        playerWinCanvas.enabled = true;
        Time.timeScale = 0;
    }

    void EnemyWin()
    {
        enemyWinCanvas.enabled = true;
        Time.timeScale = 0;
    }
}
