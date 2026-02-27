using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEditor;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering;


public class EnemyBehavior : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }

    [SerializeField] Transform objective;

    [SerializeField] List<Transform> waypoints;
    [SerializeField] float waitTime = 3.0f;

    //cono de vision
    private float viewRadius = 3.0f;
    private float viewAngle = 90.0f;

    private int wpIndex = 0;

    //distancias
    [SerializeField] float attackDistance = 1.6f;
    [SerializeField] float loseDistance = 8.0f;


    private NavMeshAgent agent => GetComponent<NavMeshAgent>();

    private EnemyState currentState = EnemyState.Patrol;

    private Coroutine patrolCoroutine; 

    private void Start()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        wpIndex = Random.Range(0, waypoints.Count);
        agent.SetDestination(waypoints[wpIndex].transform.position);
    }


    void Update()
    {
        

        switch(currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;

            default:
                break;
        }



    }

    private void Patrol()
    {
        // llegar al waypoint
        if (agent.remainingDistance < 0.5f && !agent.isStopped)
        {
          
            if (patrolCoroutine == null)
                patrolCoroutine = StartCoroutine(PatrolPoint());
        }

        // empezar a perseguir si lo ve
        if (LookForObjective())
        {
            currentState = EnemyState.Chase;
            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }
            agent.isStopped = false;

        }
    }

    private void Chase()
    {
        //agent.SetDestination(objective.position);

        if (objective == null)
        {
            currentState = EnemyState.Patrol;
            return;

        }

        agent.isStopped = false;
        agent.SetDestination(objective.position);

        float dist = Vector3.Distance(transform.position, objective.position);

        // Atacar si esta cerca
        if (dist <= attackDistance)
        {
            currentState = EnemyState.Attack;
            return;
        }
        //que lo puerda si ya esta lejos y vuelva a patrol
        if (dist > loseDistance || !LookForObjective())
        {
            ReturnToPatrol();
            return;
        }




    }

    private void Attack()
    {
        //cuando se me acerca lo suficiente me paso a ataque
        // si lo dejo atras pasa a patrullaje

        if (objective == null)
        {
            ReturnToPatrol();
            return;
        }

        float dist = Vector3.Distance(transform.position, objective.position);

        // si se aleja, vuelve a perseguir o patrullar
        if (dist > attackDistance)
        {
            if (dist > loseDistance || !LookForObjective())
                ReturnToPatrol();
            else
                currentState = EnemyState.Chase;

            return;
        }

        agent.isStopped = true;

        // Solo mirar al jugador
        Vector3 lookPos = new Vector3(objective.position.x, transform.position.y, objective.position.z);
        transform.LookAt(lookPos);
    }


    private void ReturnToPatrol()
    {
        currentState = EnemyState.Patrol;
        agent.isStopped = false;

        if (waypoints != null && waypoints.Count > 0)
            agent.SetDestination(waypoints[wpIndex].position);
    }


    private bool LookForObjective()
    {
        if (objective == null)
        {
            return false;
        }

        Vector3 dir = objective.position - transform.position;

        if( dir.magnitude > viewRadius)
            return false;

        float angleToOjective = Vector3.Angle(transform.forward, dir.normalized);

        if ( angleToOjective > viewAngle / 2.0f)
            return false;

        //que no este detras de una pared
        if(Physics.Raycast(transform.position + Vector3.up, dir.normalized, out RaycastHit hit, viewRadius))
        {
            if (hit.transform == objective)
                return true; 
        }

        return false;
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
        patrolCoroutine = null;
    }

}
