using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager_v2 : MonoBehaviour
{
    public static UnityEvent<string, string> OnQuestInfoChanged = new();

    [SerializeField] private List<BaseSO_Properties> allQuests;

    [SerializeField] private Queue<BaseSO_Properties> activeQuests = new();  //HIDE FROM INSPECTOR LATER

    [SerializeField] private BaseSO_Properties currentQuest;


    private void Start()
    {
        InitQuests();
        SortTheQuests();
    }

    private void InitQuests()
    {
        foreach (var q in activeQuests)
        {
            if (q is DestinationQuest)
            {
                q.Init();
            }
        }
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
        UpdateInfo();
    }

    private void Update()
    {
        if (currentQuest is DestinationQuest) { 
            currentQuest.CheckProgress();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            currentQuest = GetNewQuest();
            UpdateInfo();
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

    private void UpdateInfo()
    {
        if (currentQuest != null)
            OnQuestInfoChanged?.Invoke(currentQuest.questName, currentQuest.questDescription);
    }
}
