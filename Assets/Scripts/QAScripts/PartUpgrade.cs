using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewPartUpgrade", menuName = "ScriptableObjects/Part Upgrade", order = 1)]
public class PartUpgrade : ScriptableObject
{
    public List<RequiredItem> requiredItemsList = new List<RequiredItem>();
    public int upgradePower;
    public int healthUpgrade;
    public int damageReduction;
}
