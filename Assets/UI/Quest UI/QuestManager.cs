using TMPro;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static UnityAction OnTaskCompleted { get; private set; }

    [SerializeField] private Quest_ScriptableObject[] questInfo;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questObjective;

    [SerializeField] private int currentQuestIndex;
    [SerializeField] private int currentObjectiveIndex;

    [SerializeField] private Quest[] currentQuestObjectives;

    private void Start()
    {
        if (questInfo.Length == 0)
        {
            Debug.LogError("QuestInfo is not assigned.");
            return;
        }
        else if (questName == null || questObjective == null)
        {
            Debug.LogError("questName or questObjective is not assigned.");
            return;
        }

        foreach (var quest in questInfo)
        {
            quest.ResetQuests();
        }

        UpdateQuestInfo();
        UpdateQuestObjectiveInfo();
        InitTasks();
    }

    void InitTasks()
    {
        if (currentQuestObjectives.Length > 0)
        {
            currentQuestObjectives[0].taskStatus = QuestStatus.Active; // Start the first quest
        }
    }

    private void OnEnable() { OnTaskCompleted += CompleteCurrentObjective; }

    private void OnDisable() { OnTaskCompleted -= CompleteCurrentObjective; }
    
    void UpdateQuestInfo()
    {
        if (currentQuestIndex >= questInfo.Length) return;

        Quest_ScriptableObject currentQuestInfo = questInfo[currentQuestIndex];

        questName.text = currentQuestInfo.questName;
        currentQuestObjectives = currentQuestInfo.quests;
        Debug.Log("Update Quest Info");
    }

    void UpdateQuestObjectiveInfo()
    {
        if (currentObjectiveIndex < currentQuestObjectives.Length)
            questObjective.text = currentQuestObjectives[currentObjectiveIndex].questObjective;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) CompleteCurrentObjective();
    }

    private void CompleteCurrentObjective()
    {
        if (currentObjectiveIndex < currentQuestObjectives.Length && currentQuestIndex < questInfo.Length)
        {
            currentQuestObjectives[currentObjectiveIndex].hasFinished = true;
            currentQuestObjectives[currentObjectiveIndex].taskStatus = QuestStatus.Finished;

            currentObjectiveIndex++;

            if (currentObjectiveIndex >= currentQuestObjectives.Length)
            {
                CompleteCurrentQuest();
                currentQuestObjectives[currentObjectiveIndex].taskStatus = QuestStatus.Active;
            }
            else
            {
                currentQuestObjectives[currentObjectiveIndex].taskStatus = QuestStatus.Active;
                UpdateQuestObjectiveInfo(); }
        }
    }

    private void CompleteCurrentQuest()
    {
        if (currentQuestIndex < questInfo.Length)
        {
            questInfo[currentQuestIndex].questCompleted = true;
            questInfo[currentQuestIndex].questStatus = QuestStatus.Finished;
            currentQuestIndex++;
            currentObjectiveIndex = 0;

            if (currentQuestIndex < questInfo.Length)
            {
                UpdateQuestInfo();
                UpdateQuestObjectiveInfo();
            }
            else
            {
                questName.text = "All quests completed!";
                questObjective.text = "No more missions available.";
            }
        }
    }
}
