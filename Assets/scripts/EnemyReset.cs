using UnityEngine;
using UnityEngine.AI;

public class EnemyReset : MonoBehaviour
{
    
    Vector3 startPos;
    Quaternion startRot;
    NavMeshAgent agent;

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        agent = GetComponent<NavMeshAgent>();
    }

    public void ResetEnemy()
    {
        if (agent != null) agent.enabled = false;

        transform.position = startPos;
        transform.rotation = startRot;

        if (agent != null) agent.enabled = true;
    }
}
