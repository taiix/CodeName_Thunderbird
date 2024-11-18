using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Ranged,
    Melee
}

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "ScriptableObjects/EnemyData", order = 2)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public EnemyType enemyType;
    public int health;
    public float attackSpeed;
    public float spottingRange;
    public float attackRange;
    public float optimalAttackDistance;
    public float timeInSun;
    public float deadTimer;
    public GameObject weaponPrefab;
}
