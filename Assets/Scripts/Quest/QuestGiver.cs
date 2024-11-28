using System.Collections.Generic;
using UnityEngine;

public class QuestGiver : MonoBehaviour
{
    [SerializeField] private List<BaseSO_Properties> allQuests;

    [SerializeField] private BaseSO_Properties currentQuest;

    private Queue<BaseSO_Properties> activeQuests = new();  //HIDE FROM INSPECTOR LATER

    private void Start()
    {
        SortTheQuests();
    }

    private void SortTheQuests()
    {
        allQuests.Sort((a, b) => a.id.CompareTo(b.id));

        foreach (var q in allQuests)
        {
            q.isCompleted = false;
            if (!q.isCompleted) activeQuests.Enqueue(q);
        }

        currentQuest = activeQuests.Dequeue();
    }

    /// <summary>
    /// MAKE IT TO GIVE THE QUEST UNDER SOME CONDITIONS
    /// </summary>
    private void Update()
    {
        if (currentQuest != null && currentQuest.isCompleted) currentQuest = GetNewQuest();
        if (currentQuest != null && Input.GetKeyDown(KeyCode.E))
        {
            QuestManager_v2.OnQuestActivated?.Invoke(currentQuest);
        }
    }

    private BaseSO_Properties GetNewQuest()
    {
        if (currentQuest != null && currentQuest.isCompleted)
        {
            if (activeQuests.Count > 0)
            {
                currentQuest = activeQuests.Dequeue();
            }
            else
            {
                Debug.Log("No more quests available");
                currentQuest = null;
            }
        }

        return currentQuest;
    }

    //private void UpdateInfo()
    //{
    //    if (currentQuest != null)
    //        OnQuestInfoChanged?.Invoke(currentQuest.questName, currentQuest.questDescription);
    //}
}
