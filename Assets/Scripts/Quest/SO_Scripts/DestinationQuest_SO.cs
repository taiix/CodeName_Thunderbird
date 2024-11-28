using UnityEngine;

[CreateAssetMenu(fileName = "New Reaching Quest", menuName = "Quest/Reaching Quest")]
public class DestinationQuest : BaseSO_Properties
{
    public float distanceDifference;
    public string destinationPositionObjectName; 
    public bool _isCompleted;

    public override bool isCompleted => _isCompleted;

    public override void MarkAsCompleted()
    {
        _isCompleted = true;
        Debug.Log($"DestinationQuest '{questName}' marked as completed!");
    }
}
