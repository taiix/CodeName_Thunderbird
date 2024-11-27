using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests/Quest", order = 1)]
public class Quest_ScriptableObject : ScriptableObject
{
    public QuestStatus questStatus;

    public string questName;

    public bool questCompleted = false;

    public Quest[] quests;

    public void ResetQuests()
    {
        questCompleted = false;

        for (int i = 0; i < quests.Length; i++)
        {
            quests[i].hasFinished = false;
            quests[i].taskStatus = QuestStatus.NotStarted;
        }
    }
}

[System.Serializable]
public struct Quest
{
    public QuestStatus taskStatus;

    [TextArea(3, 10)]
    public string questObjective;

    public bool hasFinished;
}
public enum QuestStatus { NotStarted, Active, Pending, Finished }

