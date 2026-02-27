using UnityEngine;
using UnityEngine.AI;

public class EnemyJumpAbility : MonoBehaviour
{
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float jumpDuration = 0.6f;
    [SerializeField] float jumpInterval = 7f;

    private float nextJumpTime = 0f;
    private bool isJumping = false;

    private NavMeshAgent agent;
    private EnemyBehavior brain;

    private Vector3 startPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        brain = GetComponent<EnemyBehavior>();
    }

    private void Update()
    {
        if (brain == null || agent == null) return;

        // Solo salta si esta persiguiendo
        if (!brain.IsChasing) return;

        if (Time.time >= nextJumpTime && !isJumping)
        {
            nextJumpTime = Time.time + jumpInterval;
            StartCoroutine(Jump());
        }
    }

    private System.Collections.IEnumerator Jump()
    {
        isJumping = true;

        startPosition = transform.position;

        float time = 0f;

        while (time < jumpDuration)
        {
            float normalized = time / jumpDuration;

            // curva parabolica par a el salto
            float height = 4f * jumpHeight * normalized * (1f - normalized);

            transform.position = startPosition + Vector3.up * height;

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
        isJumping = false;
    }
}