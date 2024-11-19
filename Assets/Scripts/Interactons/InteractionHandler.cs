using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    public Vector3 interactionRaypoint = new Vector3(0.5f, 0.5f, 0f);
    public float interactionDistance = default;

    public LayerMask layerMask;

    public Interactable currentInteractable;

    public CinemachineVirtualCamera virtualCamera;
    private Camera mainCamera;
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private PlayerInput playerInput;

    [SerializeField] public GameObject interactionUI;
    [SerializeField] private GameObject treeUI;
    public TreeInteractable treeInteractable;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        inputAsset = this.GetComponentInParent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");
        playerInput = this.GetComponentInParent<PlayerInput>();

        mainCamera = Camera.main;
    }


    private void OnEnable()
    {
        player.FindAction("Interaction").started += Interact;
    }

    //Unsubscirbe
    private void OnDisable()
    {
        player.FindAction("Interaction").started -= Interact;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleInteractionCheck();
    }

    void HandleInteractionCheck()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera is not set!");
            return;
        }

        var ray = mainCamera.ViewportPointToRay(interactionRaypoint);
        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.green);


        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, layerMask))
        {
            if (hit.collider.TryGetComponent(out Interactable hitInteractable) && hitInteractable != currentInteractable)
            {
                // If the current interactable is different, handle focus change
                if (currentInteractable != hitInteractable)
                {
                    // Lose focus on the previous interactable
                    if (currentInteractable != null)
                    {
                        currentInteractable.OnLoseFocus();
                        interactionUI.SetActive(false);
                    }

                    // Focus on the new interactable
                    currentInteractable = hitInteractable;
                    currentInteractable.OnFocus();

                    treeInteractable = currentInteractable.GetComponent<TreeInteractable>();

                    if (treeInteractable && treeInteractable.playerInRange)
                    {
                        treeInteractable.canBeChopped = true;
                        treeUI.SetActive(true);
                    }

                    interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = currentInteractable.interactionText;
                    interactionUI.SetActive(true);
                }
            }
        }
        else if (currentInteractable != null)
        {
            // No hit; lose focus on the current interactable
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
            interactionUI.SetActive(false);

            if (treeInteractable != null)
            {
                treeUI.SetActive(false);
                treeInteractable.canBeChopped = false;
                treeInteractable = null;

            }

        }
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        HandleInteractionInput();
    }

    void HandleInteractionInput()
    {
        if (currentInteractable != null)
        {
            //anim?.SetTrigger("press");
            currentInteractable.OnInteract();
        }
    }

    public void UpdateInteractionText(string text)
    {
        interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }

    public void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if(treeUI != null)
        {
            treeUI.SetActive(false);
        }
    }

}
