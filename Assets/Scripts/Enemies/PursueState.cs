using UnityEngine;
using UnityEngine.AI;

public class PursueState : State
{
    private EnemyAI npcScript;
    private float retreatDistance = 2.0f; // Distance to retreat back into shadow

    public PursueState(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
       : base(_npc, _agent, _anim, _player)
    {
        name = STATE.PURSUE;
        agent.speed = 6.5f;
        agent.isStopped = false;
        npcScript = _npc.GetComponent<EnemyAI>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        agent.SetDestination(player.position);
        base.Enter();
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(npc.transform.position, player.position);

        if (npcScript.IsInShadow())
        {
            // If enemy is back in the shadow, face the player
            FacePlayer();

            // Check if the player is within ranged attack range
            if (distanceToPlayer <= npcScript.RangedAttackRange())
            {
                npcScript.ChangeCurrentState(new RangeAttackState(npc, agent, anim, player));
            }
            else
            {
                Debug.Log("Enemy is taunting.");
                // npcScript.ChangeCurrentState(new TauntState(npc, agent, anim, player)); // Uncomment when TauntState is implemented
            }
        }
        else
        {
            // Retreat to shadow if the enemy is outside shadow while pursuing
            RetreatToShadow();
        }
    }

    private void RetreatToShadow()
    {
        // Calculate direction opposite to the player to move back into shadow
        Vector3 directionAwayFromPlayer = npc.transform.position - player.position;
        Vector3 retreatPosition = npc.transform.position + directionAwayFromPlayer.normalized * retreatDistance;

        // Set destination for retreat
        agent.SetDestination(retreatPosition);
        anim.SetTrigger("isRunning");

        // Check if back in shadow and immediately face the player
        if (npcScript.IsInShadow())
        {
            agent.ResetPath(); // Stop moving once back in shadow
            FacePlayer();
        }
    }

    private void FacePlayer()
    {
        // Rotate to face the player
        Vector3 directionToPlayer = (player.position - npc.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        agent.speed = 3.5f;
        base.Exit();
    }
}
