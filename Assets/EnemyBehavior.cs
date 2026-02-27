using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    private enum EnemyState { Patrol, Chase, Attack }

    [Header("Target")]
    [SerializeField] Transform objective;

    [Header("Patrol")]
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float waitTime = 3.0f;

    [Header("Speeds")]
    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float chaseSpeed = 6f;

    [Header("Vision")]
    [SerializeField] float viewRadius = 10.0f;
    [SerializeField] float viewAngle = 180.0f;
    //tiempo para recordar
    [SerializeField] float rememberTime = 1.25f;
    //altura en donde lo mira
    [SerializeField] float eyeHeight = 1.6f;

    [Header("Raycast Layers")]
    //paredes o obstaculos
    [SerializeField] LayerMask obstacleMask;
    //jugador
    [SerializeField] LayerMask targetMask;

    [Header("Distances")]
    [SerializeField] float attackDistance = 1.6f;
    [SerializeField] float loseDistance = 20.0f;

    private int wpIndex = 0;
    private EnemyState currentState = EnemyState.Patrol;
    private Coroutine patrolCoroutine;

    private float lastTimeSawObjective = -999f;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        agent.isStopped = true;
        StartCoroutine(StartDelay());

        wpIndex = Random.Range(0, waypoints.Count);
        agent.SetDestination(waypoints[wpIndex].position);
    }

    private void Update()
    {
        switch (currentState)
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

        }

    }

    private void Patrol()
    {
        agent.speed = patrolSpeed;

        // llegar al waypoint
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f && !agent.isStopped)
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
        agent.speed = chaseSpeed;

        if (objective == null)
        {
            ReturnToPatrol();
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(objective.position);

        float dist = Vector3.Distance(transform.position, objective.position);

        // atacar si está cerca
        if (dist <= attackDistance)
        {
            currentState = EnemyState.Attack;
            return;
        }

        // perderlo si esta muy lejos o si no lo ve y no lo ha visto recientemente
        bool seesNow = LookForObjective();
        if (dist > loseDistance || (!seesNow && !SawRecently()))
        {
            ReturnToPatrol();
            return;
        }
    }

    private void Attack()
    {
        if (objective == null)
        {
            ReturnToPatrol();
            return;
        }

        float dist = Vector3.Distance(transform.position, objective.position);

        // si se aleja, vuelve a perseguir o patrullar
        if (dist > attackDistance)
        {
            bool seesNow = LookForObjective();

            if (dist > loseDistance || (!seesNow && !SawRecently()))
                ReturnToPatrol();
            else
                currentState = EnemyState.Chase;

            return;
        }

        agent.isStopped = true;

        // solo ver al jugador
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

    private bool SawRecently()
    {
        return Time.time - lastTimeSawObjective <= rememberTime;
    }

    private bool LookForObjective()
    {
        if (objective == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = objective.position + Vector3.up * 1.0f;

        Vector3 dir = targetPos - eyePos;
        float dist = dir.magnitude;

        if (dist > viewRadius) return false;

        float angleToObjective = Vector3.Angle(transform.forward, dir.normalized);
        if (angleToObjective > viewAngle * 0.5f) return false;

        // si hay pared en medio, no lo ve
        if (obstacleMask.value != 0)
        {
            if (Physics.Raycast(eyePos, dir.normalized, dist, obstacleMask))
                return false;
        }

        // confirmar que es el jugador
        bool hitTarget = true;
        if (targetMask.value != 0)
        {
            hitTarget = Physics.Raycast(eyePos, dir.normalized, dist, targetMask);
        }

        if (hitTarget)
        {
            // memoria
            lastTimeSawObjective = Time.time; 
            return true;
        }

        return false;
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);
        agent.isStopped = false;

        if (waypoints != null && waypoints.Count > 0)
        {
            wpIndex = Random.Range(0, waypoints.Count);
            agent.SetDestination(waypoints[wpIndex].position);
        }
    }


    private IEnumerator PatrolPoint()
    {
        agent.isStopped = true;

        yield return new WaitForSeconds(waitTime);

        int newIndex;
        do
        {
            newIndex = Random.Range(0, waypoints.Count);

        } while (newIndex == wpIndex);

        wpIndex = newIndex;


        agent.isStopped = false;
        agent.SetDestination(waypoints[wpIndex].position);

        patrolCoroutine = null;
    }

    public Transform Objective => objective;
    public bool IsChasing => currentState == EnemyState.Chase;

}