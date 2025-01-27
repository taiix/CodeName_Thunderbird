using UnityEngine;

public class NPC_Dialogue : Interactable
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
                }
                else
                {
                    Debug.LogError($"Dialogue {currentDialogueIndex + 1} is marked as having a quest, but no quest is assigned.");
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
            Debug.Log($"Advancing to dialogue {currentDialogueIndex}.");
        }
        else
        {
            Debug.Log("All dialogues completed.");
        }
    }
}

[System.Serializable]
public struct Dialogue
{
    public bool dialogueCompleted;
    public bool hasQuest;
    public BaseSO_Properties currentDialogueQuest;

    [TextArea(3, 10)]
    public string[] dialogue;
}
