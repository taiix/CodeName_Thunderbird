using UnityEngine;

public class NPC_Dialogue : Interactable, ISavableData
{
    public string npcName;

    [SerializeField] private Dialogue[] dialogues;
    [SerializeField] private int currentDialogueIndex = 0;

    private void Start()
    {
        for (int i = 0; i < dialogues.Length; i++) { dialogues[i].dialogueCompleted = false; }
    }

    public override void OnFocus()
    {
        interactionText = $"Press 'F' to speak with {npcName}";
    }

    public override void OnInteract()
    {
        bool isTalking = DialogueManager.isTalking;
        if (isTalking)
        {
            Debug.Log("Already in conversation. Cannot start a new one.");
            return;
        }

        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
        TriggerDialogue();
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }

    private void Update()
    {
        CheckCompletetion();
    }

    void TriggerDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            if (!dialogues[currentDialogueIndex].hasQuest)
            {
                DialogueManager.sendDialogue?.Invoke(npcName, dialogues[currentDialogueIndex].dialogue, null);
            }
            else
            {
                if (dialogues[currentDialogueIndex].currentDialogueQuest != null)
                {
                    DialogueManager.sendDialogue?.Invoke(
                        npcName, dialogues[currentDialogueIndex].dialogue, dialogues[currentDialogueIndex].currentDialogueQuest);
                    dialogues[currentDialogueIndex].hasBeenTaken = true;
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            Debug.Log("No more dialogues available.");
        }
    }


    void CheckCompletetion()
    {
        if (currentDialogueIndex >= dialogues.Length) return;

        Dialogue currentDialogue = dialogues[currentDialogueIndex];
        BaseSO_Properties currentQuest = currentDialogue.currentDialogueQuest;

        if (currentDialogue.dialogueCompleted) return;

        if (currentQuest is DestinationQuest destQ)
        {
            if (destQ.isCompleted)
            {
                CompleteCurrentDialogue();
            }
        }
        else if (currentQuest is CollectingQuest_SO collQ)
        {
            if (collQ.isCompleted)
            {
                CompleteCurrentDialogue();
            }
        }
        else if (currentQuest is RepairQuest repQ)
        {
            if (repQ.isCompleted)
            {
                CompleteCurrentDialogue();
            }
        }
        else
        {
            Debug.LogWarning($"Dialogue {currentDialogueIndex} has no valid quest associated.");
        }
    }

    void CompleteCurrentDialogue()
    {
        dialogues[currentDialogueIndex].dialogueCompleted = true;
        Debug.Log($"Dialogue {currentDialogueIndex} completed.");

        if (currentDialogueIndex < dialogues.Length - 1)
        {
            currentDialogueIndex++;
        }
        else
        {
            Debug.Log("All dialogues completed.");
        }
    }

    public string ToJson()
    {
        int saveIndex = currentDialogueIndex;
        bool hasTakenQuest = dialogues[currentDialogueIndex].hasBeenTaken;
        bool isQuestCompleted = dialogues[currentDialogueIndex].currentDialogueQuest != null
                                && dialogues[currentDialogueIndex].currentDialogueQuest.isCompleted;

        for (int i = 0; i < dialogues.Length; i++)
        {
            Dialogue dialogue = dialogues[i];

            if (dialogue.hasQuest && dialogue.currentDialogueQuest != null)
            {
                if (!dialogue.currentDialogueQuest.isCompleted)
                {
                    saveIndex = i;
                    hasTakenQuest = dialogue.hasBeenTaken;
                    isQuestCompleted = dialogue.currentDialogueQuest.isCompleted;
                    break;
                }
            }
        }

        NPCDialogueData data = new NPCDialogueData(saveIndex, hasTakenQuest, isQuestCompleted);
        return JsonUtility.ToJson(data);
    }

    public void FromJson(string json)
    {
        NPCDialogueData data = JsonUtility.FromJson<NPCDialogueData>(json);
        currentDialogueIndex = data.dialogueIndex;

        if (currentDialogueIndex < dialogues.Length)
        {
            Dialogue currentDialogue = dialogues[currentDialogueIndex];

            currentDialogue.hasBeenTaken = data.questHasBeenTaken;

            if (currentDialogue.currentDialogueQuest != null)
            {
                if (data.isCompleted)
                {
                    currentDialogue.currentDialogueQuest.MarkAsCompleted();
                }
            }

            if (currentDialogue.hasQuest && currentDialogue.currentDialogueQuest != null)
            {
                if (currentDialogue.hasBeenTaken && !currentDialogue.currentDialogueQuest.isCompleted)
                {
                    AssignQuestToPlayer(currentDialogue.currentDialogueQuest);
                }
            }
        }
    }

    void AssignQuestToPlayer(BaseSO_Properties quest)
    {
        if (quest is DestinationQuest destQ && !destQ.isCompleted)
        {
            QuestManager_v2.OnQuestActivated?.Invoke(quest);
        }
        else if (quest is CollectingQuest_SO collQ && !collQ.isCompleted)
        {
            QuestManager_v2.OnQuestActivated?.Invoke(quest);
        }
        else if (quest is RepairQuest repQ && !repQ.isCompleted)
        {
            QuestManager_v2.OnQuestActivated?.Invoke(quest);
        }
        else
        {
            Debug.Log($"Quest '{quest.name}' is already completed or is not valid.");
        }
    }
}

[System.Serializable]
public struct Dialogue
{
    public bool dialogueCompleted;
    public bool hasQuest;
    public bool hasBeenTaken;
    public BaseSO_Properties currentDialogueQuest;

    [TextArea(3, 10)]
    public string[] dialogue;
}
