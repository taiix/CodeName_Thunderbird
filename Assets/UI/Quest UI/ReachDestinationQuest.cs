using UnityEngine;

public class ReachDestinationQuest : QuestTask
{
    public float completionDistance;
    public Transform playerDestination;

    private void Update()
    {
        EvaluateTast();
    }

    public override void EvaluateTast()
    {
        Vector3 desiredDestination = this.transform.position;

        float distance = (desiredDestination - playerDestination.position).magnitude;
        if (distance <= completionDistance)
        {
            CompleteTask();
            return;
        }
    }
}
