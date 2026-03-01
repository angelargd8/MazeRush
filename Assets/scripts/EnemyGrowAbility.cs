using UnityEngine;
using UnityEngine.AI;

public class EnemyGrowOnSee : MonoBehaviour
{
    [SerializeField] float growMultiplier = 0.2f;
    [SerializeField] float growSpeed = 6f;

    private NavMeshAgent agent;
    private EnemyBehavior behavior;

    private Vector3 originalScale;
    private float originalRadius;
    private float originalHeight;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        behavior = GetComponent<EnemyBehavior>();

        originalScale = transform.localScale;

        if (agent != null)
        {
            originalRadius = agent.radius;
            originalHeight = agent.height;
        }
    }

    private void Update()
    {
        if (agent == null || behavior == null) return;

        // lo esta siguiendo
        bool seesPlayer = behavior.IsChasing;

        Vector3 targetScale = seesPlayer ? originalScale * growMultiplier : originalScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * growSpeed);

        // ajustar el NavMeshAgent
        float targetRadius = seesPlayer ? originalRadius * growMultiplier : originalRadius;
        float targetHeight = seesPlayer ? originalHeight * growMultiplier : originalHeight;

        agent.radius = Mathf.Lerp(agent.radius, targetRadius, Time.deltaTime * growSpeed);
        agent.height = Mathf.Lerp(agent.height, targetHeight, Time.deltaTime * growSpeed);
    }
}