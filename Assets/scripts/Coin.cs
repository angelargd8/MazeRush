using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] AudioClip CoinSFX;
    public float rotationSpeed = 180f;
        
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {   
            AudioManager.Instance.PlaySFX(CoinSFX);
            GameManager.instance.AddCoin(1);
            Destroy(gameObject);
        }
        else
        {
            return;
        }

    }

    public void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }

}
