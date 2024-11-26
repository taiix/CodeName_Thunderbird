using UnityEngine;
using UnityEngine.AI;

public class State
{

    public enum STATE
    {
        IDLE, PATROl, PURSUE, RANGE_ATTACK, MELEE_ATTACK, DEAD, RUNAWAY, RETREAT, SHELTER
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject npc;
    protected Animator anim;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent;

    float visDist = 4.0f;
    float visAngle = 90.0f;
    float shootDistance = 2.0f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }


    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }


    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }

    public bool CanSeePlayer(float _visDistance, float _visAngle)
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);

        if (direction.magnitude < _visDistance && angle < _visAngle)
        {
            return true;
        }
        return false;
    }

    public bool IsPlayerBehind()
    {
        Vector3 direction = npc.transform.position - player.position;
        float angle = Vector3.Angle(direction, npc.transform.forward);
        if (direction.magnitude < 2 && angle < 30)
        {
            return true;
        }
        return false;
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - npc.transform.position;

        if (direction.magnitude < shootDistance)
        {
            return true;
        }
        return false;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 directionToTarget = (target - npc.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
        npc.transform.rotation = lookRotation;
    }

    public void StopAgent()
    {
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.velocity = Vector3.zero;
        anim.applyRootMotion = false;
    }

    public void StartAgent()
    {
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
        anim.applyRootMotion = true;
    }
}




