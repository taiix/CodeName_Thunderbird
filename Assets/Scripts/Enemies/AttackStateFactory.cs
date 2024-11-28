using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackStateFactory 
{
    private static readonly Dictionary<EnemyType, System.Func<GameObject, NavMeshAgent, Animator, Transform, State>> stateMap =
       new()
       {
            { EnemyType.Ranged, (npc, agent, anim, player) => new RangeAttackState(npc, agent, anim, player) },
            { EnemyType.Melee, (npc, agent, anim, player) => new MeleeAttackState(npc, agent, anim, player) }
       };

    public static State GetAttackState(EnemyType enemyType, GameObject npc, NavMeshAgent agent, Animator anim, Transform player)
    {
        if (stateMap.TryGetValue(enemyType, out var createState))
        {
            return createState(npc, agent, anim, player);
        }

        throw new System.ArgumentException($"No attack state for enemy type: {enemyType}");
    }
}
