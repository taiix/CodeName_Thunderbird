using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToShadowState : State
{
    private Vector3 targetShadowPosition;
    private EnemyAI enemyScript;
    private float damageTimer = 0f;
    private bool wasInShelter = false;
    private Vector3 shelterLocation;

    public ReturnToShadowState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Vector3 _shelterPosition, Transform _player, Vector3 _shadowPosition, bool isInShelter)
        : base(_npc, _agent, _anim, null)
    {
        name = STATE.RETREAT;
        npc = _npc;
        player = _player;
        anim = _anim;
        agent = _agent;
        targetShadowPosition = _shadowPosition;
        wasInShelter = isInShelter;
        shelterLocation = _shelterPosition;
        enemyScript = _npc.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        StartAgent();
        agent.SetDestination(targetShadowPosition);
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (wasInShelter)
            {
                enemyScript.ChangeCurrentState(new ReturnToShelter(npc, agent, anim, player, shelterLocation));
                return;
            }

            if (enemyScript.IsPointInShadow(npc.transform.position))
            {
                enemyScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
            }
            else
            {
                // Find next shadow position if not already in one
                Vector3 newShadowPosition = FindNextShadowPosition(npc.transform.position);
                if (newShadowPosition != npc.transform.position)
                {
                    targetShadowPosition = newShadowPosition;
                    agent.SetDestination(targetShadowPosition);
                }
            }
        }


        // Deal 1 health damage every 1.5 seconds
        damageTimer += Time.deltaTime;
        if (damageTimer >= 1.5f)
        {
            Debug.Log("Take Damage");
            enemyScript.TakeDamage(1);
            damageTimer = 0f;
        }

    }

    private Vector3 FindNextShadowPosition(Vector3 currentPosition)
    {
        float maxSearchDistance = 100f;
        float stepSize = 1f;
        Vector3 direction = targetShadowPosition - currentPosition;

        for (float i = stepSize; i <= maxSearchDistance; i += stepSize)
        {
            Vector3 testPosition = currentPosition + direction * i;
            if (enemyScript.IsPointInShadow(testPosition))
            {
                return testPosition;
            }
        }

        //If no point in shadow is found 
        Debug.Log("No new position found going to shelter");
        return shelterLocation;
    }


    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        anim.ResetTrigger("isWalking");
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}
