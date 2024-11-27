using UnityEngine;

public abstract class QuestTask : MonoBehaviour
{
    public abstract void EvaluateTast();

    public virtual void CompleteTask()
    {
        QuestManager.OnTaskCompleted?.Invoke();
        enabled = false;
    }
}
