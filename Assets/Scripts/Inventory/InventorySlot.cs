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


    public void SetStats()
    {

        // Initialize UI components if not already set
        if (itemIcon == null)
        {
            itemIcon = GetComponentInChildren<Image>(); // Ensure itemIcon is assigned
        }

        if (amountText == null)
        {
            amountText = GetComponentInChildren<TextMeshProUGUI>(); // Ensure amountText is assigned
        }

        if (itemInSlot != null)
        {
            itemIcon.sprite = itemInSlot.itemIcon; // Set item icon sprite
            amountText.text = amountInSlot.ToString() + "x"; // Set amount text

            // Make sure the UI elements are active
            itemIcon.gameObject.SetActive(true);
            amountText.gameObject.SetActive(true);
        }
        else
        {
            // Hide UI elements if there's no item
            itemIcon.sprite = null;
            amountText.text = string.Empty;

            itemIcon.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
    }

}
