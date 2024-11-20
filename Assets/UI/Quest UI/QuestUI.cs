using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuestUI : MonoBehaviour
{
    public static UnityAction OnQuestCompleted;

    [SerializeField] private Quest_ScriptableObject[] questInfo;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questObjective;

    [SerializeField] private int currentQuestIndex;
    [SerializeField] private int currentObjectiveIndex;


    [SerializeField] private Quest[] currentQuestObjectives;

    private void Awake()
    {
        if (questInfo.Length < 0)
        {
            Debug.LogError("QuestInfo is not assigned.");
            return;
        }

        foreach (var quest in questInfo)
        {
            quest.ResetQuests();
        }

        UpdateQuestInfo();
        UpdateQuestObjectiveInfo();
    }

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
        if (currentObjectiveIndex < currentQuestObjectives.Length)
        {
            currentQuestObjectives[currentObjectiveIndex].hasFinished = true;

            currentObjectiveIndex++;
            if (currentObjectiveIndex >= currentQuestObjectives.Length)
            {
                CompleteCurrentQuest();
            }
            else UpdateQuestObjectiveInfo();
        }
    }

    private void CompleteCurrentQuest()
    {

        if (currentQuestIndex < questInfo.Length)
        {
            questInfo[currentQuestIndex].questCompleted = true;
            currentQuestIndex++;
            currentObjectiveIndex = 0;

            UpdateQuestInfo();
            UpdateQuestObjectiveInfo();
        }
        else questObjective.text = "No more missions";
    }
}
