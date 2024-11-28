using UnityEngine;

[CreateAssetMenu(fileName = "New Collecting Quest", menuName = "Quest/Collecting Quest")]
public class CollectingQuest_SO : BaseSO_Properties
{
    public ItemsNeeded[] items;

    [System.Serializable]
    public struct ItemsNeeded {
        public Item requiredItem;
        public int requiredAmount;
    }
}
