using System;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;

    private void Start()
    {
        QuestManager_v2.OnQuestInfoChanged.AddListener(UpdateInfo);
    }
    private void OnDisable()
    {
        QuestManager_v2.OnQuestInfoChanged.RemoveListener(UpdateInfo);
    }

    void UpdateInfo(string qName, string qDescription)
    {
        questName.text = qName;
        questDescription.text = qDescription;
    }
}
