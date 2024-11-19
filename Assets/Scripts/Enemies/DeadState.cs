using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DeadState : State
{
    private EnemyAI enemyScript;
    private EnemyData enemyData;

    private float deadTime;

    public DeadState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player) : base(_npc, _agent, _anim, _player)
    {
        name = STATE.DEAD;
        npc = _npc;
        agent = _agent;
        anim = _anim;
        player = _player;

        enemyScript = _npc.GetComponent<EnemyAI>();
        enemyData = enemyScript.enemyData;
    }


    public override void Enter()
    {
        anim.ResetTrigger("isThrowing");
        anim.ResetTrigger("isMelee");
        anim.ResetTrigger("isRunning");
        agent.ResetPath();
        agent.isStopped = true;
        anim.SetTrigger("isDying");
        base.Enter();
    }
    public override void Update()
    {
        deadTime += Time.deltaTime;

        if(deadTime > enemyData.deadTimer)
        {
            GameObject.Destroy(enemyScript.gameObject);
        }


        //base.Update();
    }

    public override void Exit()
    {
        base.Exit(); 
    }

}
