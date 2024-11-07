using UnityEngine;
using UnityEngine.AI;

public class PursueState : State
{
    private EnemyAI npcScript;
    private float retreatDistance = 2.0f;

    private EnemyData enemyData;
    public PursueState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
       : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;

        agent.isStopped = false;
        npcScript = _npc.GetComponent<EnemyAI>();
        enemyData = npcScript.enemyData;
    }

    public override void Enter()
    {
        agent.speed = 6.5f;
        Debug.Log("Agent speed set to: " + agent.speed);
        anim.SetTrigger("isRunning");
        agent.SetDestination(player.position);
        base.Enter();
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);


        if (distanceToPlayer <= enemyData.attackRange &&
            distanceToPlayer > enemyData.optimalAttackDistance)
        {
            FacePlayer();
            agent.SetDestination(player.position);
        }
        if (distanceToPlayer <= enemyData.optimalAttackDistance)
        {
            if (enemyData.enemyType == EnemyType.Ranged)
            {
                npcScript.ChangeCurrentState(new RangeAttackState(npc, agent, anim, player));
            }
            else if (enemyData.enemyType == EnemyType.Melee)
            {
                npcScript.ChangeCurrentState(new MeleeAttackState(npc, agent, anim, player));
            }
        }
        if (distanceToPlayer >= enemyData.spottingRange)
        {
            npcScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
        }
        //else
        //{
        //    agent.SetDestination(player.position);
        //}
    }

    private void FacePlayer()
    {
        Vector3 directionToPlayer = (player.position - npc.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        // Only rotate if the angle between the current and desired rotation is significant
        float angle = Quaternion.Angle(npc.transform.rotation, lookRotation);
        if (angle > 1f) 
        {
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, lookRotation, 1f * Time.deltaTime);
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        agent.speed = 3.5f;
        agent.ResetPath();
        base.Exit();
    }
}
