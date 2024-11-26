using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangeAttackState : State
{
    private float lastAttackTime;
    private float attackCooldown; 
    private float sunTimer;
    private float comfortZone; 
    private EnemyAI npcScript;
    private GameObject weaponPrefab;
    private EnemyData enemyData;

    public RangeAttackState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.RANGE_ATTACK;
        npcScript = _npc.GetComponent<EnemyAI>();
        enemyData = npcScript.enemyData;
        weaponPrefab = enemyData.weaponPrefab;
        attackCooldown = enemyData.attackSpeed;
        comfortZone = enemyData.attackRange * 0.8f; 
    }

    public override void Enter()
    {
        StopAgent();
        base.Enter();
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

        if (distanceToPlayer <= enemyData.attackRange)
        {
            LookAt(player.position);
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                anim.SetTrigger("isThrowing");
                lastAttackTime = Time.time;
            }
        }
        else if (distanceToPlayer > enemyData.attackRange)
        {
            npcScript.ChangeCurrentState(new PursueState(npc, agent, anim, player));
        }
    }


    public override void Exit()
    {
        anim.ResetTrigger("isThrowing");
        base.Exit();
    }
}
