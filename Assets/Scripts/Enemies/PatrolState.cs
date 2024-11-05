using UnityEngine;
using UnityEngine.AI;

public class PatrolState : State
{
    private int currentIndex = -1;
    private EnemyAI npcScript;

    public PatrolState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        anim = _anim;
        name = STATE.PATROl; 
        agent.speed = 1;
        agent.isStopped = false;
        npcScript = _npc.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isWalking");
        currentIndex = -1;
        base.Enter();
    }

    public override void Update()
    {
        if (CanSeePlayer() && npcScript.CanAttack())
        {
            nextState = new PursueState(npc, agent, anim, player);
            npcScript.ChangeCurrentState(nextState);
            stage = EVENT.EXIT;
            return; 
        }

        if (IsPlayerBehind() && npcScript.CanBeScared())
        {
            // nextState = new RunAway(npc, agent, anim, player);
            npcScript.ChangeCurrentState(nextState);
            stage = EVENT.EXIT;
            return; 
        }

        if (agent.remainingDistance < 1)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (npcScript.CanPatrol()) 
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10.0f; // Example patrol distance
            randomDirection += npcScript.transform.position; // Offset by current position

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, 10.0f, NavMesh.AllAreas))
            {
                Vector3 targetPosition = navHit.position;

                if (npcScript.IsPointInShadow(targetPosition))
                {
                    agent.SetDestination(targetPosition);
                    //anim.SetTrigger("isWalking");
                }
                else
                {
                    AdjustDirection(targetPosition);
                }
            }
        }
    }

    private void AdjustDirection(Vector3 targetPosition)
    {
        for (int i = 0; i < 5; i++) 
        {
            Vector3 adjustedDirection = Quaternion.Euler(0, Random.Range(-15.0f, 15.0f), 0) * (targetPosition - npcScript.transform.position).normalized;
            Vector3 newTarget = npcScript.transform.position + adjustedDirection * 5.0f; 

            if (npcScript.IsPointInShadow(newTarget))
            {
                agent.SetDestination(newTarget);
                return; 
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isWalking");
        base.Exit();
    }
}
