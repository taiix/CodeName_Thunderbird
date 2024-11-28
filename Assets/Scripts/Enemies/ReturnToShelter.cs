using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToShelter : State
{
    private Transform locationBeforeShelter;
    private Vector3 shelterLocation;
    private EnemyAI npcScript;
    private EnemyData enemyData;

    private float insultRange = 20f;
    private bool isInShelter = false;

    public ReturnToShelter(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Vector3 _shelterLocation) : base(_npc, _agent, _anim, _player)
    {
        name = STATE.SHELTER;
        npc = _npc;
        agent = _agent;
        anim = _anim;
        shelterLocation = _shelterLocation;
        player = _player;
        npcScript = npc.GetComponent<EnemyAI>();
        enemyData = npcScript.enemyData;

    }

    public override void Enter()
    {
        StartAgent();
        anim.SetTrigger("isRunning");
        agent.speed = enemyData.runningSpeed;
        agent.SetDestination(shelterLocation);
        Debug.Log("Enemy returning to shelter");
        base.Enter();
    }

    public override void Update()
    {
        if (agent.pathPending) return;

        // Check if reached shelter
        if (!isInShelter && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            anim.ResetTrigger("isRunning");
            anim.SetTrigger("isSitting");
            StopAgent();
            isInShelter = true;
        }

        // Player proximity check
        if (isInShelter)
        {
            float distanceToPlayer = Vector3.Distance(player.position, npc.transform.position);

            if (distanceToPlayer <= enemyData.attackRange)
            {
                npcScript.ChangeCurrentState(new PursueState(npc, agent, anim, player));
            }
            else if (distanceToPlayer <= enemyData.spottingRange)
            {
                npc.transform.LookAt(player.position);
                anim.SetTrigger("isInsulting");
                anim.ResetTrigger("isSitting");
            }
            else
            {
                anim.ResetTrigger("isInsulting");
                anim.SetTrigger("isSitting");
            }
        }

        base.Update();
    }

    public override void Exit()
    {
        anim.ResetTrigger("isInsulting");
        anim.ResetTrigger("isSitting");
        base.Exit();
    }
}
