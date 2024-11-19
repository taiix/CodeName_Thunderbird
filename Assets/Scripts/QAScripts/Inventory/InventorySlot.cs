using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item itemInSlot;
    public int amountInSlot = 1;

    public Image itemIcon;
    public TextMeshProUGUI amountText;

    private Canvas mainCanvas;
    private Button slotButton;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private InventorySlot originalSlot;

    private Transform originalParent;

    private GameObject draggingIcon;
    private void Start()
    {
        mainCanvas = GetComponentInParent<Canvas>();
        originalParent = itemIcon.transform.parent;
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(OnSlotClicked);

        if (itemIcon == null)
        {
            itemIcon = GetComponentInChildren<Image>();
        }
        if (amountText == null)
        {
            amountText = GetComponentInChildren<TextMeshProUGUI>();
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void SetStats()
    {
        if (itemInSlot != null)
        {
            itemIcon.sprite = itemInSlot.itemIcon;
            amountText.text = amountInSlot.ToString() + "x";

         
            itemIcon.gameObject.SetActive(true);
            amountText.gameObject.SetActive(true);
        }
        else
        {
          
            itemIcon.sprite = null;
            amountText.text = string.Empty;
            itemIcon.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
    }

    private void OnSlotClicked()
    {
      
        InventorySystem.Instance.OnSlotClicked(this);
    }

    // Begin Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemInSlot != null)
        {
       
            originalPosition = itemIcon.transform.position;
            originalSlot = this;

            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                itemIcon.transform.SetParent(canvas.transform);
            }

            canvasGroup.blocksRaycasts = false;
        }
    }

    // Handle Drag
    public void OnDrag(PointerEventData eventData)
    {
        InventorySlot draggedSlot = eventData.pointerDrag?.GetComponent<InventorySlot>();
        itemIcon.transform.position = eventData.position;
        if (draggedSlot != null && draggedSlot != this)
        {
            InventorySystem.Instance.SwapItems(draggedSlot, this);

            draggedSlot.SetStats();
            this.SetStats();
        }
    }

    // End Drag
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerCurrentRaycast.gameObject == null)
        {
            itemIcon.transform.SetParent(originalParent);
            itemIcon.transform.localPosition = Vector3.zero;
        }
        else
        {
            itemIcon.transform.SetParent(originalSlot.transform);
            itemIcon.transform.localPosition = Vector3.zero;
        }
    }

    // Handle Drop
    public void OnDrop(PointerEventData eventData)
    {

        InventorySlot draggedSlot = eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot != null && draggedSlot != this)
        {
            InventorySystem.Instance.SwapItems(draggedSlot, this);

            draggedSlot.SetStats();
            this.SetStats();
        }

        if (eventData.pointerDrag != null)
        {
            draggedSlot.itemIcon.transform.SetParent(draggedSlot.transform);
            draggedSlot.itemIcon.transform.localPosition = Vector3.zero;
        }

    }

    // Swap items if an item is being dragged over this slot
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (originalSlot != null && originalSlot != this)
        {
            InventorySystem.Instance.SwapItems(originalSlot, this);
        }
    }
}
