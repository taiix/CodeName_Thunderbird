using UnityEngine;

public class NPC_Dialogue : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;

    private void Start()
    {
        //TODO: Now it sends the dioalogue at start. Make it to send it on interaction
        TriggerDialogue();
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
