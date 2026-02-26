using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;
using System.Collections;


public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float waitTime = 3.0f;

    private int wpIndex = 0;

    private NavMeshAgent agent => GetComponent<NavMeshAgent>();

    private void Start()
    {
        wpIndex = Random.Range(0, waypoints.Count);
        agent.SetDestination(waypoints[wpIndex].transform.position);
        

    }


    void Update()
    {
        if(agent.remainingDistance < 0.5f && !agent.isStopped) //0.5 por el stopping distance
        {
            StartCoroutine(PatrolPoint());

            

        }
    }

    IEnumerator PatrolPoint()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        int newIndex = 0;
        do
        {
            newIndex = Random.Range(0, waypoints.Count);

        } while (newIndex == wpIndex);

        wpIndex = newIndex;
        agent.SetDestination(waypoints[wpIndex].transform.position);

        agent.isStopped = false;
    }

}
