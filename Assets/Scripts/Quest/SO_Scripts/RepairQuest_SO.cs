using UnityEngine;

[CreateAssetMenu(fileName = "New Reaching Quest", menuName = "Quest/Repair Quest")]
public class RepairQuest : BaseSO_Properties
{
    public bool _isCompleted;

    public override bool isCompleted => _isCompleted;

    public override void MarkAsCompleted()
    {
        _isCompleted = true;
        Debug.Log($"RepairQuest '{questName}' marked as completed!");
    }
}
