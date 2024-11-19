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

    private EnemyData enemyData;

    public PatrolState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        anim = _anim;
        name = STATE.PATROl;
        agent.speed = 2;
        agent.isStopped = false;
        npcScript = _npc.GetComponent<EnemyAI>();
        enemyData = npcScript.enemyData;
    }

    public override void Enter()
    {
        StartAgent();
        anim.SetTrigger("isWalking");
        idleTimer = 0f;
        isIdling = false;
        Patrol();
        base.Enter();
    }

    public override void Update()
    {
        if (npcScript.IsInShadow())
        {

            if (Vector3.Distance(npc.transform.position, player.position) <= enemyData.spottingRange)
            {
                npcScript.ChangeCurrentState(new PursueState(npc, agent, anim, player));
                return;
            }
        }
        if (isIdling)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime)
            {
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
        StopAgent();
        //Random Idle time
        idleTime = Random.Range(2f, 4.5f);
        idleTimer = 0f;
        isIdling = true;
        anim.SetTrigger("isIdle");
    }

    private void Patrol()
    {
        isIdling = false;
        float randomDistance = Random.Range(10.0f, 25.0f);
        Vector3 randomDirection = Random.insideUnitSphere * randomDistance;
        randomDirection += npcScript.transform.position;
        NavMeshHit navHit;
        StartAgent();
        if (NavMesh.SamplePosition(randomDirection, out navHit, randomDistance, NavMesh.AllAreas))
        {
            Vector3 targetPosition = navHit.position;
            anim.SetTrigger("isWalking");
            if (npcScript.IsPointInShadow(targetPosition))
            {
                agent.SetDestination(targetPosition);
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
        Debug.Log("Adjust Direction");
        float randomDistance = Random.Range(10.0f, 35.0f);
        for (int i = 0; i < 5; i++)
        {
            Vector3 adjustedDirection = Quaternion.Euler(0, Random.Range(-15.0f, 15.0f), 0) * (targetPosition - npcScript.transform.position).normalized;
            Vector3 newTarget = npcScript.transform.position + adjustedDirection * randomDistance;

            if (npcScript.IsPointInShadow(newTarget))
            {
                StartAgent();
                anim.SetTrigger("isWalking");
                agent.SetDestination(newTarget);
                return;
            }
        }
        //StartIdle();
    }

    private void AdjustAnimationAndSpeedBasedOnShadow()
    {
        bool isCurrentlyInShadow = npcScript.IsInShadow();
        if (agent.velocity.magnitude > 0f)
        {
            if (isCurrentlyInShadow && !isWalking)
            {
                anim.SetTrigger("isWalking");
                agent.speed = 3.5f; 
                isWalking = true;
                isRunning = false;
            }
            else if (!isCurrentlyInShadow && !isRunning)
            {
                anim.SetTrigger("isRunning");
                agent.speed = 6.5f;
                isWalking = false;
                isRunning = true;
            }
        }
        else
        {
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
        base.Exit();
    }
}
