using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReturnToShelter : State
{
    private Transform locationBeforeShelter;
    private Transform shelterLocation;
    private EnemyAI npcScript;
    private EnemyData enemyData;

    private float insultRange = 20f;
    private bool isInShelter = false;

    public ReturnToShelter(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player, Transform _shelterLocation) : base(_npc, _agent, _anim, _shelterLocation)
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
        agent.speed = 7;
        if (shelterLocation != null) agent.SetDestination(shelterLocation.position);
        Debug.Log("AGENT REMAINING DISTANCE = " + agent.remainingDistance);

        base.Enter();
    }

    public override void Update()
    {
        if (agent.pathPending) return;

        if (!isInShelter && agent.remainingDistance <= agent.stoppingDistance + 0.5f)
        {
            anim.ResetTrigger("isRunning");
            anim.SetTrigger("isSitting");
            StopAgent();
            isInShelter = true;
        }

        if (isInShelter)
        {
            float distanceToPlayer = Vector3.Distance(player.position, npc.transform.position);

            if (distanceToPlayer <= enemyData.attackRange)
            {
                npcScript.ChangeCurrentState(new PursueState(npc, agent, anim, player));
            }

            if (distanceToPlayer <= enemyData.spottingRange)
            {
                FacePlayer();
                anim.ResetTrigger("isSitting");
                anim.SetTrigger("isInsulting");
                Debug.Log("Enemy is insulting the player!");

            }
            else
            {
                // anim.ResetTrigger("isInsulting");
                anim.SetTrigger("isSitting");
                //Debug.Log("Enemy goes back to sitting.");
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
