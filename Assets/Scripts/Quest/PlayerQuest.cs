using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerQuest : MonoBehaviour
{
    public static UnityAction OnQuestCompleted;

    [SerializeField] private BaseSO_Properties activeQuest;
    [SerializeField] private Transform desiredLocation;

    [Header("Sound Effect")]
    [SerializeField] private AudioClip questCompletedFX;

    [Header("UI")]
    [SerializeField] private GameObject questCompletedUI;

    private void OnEnable()
    {
        QuestManager_v2.OnQuestSent.AddListener(ReceiveQuest);
        OnQuestCompleted += TrackRepairQuest;
    }
    private void OnDisable()
    {
        QuestManager_v2.OnQuestSent.RemoveListener(ReceiveQuest);
        OnQuestCompleted -= TrackRepairQuest;

    }

    private void Update()
    {
        if (activeQuest is DestinationQuest destinationQuest)
        {
            TrackDestinationQuest(destinationQuest);
        }
        if (activeQuest is CollectingQuest_SO collectQuest)
        {
            TrackCollectQuest(collectQuest);
        }
    }

    void ReceiveQuest(BaseSO_Properties q)
    {
        activeQuest = q;
        QuestUI.OnQuestInfoChanged?.Invoke(activeQuest.questName, activeQuest.questDescription);

        if (activeQuest is DestinationQuest destinationQuest)
        {
            GameObject go = GameObject.Find(destinationQuest.destinationPositionObjectName);
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

        float distance = Vector3.Distance(transform.position, desiredLocation.position);

        if (distance <= destinationQuest.distanceDifference)
        {
            Debug.Log($"Quest '{destinationQuest.questName}' completed! Destination reached.");
            CompleteQuest(activeQuest);
        }
    }

    void TrackCollectQuest(CollectingQuest_SO collectQuest)
    {
        InventorySystem inventory = InventorySystem.Instance;
        bool allItemsCollected = true;

        foreach (var item in collectQuest.items)
        {
            if (!inventory.HasRequiredItem(item.requiredItem, item.requiredAmount))
            {
                allItemsCollected = false;
                break;
            }
        }

        if (allItemsCollected)
        {
            CompleteQuest(activeQuest);
        }
        else
        {
            CollectingQuest_SO currentQ = activeQuest as CollectingQuest_SO;
            QuestUI.OnQuestInfoChanged?.Invoke(
                activeQuest.questName,
                $"{activeQuest.questDescription}. Required items: {currentQ.items[0].requiredAmount}"
            );
        }
    }

    void TrackRepairQuest()
    {
        if (activeQuest is RepairQuest repQ)
        {
            CompleteQuest(activeQuest);
        }
    }

    bool isAnimating;
    private void CompleteQuest(BaseSO_Properties quest)
    {
        if (quest == null || quest.isCompleted) return;
        if (!isAnimating)
            StartCoroutine(WaitUI());

        quest.MarkAsCompleted();
        AudioManager.instance?.OneshotAudioFX(questCompletedFX, 5, false);
        QuestUI.OnQuestInfoChanged?.Invoke("No name", "The quest has been completed");
    }

    IEnumerator WaitUI()
    {
        isAnimating = true;
        questCompletedUI.SetActive(true);
        if (questCompletedUI.transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI t))
        {
            Color originalColor = t.color;
            t.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            while (t.color.a > 0)
            {
                float newAlpha = t.color.a - Time.deltaTime * 0.2f;
                t.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Max(0, newAlpha));
                yield return null;
            }
        }
            questCompletedUI.SetActive(false);

        yield return new WaitForSeconds(1f);
        isAnimating = false;
    }
}
