using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class QuestUI : MonoBehaviour
{
    public static UnityEvent<string, string> OnQuestInfoChanged = new();

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;

    private void OnEnable()
    {
        OnQuestInfoChanged.AddListener(UpdateInfo);
    }
    private void OnDisable()
    {
        OnQuestInfoChanged.RemoveListener(UpdateInfo);
    }

    void UpdateInfo(string qName, string qDescription)
    {
        questName.text = qName;
        questDescription.text = qDescription;
    }
}
