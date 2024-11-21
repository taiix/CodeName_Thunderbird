using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackStrategy : IAttackStrategy
{
    private bool isPlayerInRange = false;
    private const float heightDifferenceThreshold = 2f; 
    private Rigidbody playerRigidbody;

    public void PerformAttack(EnemyAI enemyAI)
    {
        if (IsPlayerAbove(enemyAI))
        {
            PerformUpAttack(enemyAI);
        }
        else
        {
            HitPlayer(enemyAI);
        }
    }

    public void HitPlayer(EnemyAI enemyAI)
    {
        if (!isPlayerInRange) return;

        Debug.Log("Regular melee attack!");
        PlayerHealth.OnPlayerDamaged?.Invoke(enemyAI.enemyData.attackDamage);
    }

    private bool IsPlayerAbove(EnemyAI enemyAI)
    {
        if (playerRigidbody == null) return false;

        float heightDifference = playerRigidbody.transform.position.y - enemyAI.transform.position.y;
        return heightDifference > heightDifferenceThreshold;
    }

    private void PerformUpAttack(EnemyAI enemyAI)
    {
        Debug.Log("Up attack triggered!");
        PlayerHealth.OnPlayerDamaged?.Invoke(enemyAI.enemyData.attackDamage * 2);

        if (playerRigidbody != null)
        {
            enemyAI.StartCoroutine(ApplySmoothForce(enemyAI));
        }
    }

    private IEnumerator ApplySmoothForce(EnemyAI enemyAI)
    {
        float pushDuration = 0.5f; 
        float elapsedTime = 0f;

        Vector3 upwardForce = Vector3.up * 100f; 
        Vector3 leftwardForce = -enemyAI.transform.right * 500f; 
        Vector3 totalForce = upwardForce + leftwardForce;

        while (elapsedTime < pushDuration)
        {
            if(playerRigidbody == null) break;
            float forceFactor = Time.deltaTime / pushDuration; 
            playerRigidbody.AddForce(totalForce * forceFactor, ForceMode.Impulse);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerRigidbody = other.GetComponent<Rigidbody>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerRigidbody = null;
        }
    }
}
