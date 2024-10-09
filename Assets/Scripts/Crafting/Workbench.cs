using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class Workbench : Interactable
{

    public GameObject backgroundVideo;
    public GameObject testingText;
    public GameObject crosshair;

    private Coroutine showTextCoroutine;

    [SerializeField] CinemachineVirtualCamera workbenchCamera;

    [SerializeField] Button exitButton;

    [SerializeField] Button upArrowButton;
    [SerializeField] Button downArrowButton;

    [SerializeField] List<GameObject> planeParts = new List<GameObject>();
    int currentPart = 0;

    bool isInteracting = false;

    private void OnEnable()
    {
        exitButton.onClick.AddListener(DisableInteraction);
        upArrowButton.onClick.AddListener(ShowNextPart);
        downArrowButton.onClick.AddListener(ShowPreviousPart);
    }

    private void OnDisable()
    {
        exitButton.onClick.RemoveAllListeners();
        upArrowButton.onClick.RemoveAllListeners();
        downArrowButton.onClick.RemoveAllListeners();
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
        crosshair.SetActive(false);
        StartCoroutine(ShowTextTemporarily());
        InteractionHandler.Instance.HideInteractionUI();
        isInteracting = true;
        Debug.Log("Interacting with Workbench");
        workbenchCamera.gameObject.SetActive(true);

        ShowCurrentPartUI();
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;
    }

    // Start is called before the first frame update
    void Start()
    {
        backgroundVideo.SetActive(true);
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



    private void ShowNextPart()
    {
        planeParts[currentPart].SetActive(false);

        currentPart = (currentPart + 1) % planeParts.Count;

        ShowCurrentPartUI();
    }

    private void ShowPreviousPart()
    {
        planeParts[currentPart].SetActive(false);

        currentPart = (currentPart - 1 + planeParts.Count) % planeParts.Count;

        ShowCurrentPartUI();
    }

    private void ShowCurrentPartUI()
    {
        if (planeParts.Count == 0)
            return;

        planeParts[currentPart].SetActive(true);
    }
    private IEnumerator ShowTextTemporarily()
    {

        // Wait for 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        backgroundVideo.SetActive(false);
    }

    void DisableInteraction()
    {

        isInteracting = false;
        workbenchCamera.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.EnablePlayerControls();
        crosshair.SetActive(true);
        backgroundVideo.SetActive(true);
    }
}

