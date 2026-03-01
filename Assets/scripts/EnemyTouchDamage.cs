using UnityEngine;

public class EnemyTouchDamage : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerLives lives = other.GetComponent<PlayerLives>();
        if (lives == null) return;

        lives.LoseLifeAndRespawn();

        if (gameManager != null)
            gameManager.ResetAllEnemies();
    }
}
