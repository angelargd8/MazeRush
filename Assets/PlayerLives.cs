using UnityEngine;
using UnityEngine.UI;


public class PlayerLives : MonoBehaviour
{
    [SerializeField] int maxLives = 3;
    [SerializeField] Transform respawnPoint;
    [SerializeField] GameManager gameManager;
    public Text TextLives;


    public int Lives { get; private set; }

    CharacterController cc;
    Rigidbody rb;

    void Awake()
    {
        Lives = maxLives;
        cc = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
    }

    public void LoseLifeAndRespawn()
    {
        if (Lives <= 0) return;

        Lives--;
        UpdateUI();
        Respawn();

        if (Lives <= 0)
        {
            gameManager.finished = true;
            Debug.Log("Game Over");
            //se puede poner una UI o que se termine xd
        }
    }

    void Respawn()
    {
        if (cc != null) cc.enabled = false;

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        if (cc != null) cc.enabled = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }


    }

    private void UpdateUI()
    {
        TextLives.text = "Lives: " + Lives;
    }



}
