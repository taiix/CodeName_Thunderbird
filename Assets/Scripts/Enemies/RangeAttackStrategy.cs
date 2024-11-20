using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttackStrategy : IAttackStrategy
{
    public void PerformAttack(EnemyAI enemyAI)
    {
        if (enemyAI.player == null) return;

        Vector3 throwDirection = (enemyAI.player.position - enemyAI.transform.position).normalized;
        GameObject thrownStone = Object.Instantiate(
            enemyAI.enemyData.weaponPrefab,
            enemyAI.transform.position + Vector3.up * 1.5f,
            Quaternion.identity
        );

        Rigidbody stoneRb = thrownStone.GetComponent<Rigidbody>();
        if (stoneRb != null)
        {
            stoneRb.AddForce(throwDirection * 20, ForceMode.VelocityChange);
        }
    }
}

