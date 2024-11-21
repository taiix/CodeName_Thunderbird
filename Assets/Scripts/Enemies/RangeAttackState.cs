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

        if (npcScript.NeedsToRetreat())
        {
                RetreatToShadow();
        }
        else
        {
            sunTimer = 0f; 
        }
    }


    private void HandleThrowAttack()
    {
        FacePlayer();
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("isThrowing");

            Vector3 throwDirection = (player.position - npc.transform.position).normalized;
            GameObject thrownStone = GameObject.Instantiate(weaponPrefab, npc.transform.position + Vector3.up * 1.5f, Quaternion.identity);

            Rigidbody stoneRb = thrownStone.GetComponent<Rigidbody>();
            if (stoneRb != null)
            {
                stoneRb.AddForce(throwDirection * 20, ForceMode.VelocityChange);
            }
            lastAttackTime = Time.time;
        }
    }

    private void RetreatToShadow()
    {
        npcScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
    }

    public override void Exit()
    {
        anim.ResetTrigger("isThrowing");
        base.Exit();
    }
}
