using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackStrategyFactory 
{
    private static readonly Dictionary<EnemyType, IAttackStrategy> strategyMap = new()
    {
        { EnemyType.Ranged, new RangeAttackStrategy() },
        { EnemyType.Melee, new MeleeAttackStrategy() }
    };

    public static IAttackStrategy GetStrategy(EnemyType enemyType)
    {
        return strategyMap.TryGetValue(enemyType, out var strategy) ? strategy : null;
    }
}
