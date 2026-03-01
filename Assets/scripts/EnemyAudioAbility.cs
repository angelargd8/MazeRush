using UnityEngine;

public class AudioEnemyAbility : MonoBehaviour
{
    [SerializeField] AudioClip enemySFX;

    private EnemyBehavior behavior;
    private bool wasChasing = false;

    private void Awake()
    {
        behavior = GetComponent<EnemyBehavior>();
    }

    private void Update()
    {
        if (behavior == null || enemySFX == null) return;

        bool chasingNow = behavior.IsChasing;

        // empezó a perseguir
        if (chasingNow && !wasChasing)
        {
            AudioManager.Instance.PlaySFX(enemySFX);
        }

        // dejó de perseguir
        if (!chasingNow && wasChasing)
        {
            AudioManager.Instance.StopSFX();
        }

        wasChasing = chasingNow;
    }
}