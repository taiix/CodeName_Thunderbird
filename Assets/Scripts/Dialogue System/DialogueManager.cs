using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static UnityAction<Dialogue> sendDialogue;
    private Queue<string> sentences = new();

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
        if (Input.GetKeyDown(KeyCode.Tab)) NextSentence();
    }

    public void StartDialogue(Dialogue dialoge)
    {
        sentences.Clear();

        string[] s = dialoge.dialogue;
        if(s.Length <= 0) return;
        npcNameText.text = dialoge.npcName;
        foreach (string sentence in s)
        {
            sentences.Enqueue(sentence);
        }
        Debug.Log($"Starting dialogue with {dialoge.npcName} + {sentences.Count}");
    }

    private void NextSentence()
    {
        if (sentences.Count > 0)
        {
            string sentence = sentences.Dequeue();
            StartCoroutine(TypingDialogue(sentence));
        }
    }

    private IEnumerator TypingDialogue(string sentence)
    {
        dialogueTextBox.text = "";
        foreach (char c in sentence.ToCharArray())
        {
            dialogueTextBox.text += c;
            yield return new WaitForSeconds(0.04f);
        }
    }
}
