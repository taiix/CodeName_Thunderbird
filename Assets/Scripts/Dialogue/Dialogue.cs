using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public string title;
    [TextArea(3, 10)]
    public string[] dialogueLines;
}
