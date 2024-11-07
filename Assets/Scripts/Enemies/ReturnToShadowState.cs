using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToShadowState : State
{
    private Vector3 targetShadowPosition;
    private EnemyAI enemyAI;

    public ReturnToShadowState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _shadowPosition)
        : base(_npc, _agent, _anim, null)
    {
        npc = _npc;
        player = _player;
        anim = _anim;
        agent = _agent;
        targetShadowPosition = _shadowPosition;
        enemyAI = _npc.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        agent.isStopped = false;
        agent.SetDestination(targetShadowPosition);
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            enemyAI.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}
