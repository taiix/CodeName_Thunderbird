using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Workbench : Interactable
{
    public GameObject testingText;

    private Coroutine showTextCoroutine;

    [SerializeField] CinemachineVirtualCamera workbenchCamera;

    [SerializeField] Button exitButton;

    bool isInteracting = false;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(DisableInteraction);
    }

    private void OnDisable()
    {
        exitButton.onClick.RemoveAllListeners();
    }

    public override void OnFocus()
    {
        if (!isInteracting)
        {

            interactionText = "Press 'F' to interact with Workbench.";
        }
    }

    public override void OnInteract()
    {
        
        InteractionHandler.Instance.HideInteractionUI();
        isInteracting = true;
        Debug.Log("Interacting with Workbench");
        workbenchCamera.gameObject.SetActive(true);
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }

    // Start is called before the first frame update
    void Start()
    {
        testingText.SetActive(false);
        workbenchCamera.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isInteracting)
        {
            Cursor.lockState = CursorLockMode.None;
            GameManager.Instance.DisablePlayerControls();
            interactionText = string.Empty;
        }
    }

    private IEnumerator ShowTextTemporarily()
    {
        // Enable the text
        testingText.SetActive(true);

        // Wait for 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Disable the text
        interactionText = string.Empty;
    }

    void DisableInteraction()
    {
        isInteracting = false;
        workbenchCamera.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.EnablePlayerControls();
    }
}
