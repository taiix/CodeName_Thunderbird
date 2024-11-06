using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    private EnemyAI npcScript;

    private float idleTime;
    private float idleTimer;
    private bool isIdling;

    private bool isRunning = false;
    private bool isWalking = false;

    public PatrolState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        anim = _anim;
        name = STATE.PATROl;
        agent.speed = 2;
        agent.isStopped = false;
        npcScript = _npc.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isWalking");
        idleTimer = 0f;
        isIdling = false;
        Patrol();
        base.Enter();
    }

    public override void Update()
    {

        if (Vector3.Distance(npc.transform.position, player.position) <= npcScript.SpottingRange())
        {
            // Transition to PursueState
            State pursueState = new PursueState(npc, agent, anim, player);
            npcScript.ChangeCurrentState(pursueState);
            return;
        }

        // If currently idling, update the timer
        if (isIdling)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
                // Idle time is over, continue patrolling
                isIdling = false;
                Patrol();

            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartIdle();
        }
        if (!isIdling)
        {
            AdjustAnimationAndSpeedBasedOnShadow();
        }
    }

    private void StartIdle()
    {
        //Random Idle time
        idleTime = Random.Range(2f, 4.5f);
        idleTimer = 0f;
        isIdling = true;
        anim.SetTrigger("isIdle");
        agent.isStopped = true;
    }

    private void Patrol()
    {
        float randomDistance = Random.Range(10.0f, 35.0f);
        Vector3 randomDirection = Random.insideUnitSphere * randomDistance;
        randomDirection += npcScript.transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, randomDistance, NavMesh.AllAreas))
        {
            Vector3 targetPosition = navHit.position;
            anim.SetTrigger("isWalking");
            if (npcScript.IsPointInShadow(targetPosition))
            {
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
                //Debug.Log($"Patrolling to new shadow point: {targetPosition}");
            }
            else
            {
                AdjustDirection(targetPosition);
            }
        }
        else
        {
            StartIdle();
        }
    }

    private void AdjustDirection(Vector3 targetPosition)
    {
        float randomDistance = Random.Range(10.0f, 35.0f);
        for (int i = 0; i < 5; i++)
        {
            Vector3 adjustedDirection = Quaternion.Euler(0, Random.Range(-15.0f, 15.0f), 0) * (targetPosition - npcScript.transform.position).normalized;
            Vector3 newTarget = npcScript.transform.position + adjustedDirection * randomDistance;

            if (npcScript.IsPointInShadow(newTarget))
            {
                agent.isStopped = false;
                agent.SetDestination(newTarget);
                return;
            }
        }
        //StartIdle();
    }

    private void AdjustAnimationAndSpeedBasedOnShadow()
    {
        bool isCurrentlyInShadow = npcScript.IsInShadow();

        // If the enemy is moving towards a patrol point, adjust its animation and speed
        if (agent.velocity.magnitude > 0f)  // Only adjust if the enemy is moving
        {
            if (isCurrentlyInShadow && !isWalking)
            {
                // Switch to walking if in shadow
                anim.SetTrigger("isWalking");
                agent.speed = 3.5f; // Walking speed
                isWalking = true;
                isRunning = false;
            }
            else if (!isCurrentlyInShadow && !isRunning)
            {
                // Switch to running if not in shadow
                anim.SetTrigger("isRunning");
                agent.speed = 6.5f; // Running speed
                isWalking = false;
                isRunning = true;
            }
        }
        else
        {
            // If the enemy is not moving, reset to idle animation
            if (!isWalking && !isRunning)
            {
                anim.SetTrigger("isIdle");
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isWalking");
        anim.ResetTrigger("isIdle");
        agent.isStopped = false;
        base.Exit();
    }
}
