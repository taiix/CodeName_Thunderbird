using UnityEngine;
using UnityEngine.Events;

public class QuestManager_v2 : MonoBehaviour
{
    public static UnityEvent<BaseSO_Properties> OnQuestActivated = new();
    public static UnityEvent<BaseSO_Properties> OnQuestCompleted = new();
    public static UnityEvent<BaseSO_Properties> OnQuestSent = new();

    private void OnEnable() { OnQuestActivated.AddListener(ReceiveQuest); }

    private void OnDisable() { OnQuestActivated.RemoveListener(ReceiveQuest); }

    /// <summary>
    /// Sends the quest from NPC to Player
    /// </summary>
    /// <param name="q"></param>
    void ReceiveQuest(BaseSO_Properties q)
    {
        if (!q.isCompleted)
        {
            OnQuestSent?.Invoke(q);
        }
    }
}
