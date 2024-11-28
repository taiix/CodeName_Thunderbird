using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static UnityAction<Dialogue> sendDialogue;
    public static UnityAction OnDialogueStarted;
    public static UnityAction OnDialogueEnded;

    private Queue<string> sentences = new();
    public DialogueState dialogueState;
    private bool isTyping;

    [SerializeField] private GameObject dialogCanvas;
    [SerializeField] private GameObject[] otherCanvases;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI dialogueTextBox;

    private void Awake()
    {
        sendDialogue += StartDialogue;
    }

    private void OnDisable()
    {
        sendDialogue -= StartDialogue;
    }

    private void Update()
    {
        DialogSettings();

        if (Input.GetKeyDown(KeyCode.Tab) && !isTyping)
        {
            NextSentence();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueState = DialogueState.StartDialogue;
        OnDialogueStarted?.Invoke();
        ToggleCanvases(true);

        sentences.Clear();
        dialogueTextBox.text = "";
        npcNameText.text = dialogue.npcName;

        foreach (var sentence in dialogue.dialogue)
        {
            sentences.Enqueue(sentence);
        }

        NextSentence();
    }

    private void NextSentence()
    {
        if (sentences.Count > 0)
        {
            dialogueState = DialogueState.Talking;
            StartCoroutine(TypingDialogue(sentences.Dequeue()));
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueState = DialogueState.EndDialogue;

        ToggleCanvases(false);
        ClearDialogueUI();

        OnDialogueEnded?.Invoke();
        Debug.Log("Dialogue has ended.");
    }

    private IEnumerator TypingDialogue(string sentence)
    {
        isTyping = true;
        dialogueTextBox.text = "";

        foreach (char c in sentence)
        {
            dialogueTextBox.text += c;
            yield return new WaitForSeconds(0.04f);
        }

        isTyping = false;
    }

    private void ToggleCanvases(bool dialogueActive)
    {
        dialogCanvas.SetActive(dialogueActive);
        foreach (var canvas in otherCanvases)
        {
            canvas.SetActive(!dialogueActive);
        }
    }

    private void ClearDialogueUI()
    {
        dialogueTextBox.text = "";
        npcNameText.text = "";
    }

    private void DialogSettings()
    {
        switch (dialogueState)
        {
            case DialogueState.StartDialogue:
                Time.timeScale = 0;
                break;
            case DialogueState.Talking:
                Debug.Log("Talking dialogue");
                break;
            case DialogueState.EndDialogue:
                Time.timeScale = 1;
                break;
        }
    }
}

public enum DialogueState
{
    None,
    StartDialogue,
    Talking,
    EndDialogue
}
