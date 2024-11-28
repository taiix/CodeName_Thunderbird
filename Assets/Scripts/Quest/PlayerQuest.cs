using UnityEngine;

public class PlayerQuest : MonoBehaviour
{
    [SerializeField] private BaseSO_Properties activeQuest;
    [SerializeField] private Transform desiredLocation;

    private void OnEnable() { QuestManager_v2.OnQuestSent.AddListener(ReceiveQuest); }
    private void OnDisable() { QuestManager_v2.OnQuestSent.RemoveListener(ReceiveQuest); }

    private void Update()
    {
        if (activeQuest is DestinationQuest destinationQuest)
        {
            TrackDestinationQuest(destinationQuest);
        }
        else if (activeQuest is CollectingQuest_SO collectQuest)
        {
            TrackCollectQuest(collectQuest);
        }
    }

    void ReceiveQuest(BaseSO_Properties q)
    {
        activeQuest = q;
        QuestUI.OnQuestInfoChanged?.Invoke(activeQuest.questName, activeQuest.questDescription);
    }

    void TrackDestinationQuest(DestinationQuest destinationQuest)
    {
        GameObject go = GameObject.Find(destinationQuest.destinationPositionObjectName);

        if (go == null)
        {
            Debug.LogError("No destination found for destionation quest. " +
            "Check the quest's object name to find");
        }
        else desiredLocation = go.transform;

        if (desiredLocation == null)
        {
            Debug.LogWarning("Destination Transform is not set. Skipping progress tracking.");
            return;
        }

        Transform playerTransform = transform; // Player's position
        float distance = Vector3.Distance(playerTransform.position, desiredLocation.position);

        if (distance <= destinationQuest.distanceDifference)
        {
            Debug.Log($"Quest '{destinationQuest.questName}' completed! Destination reached.");
            CompleteQuest();
        }
    }

    void TrackCollectQuest(CollectingQuest_SO collectQuest)
    {
        activeQuest = collectQuest;
        
        InventorySystem inventory = InventorySystem.Instance;

        for (int i = 0; i < collectQuest.items.Length - 1; i++)
        {
            QuestUI.OnQuestInfoChanged?.Invoke(
                activeQuest.questName, activeQuest.questDescription + 
                $"{collectQuest.items[i].requiredAmount}" + "\n");

            if (!inventory.HasRequiredItem(collectQuest.items[i].requiredItem, collectQuest.items[i].requiredAmount))
                return;
        }
        CompleteQuest();
    }

    private void CompleteQuest()
    {
        QuestUI.OnQuestInfoChanged?.Invoke("No name", "The quest has been completed");
        activeQuest.isCompleted = true;
        //INFORM THAT HE QUEST HAS BEEN FINISHED
    }
}
