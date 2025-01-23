using UnityEngine;
using UnityEngine.AI;

public class PursueState : State
{
    private EnemyAI npcScript;
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
        StartAgent();
        agent.speed = enemyData.runningSpeed;
        anim.SetTrigger("isRunning");
        agent.SetDestination(player.position);
        LookAt(player.position);  
        base.Enter();
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

        agent.SetDestination(player.position);
        LookAtPlayer(player.transform.position);

        //if (distanceToPlayer <= enemyData.attackRange &&
        //    distanceToPlayer > enemyData.optimalAttackDistance)
        //{
        //    LookAtPlayer(player.transform.position);
        //    agent.SetDestination(player.position);
        //}
        if (distanceToPlayer <= enemyData.optimalAttackDistance)
        {
            npcScript.ChooseAttackState();
        }
        if (distanceToPlayer >= enemyData.spottingRange)
        {
            npcScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
        }
    }

    private void LookAtPlayer(Vector3 target)
    {
        Vector3 directionToTarget = (target - npc.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        npc.transform.rotation = Quaternion.RotateTowards(
            npc.transform.rotation,
            targetRotation,
            500 * Time.deltaTime // Ensure smooth rotation
        );
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        anim.ResetTrigger("isWalking");
        anim.ResetTrigger("isIdle");
        agent.ResetPath();
        base.Exit();
    }
}
