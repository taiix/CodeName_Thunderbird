using UnityEngine;

[CreateAssetMenu(fileName = "New Collecting Quest", menuName = "Quest/Collecting Quest")]
public class CollectingQuest_SO : BaseSO_Properties
{
    public ItemsNeeded[] items;

    public bool _isCompleted;

    public override bool isCompleted => _isCompleted;

    public override void MarkAsCompleted()
    {
        _isCompleted = true;
        Debug.Log($"RepairQuest '{questName}' marked as completed!");
    }

    [System.Serializable]
    public struct ItemsNeeded {
        public Item requiredItem;
        public int requiredAmount;
    }
}
