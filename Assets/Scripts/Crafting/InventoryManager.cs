using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    GameObject draggedObject;
    GameObject lastItemSlot;

    [SerializeField] GameObject inventoryUI;

    [SerializeField]  InputActionAsset playerActions;
    private InputAction openInventory;

    bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.SetActive(false);
    }

    private void OnEnable()
    {
        openInventory = playerActions.FindAction("Inventory");

        openInventory.performed += ToggleInventory;

        openInventory.Enable();
    }

    private void OnDisable()
    {
        openInventory.Disable();

        openInventory.performed -= ToggleInventory;

    }

    // Update is called once per frame
    void Update()
    {
        if(draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();
            if ( slot != null && slot.itemHeld != null)
            {
                draggedObject = slot.itemHeld;
                slot.itemHeld = null;
                lastItemSlot = clickedObject;
            }
        }
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {

            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            if(slot != null && slot.itemHeld == null)
            {
                slot.SetHeldItem(draggedObject);
                draggedObject = null;
            }
            else if(slot != null && slot.itemHeld != null)
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(slot.itemHeld);
                slot.SetHeldItem(draggedObject);
                draggedObject = null;
            }
        }
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);
    }
}
