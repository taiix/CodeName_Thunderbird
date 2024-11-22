using System;
using TMPro;
using UnityEngine;

public class QuestUI_Test : MonoBehaviour
{
    public static Action<string, string> OnQuestUpdate;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questTask;

    private void Start()
    {
        OnQuestUpdate += QuestUpdated;
    }

    private void OnDestroy()
    {
        OnQuestUpdate -= QuestUpdated;
    }

    void QuestUpdated(string qName, string qTask)
    {
        if (questName != null && questTask != null)
        {
            questName.text = qName;
            questTask.text = qTask;
        }
        else
        {
            Debug.LogWarning("Quest UI elements are not assigned.");
        }
    }
}
