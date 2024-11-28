using UnityEngine;

public class NPC_Dialogue : Interactable
{    public string npcName;

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
        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
        TriggerDialogue();
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }

    private void Update()
    {
        dialogues[currentDialogueIndex].dialogueCompleted = dialogues[currentDialogueIndex].currentDialogueQuest.isCompleted;
        if (currentDialogueIndex < dialogues.Length - 1 && 
            dialogues[currentDialogueIndex].dialogueCompleted) currentDialogueIndex++;
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
