using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToShadowState : State
{
    private Vector3 targetShadowPosition;
    private EnemyAI enemyScript;
    private float damageTimer = 0f;

    public ReturnToShadowState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _shadowPosition)
        : base(_npc, _agent, _anim, null)
    {
        npc = _npc;
        player = _player;
        anim = _anim;
        agent = _agent;
        targetShadowPosition = _shadowPosition;
        enemyScript = _npc.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(targetShadowPosition);
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && enemyScript.GetHealth() > 0)
        {
            enemyScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
        }
        else
        {
            // Deal 1 health damage every second
            damageTimer += Time.deltaTime;
            if (damageTimer >= 1f)
            {
                enemyScript.TakeDamage(1);
                damageTimer = 0f;
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        anim.ResetTrigger("isWalking");
        anim.ResetTrigger("isIdle");
        agent.isStopped = false;
        base.Exit();
    }
}
