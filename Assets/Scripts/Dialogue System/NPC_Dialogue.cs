using UnityEngine;

public class NPC_Dialogue : Interactable
{
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private bool hasQuests;

    public override void OnFocus()
    {
        interactionText = $"Press 'F' to speak with {dialogue.npcName}";
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


    void TriggerDialogue()
    {
        DialogueManager.sendDialogue?.Invoke(dialogue);
    }
}

[System.Serializable]
public struct Dialogue
{
    public string npcName;

    [TextArea(3, 10)]
    public string[] dialogue;
}
