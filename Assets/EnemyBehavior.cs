using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;

    private int wpIndex = 0;

    private NavMeshAgent agent => GetComponent<NavMeshAgent>();

    private void Start()
    {
        agent.SetDestination(waypoints[wpIndex].transform.position);
    }


    void Update()
    {
        
    }


}
