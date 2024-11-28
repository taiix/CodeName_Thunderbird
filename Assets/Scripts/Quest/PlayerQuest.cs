using System;
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

        if (activeQuest != null && Input.GetKeyDown(KeyCode.V))
        {
            activeQuest.isCompleted = true;
            activeQuest = null;
            QuestUI.OnQuestInfoChanged?.Invoke("", "There is no current quest");
        }
    }

    void ReceiveQuest(BaseSO_Properties q)
    {
        Debug.Log("received quest from QM");
        activeQuest = q;
        QuestUI.OnQuestInfoChanged?.Invoke(activeQuest.questName, activeQuest.questDescription);

        if (activeQuest is DestinationQuest)
        {
            DestinationQuest e = activeQuest as DestinationQuest;
            GameObject go = GameObject.Find(e.destinationPositionObjectName);
            if (go == null)
            {
                Debug.LogError("No destination found for destionation quest. " +
                "Check the quest's object name to find");
            }
            else desiredLocation = go.transform;
        }
    }

    void TrackDestinationQuest(DestinationQuest destinationQuest)
    {
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

    private void CompleteQuest()
    {
        //INFORM THAT HE QUEST HAS BEEN FINISHED
    }
}
