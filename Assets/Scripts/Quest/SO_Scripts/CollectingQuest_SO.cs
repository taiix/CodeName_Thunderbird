using UnityEngine;

[CreateAssetMenu(fileName = "New Collecting Quest", menuName = "Quest/Collecting Quest")]
public class CollectingQuest_SO : BaseSO_Properties
{
    public int currentAmount;
    public int requiredAmount;

    public override void CheckProgress()
    {
        if (currentAmount >= requiredAmount)
            this.isCompleted = true;
    }

    public override void Init()
    { }
}
