using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Item itemInSlot;
    public int amountInSlot = 1;


    public Image itemIcon;
    public TextMeshProUGUI amountText;
    TextMeshProUGUI descriptionText;

    private Button slotButton;

    void Start()
    {
        slotButton = GetComponent<Button>();
        slotButton.onClick.AddListener(OnSlotClicked); 
    }
    public void SetStats()
    {

        // Initialize UI components if not already set
        if (itemIcon == null)
        {
            itemIcon = GetComponentInChildren<Image>(); 
        }

        if (amountText == null)
        {
            amountText = GetComponentInChildren<TextMeshProUGUI>(); 
        }

        if (itemInSlot != null)
        {
            itemIcon.sprite = itemInSlot.itemIcon; 
            amountText.text = amountInSlot.ToString() + "x"; 

            // Make sure the UI elements are active
            itemIcon.gameObject.SetActive(true);
            amountText.gameObject.SetActive(true);
        }
        //else
        //{
        //    // Hide UI elements if there's no item
        //    itemIcon.sprite = null;
        //    amountText.text = string.Empty;

        //    itemIcon.gameObject.SetActive(false);
        //    amountText.gameObject.SetActive(false);
        //}
    }

    private void OnSlotClicked()
    {
        // Notify the InventorySystem that this slot was clicked.
        InventorySystem.Instance.OnSlotClicked(this);
    }

}
