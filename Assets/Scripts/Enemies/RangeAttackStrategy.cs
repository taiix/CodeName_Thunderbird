using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackStrategy : IAttackStrategy
{
    public void PerformAttack(EnemyAI enemyAI)
    {
        if (enemyAI.player == null) return;

        Vector3 throwDirection = (enemyAI.player.position - enemyAI.transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(enemyAI.player.position, enemyAI.transform.position);

        float adjustedForce = Mathf.Clamp(distanceToPlayer * 5f, 10f, 20f);

        GameObject thrownStone = Object.Instantiate(
            enemyAI.enemyData.weaponPrefab,
            enemyAI.transform.position + Vector3.up * 1.5f,
            Quaternion.identity
        );

        Projectile projectile = thrownStone.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Throw(throwDirection * adjustedForce, false, enemyAI.gameObject);
        }
        else
        {
            Debug.LogWarning("Thrown object does not have a Projectile component.");
        }
    }
}

