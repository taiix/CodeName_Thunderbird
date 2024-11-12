using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : State
{
    private float lastAttackTime;
    private float attackCooldown;
    private EnemyAI npcScript;
    private EnemyData enemyData;

    public MeleeAttackState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.MELEE_ATTACK;
        npcScript = _npc.GetComponent<EnemyAI>();
        enemyData = npcScript.enemyData;
        attackCooldown = enemyData.attackSpeed;
    }

    public override void Enter()
    {
        agent.ResetPath();
        agent.isStopped = true;
        base.Enter();
    }

    public override void Update()
    {
        if (npcScript.NeedsToRetreat())
        {
            npcScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
            return;
        }

        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

        if (distanceToPlayer <= enemyData.attackRange)
        {
            FacePlayer();
            HandleMeleeAttack();
        }
        else
        {
            npcScript.ChangeCurrentState(new PursueState(npc, agent, anim, player));
        }
    }

    private void HandleMeleeAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("isMelee");
            lastAttackTime = Time.time;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isMelee");
        base.Exit();
    }
}
