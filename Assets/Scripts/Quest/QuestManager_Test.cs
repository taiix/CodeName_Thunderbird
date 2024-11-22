using System.Collections.Generic;
using UnityEngine;

public class QuestManager_Test : MonoBehaviour
{
    [SerializeField] private List<Quest_ScriptableObject> questsQueue;
    [SerializeField] private Quest[] questsTasks;

    int a = 0;

    private void Start()
    {
        GetQuestTasks();
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))GetQuestTasks();
    }

    void GetQuestTasks()
    {
        Quest_ScriptableObject currentQuest = questsQueue[0];
        if (currentQuest != null)
        {
            if (currentQuest.questStatus == QuestStatus.Finished || currentQuest.questCompleted) return;

            if (currentQuest.questStatus == QuestStatus.NotStarted)
            {
                currentQuest.questStatus = QuestStatus.Active;
                questsTasks = currentQuest.quests;
                for (int i = 0; i < questsTasks.Length; i++)
                {
                    if (!questsTasks[i].hasFinished)
                    {
                        QuestUI_Test.OnQuestUpdate?.Invoke(currentQuest.questName, currentQuest.quests[i].questObjective);
                        questsTasks[i].hasFinished = true;
                    }
                }
            }
        }
    }


}
