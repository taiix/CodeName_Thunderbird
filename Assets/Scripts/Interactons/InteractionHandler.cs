using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionHandler : MonoBehaviour
{
    public Vector3 interactionRaypoint = new Vector3(0.5f, 0.5f, 0f);
    public float interactionDistance = default;

    public Interactable currentInteractable;

    private Camera mainCamera;
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private PlayerInput playerInput;

    private IPlayer playerControls;

    [SerializeField] public GameObject interactionUI;
 

    private void Awake()
    {
        inputAsset = this.GetComponentInParent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");
        playerInput = this.GetComponentInParent<PlayerInput>();
        mainCamera = playerInput.camera;
    }
    void Start()
    {
        playerControls = this.GetComponentInParent<IPlayer>();
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
    void Update()
    {
        HandleInteractionCheck();
    }

    void HandleInteractionCheck()
    {
        var ray = mainCamera.ViewportPointToRay(interactionRaypoint);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            //If there is no currentInteractable or the hit object is an interactable different from the current one 
            if (currentInteractable == null || hit.collider.gameObject != currentInteractable.gameObject)
            {
                //If we had an interactable before hitting the new one call the onLoseFocus method for that interactable
                if (currentInteractable != null)
                {
                    //button.IsPressed = false;
                    currentInteractable.OnLoseFocus();
                    interactionUI.SetActive(false);

                }

                hit.collider.TryGetComponent(out currentInteractable);
                //Check if the object we are hitting is an interactable and assign the current interactable 
                if (hit.collider.TryGetComponent(out currentInteractable))
                {
                    //If the current interactable is in range of the raycast call the OnFocus method 
                    currentInteractable.OnFocus();

                    interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = currentInteractable.interactionText;

                    interactionUI.SetActive(true);          
                }
            }
        }

        else if (currentInteractable == null)
        {
            //interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = "Press F";
        }

        //If we are not looking at an interactable nullify the last current interactable 
        else if (currentInteractable != null)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
            interactionUI.SetActive(false);
            //interactionUI.GetComponentInChildren<TextMeshProUGUI>().text = "Press F";
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

}
