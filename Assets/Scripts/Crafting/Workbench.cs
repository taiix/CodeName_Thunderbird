using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField]UpgradeSystem upgradeSystem;
    [SerializeField] List<GameObject> planePartsUI = new List<GameObject>();
    [SerializeField] private List<PlanePart> planeParts = new List<PlanePart>();
    private int currentPart = 0;

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
        if(planeParts.Count > 0)
        {
            SelectCurrentPart(); 
        }
        upgradeSystem = GetComponent<UpgradeSystem>();
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
        planePartsUI[currentPart].SetActive(false);

        currentPart = (currentPart + 1) % planePartsUI.Count;

        ShowCurrentPartUI();
        SelectCurrentPart();
    }

    private void ShowPreviousPart()
    {
        planePartsUI[currentPart].SetActive(false);

        currentPart = (currentPart - 1 + planePartsUI.Count) % planePartsUI.Count;

        ShowCurrentPartUI();
        SelectCurrentPart();
    }

    private void ShowCurrentPartUI()
    {
        if (planePartsUI.Count == 0)
            return;

        planePartsUI[currentPart].SetActive(true);
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

    private void SelectCurrentPart()
    {
        if (planeParts.Count == 0 || currentPart > planeParts.Count) return;

        PlanePart selectedPart = planeParts[currentPart];
        Debug.Log("Selected Part: " + selectedPart.partName);

        // Pass the selected part to the UpgradeSystem
        upgradeSystem.SelectPlanePart(selectedPart);
    }
}

