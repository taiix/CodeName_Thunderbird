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
    private Collider attackRangeCollider;
    //private bool isPlayerInRange = false;
    private float heightDifferenceThreshold = 2f;

    private bool isAttacking = false;
    private float originalAnimSpeed = 1f;


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
        StopAgent();
        agent.ResetPath();
        base.Enter();
    }

    public override void Update()
    {
        //if (npcScript.NeedsToRetreat())
        //{
        //    npcScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
        //    return;
        //}

        //if (isAttacking)
        //{
        //    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);


        //    if (stateInfo.IsName("Melee Attack") && stateInfo.normalizedTime >= 1f)
        //    {
        //        anim.speed = originalAnimSpeed;
        //        isAttacking = false; 
        //    }
        //}

        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);
        float heightDifference = player.position.y - npc.transform.position.y;

       
        if (distanceToPlayer <= enemyData.attackRange)
        {
            if (heightDifference > heightDifferenceThreshold)
            {
                Debug.Log("handle above attack");
                
                HandleAbovePlayerAttack();
            }
            else
            {
                
                HandleMeleeAttack();
            }
           
        }
        else 
        {
            npcScript.ChangeCurrentState(new PursueState(npc, agent, anim, player));
        }
    }

    private void HandleMeleeAttack()
    {
        LookAt(player.transform.position);
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            //float animationSpeed = npcScript.enemyData.attackSpeed;
            //anim.speed = animationSpeed;

            anim.SetFloat("attackSpeed", npcScript.enemyData.attackSpeed);
            // Trigger melee animation
            anim.SetTrigger("isMelee");


            lastAttackTime = Time.time;
            isAttacking = true;
            originalAnimSpeed = anim.speed;


        }
    }

    private void HandleAbovePlayerAttack()
    {
        anim.SetTrigger("upAttack");
    }

    public override void Exit()
    {
        anim.ResetTrigger("isMelee");
        anim.ResetTrigger("upAttack");
        base.Exit();
    }
}
