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
        name = STATE.RETREAT;
        npc = _npc;
        player = _player;
        anim = _anim;
        agent = _agent;
        targetShadowPosition = _shadowPosition;
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
            if (enemyScript.IsPointInShadow(npc.transform.position))
            {
                enemyScript.ChangeCurrentState(new PatrolState(npc, agent, anim, player));
            }
            else
            {
                Vector3 newShadowPosition = FindNextShadowPosition(npc.transform.position);
                if (newShadowPosition != npc.transform.position)
                {
                    targetShadowPosition = newShadowPosition;
                    agent.SetDestination(targetShadowPosition);
                }
            }
        }


        // Deal 1 health damage every 2 seconds
        damageTimer += Time.deltaTime;
        if (damageTimer >= 2f)
        {
            Debug.Log("Take Damage");
            enemyScript.TakeDamage(1);
            damageTimer = 0f;
        }

    }

    private Vector3 FindNextShadowPosition(Vector3 currentPosition)
    {
        float maxSearchDistance = 50f;
        float stepSize = 2f;
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
        return currentPosition;
    }


    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        anim.ResetTrigger("isWalking");
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}
