using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangeAttackState : State
{
    private float lastAttackTime;
    private float attackCooldown;  // Set this dynamically based on the attackSpeed property
    private EnemyAI npcScript;
    private GameObject stonePrefab;

    public RangeAttackState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
        : base(_npc, _agent, _anim, _player)
    {
        name = STATE.RANGE_ATTACK;
        npcScript = _npc.GetComponent<EnemyAI>();
        stonePrefab = npcScript.StonePrefab(); 
        attackCooldown = npcScript.AttackSpeed(); 
    }

    public override void Enter()
    {
        base.Enter();
        agent.isStopped = true; 
    }

    public override void Update()
    {
        // Check if the enemy is within range to throw stones at the player
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);
        if (distanceToPlayer <= npcScript.RangedAttackRange())
        {
            HandleThrowAttack();  // Start the stone throwing attack
        }
        else
        {
            // If the player is out of range, stay idle or handle other logic
            // For now, just stay in the range attack state until the player is in range
        }
    }

    private void HandleThrowAttack()
    {
        // Trigger throwing animation if the cooldown has passed
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("isThrowing");  

            // Instantiate the stone prefab and set its position and direction
            Vector3 throwDirection = (player.position - npc.transform.position).normalized;
            GameObject thrownStone = GameObject.Instantiate(stonePrefab, npc.transform.position + Vector3.up * 1.5f, Quaternion.identity);

            // Apply force to the stone (simulate throwing)
            Rigidbody stoneRb = thrownStone.GetComponent<Rigidbody>();
            if (stoneRb != null)
            {
                stoneRb.AddForce(throwDirection * 20, ForceMode.VelocityChange);
            }

            // Reset the attack time for cooldown
            lastAttackTime = Time.time;
        }
    }

    public override void Exit()
    {
        // Reset the throwing animation when exiting the state
        anim.ResetTrigger("isThrowing");
        base.Exit();
    }
}
