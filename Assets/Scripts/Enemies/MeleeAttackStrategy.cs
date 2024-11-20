using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackStrategy : IAttackStrategy
{

    private bool isPlayerInRange = false;

    public void PerformAttack(EnemyAI enemyAI)
    {
            HitPlayer(enemyAI);
    }

    public void HitPlayer(EnemyAI enemyAI)
    {
        if (!isPlayerInRange) return;

        PlayerHealth.OnPlayerDamaged?.Invoke(enemyAI.enemyData.attackDamage);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Plauer is in range");
            isPlayerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }


}
