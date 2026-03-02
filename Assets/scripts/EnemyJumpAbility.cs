using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyJumpAbility : MonoBehaviour
{
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float jumpDuration = 0.6f;
    [SerializeField] float jumpInterval = 7f;
    [Header("Animation")]
    [SerializeField] Animator anim;

    private float nextJumpTime = 0f;
    private bool isJumping = false;

    private NavMeshAgent agent;
    private EnemyBehavior behavior;

    private Vector3 startPosition;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        behavior = GetComponent<EnemyBehavior>();
    }

    private void Update()
    {
        if (behavior == null || agent == null) return;

        // Solo salta si esta persiguiendo
        if (!behavior.IsChasing) return;

        if (Time.time >= nextJumpTime && !isJumping)
        {
            nextJumpTime = Time.time + jumpInterval;
            StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump()
    {
        isJumping = true;
        anim.SetTrigger("Jump");


        startPosition = transform.position;

        float time = 0f;

        while (time < jumpDuration)
        {
            
            float normalized = time / jumpDuration;

            
            float height = 4f * jumpHeight * normalized * (1f - normalized);

            transform.position = startPosition + Vector3.up * height;

            time += Time.deltaTime;
            
            yield return null;
        }

        transform.position = startPosition;
        isJumping = false;
    }
}