using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    EnemyReset[] enemies;

    public static GameManager instance;

    public Text coinsText;
    private int coins = 0;

    [SerializeField] int winCoins = 24;
    public bool finished = false;

    private void Awake()
    {
        instance = this;
        enemies = FindObjectsOfType<EnemyReset>();

    }

    private void Start()
    {
        UpdateUI();
    }

    public void ResetAllEnemies()
    {
        foreach(var e in enemies)
        {
            e.ResetEnemy();
        }
    }

    public void AddCoin(int amount)
    {
        if (finished) return;

        coins += amount;
        UpdateUI();

        if (coins >= winCoins)
        {
            FinishGame();
        }
        


    }

    private void UpdateUI()
    {
        coinsText.text = "Coins: " + coins;
    }

    private void FinishGame()
    {
        finished = true;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

#else
        Application.Quit();

#endif

    }


}
