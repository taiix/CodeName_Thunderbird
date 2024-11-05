using UnityEngine;
using UnityEngine.AI;

public class PursueState : State
{
    public PursueState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
       : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        agent.speed = 5;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        base.Enter();

    }

    public override void Update()
    {
        agent.SetDestination(player.position);
        if (agent.hasPath)
        {
            if (CanAttackPlayer() && npc.GetComponent<EnemyAI>().CanAttack())
            {
                //nextState = new Attack(npc, agent, anim, player);
                //npc.GetComponent<AI>().ChangeCurrentState(nextState);
                //stage = EVENT.EXIT;
            }
            else if (!CanSeePlayer())
            {
                //nextState = new Patrol(npc, agent, anim, player);
                //npc.GetComponent<AI>().ChangeCurrentState(nextState);
                //stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}
