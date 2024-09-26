using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Workbench : Interactable
{
    public GameObject testingText;

    private Coroutine showTextCoroutine;
    public override void OnFocus()
    {

    }

    public override void OnInteract()
    {
        Debug.Log("Interacting with Workbench");

        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
        }

        showTextCoroutine = StartCoroutine(ShowTextTemporarily());
    }

    public override void OnLoseFocus()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        testingText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ShowTextTemporarily()
    {
        // Enable the text
        testingText.SetActive(true);

        // Wait for 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Disable the text
        testingText.SetActive(false);
    }
}
