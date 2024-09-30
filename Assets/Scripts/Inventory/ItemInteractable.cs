using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemInteractable : Interactable
{
    public Item itemSO;
    public int amount = 1;
    

    private void Start()
    {
      
    }


    public override void OnFocus()
    {
        if(interactionText == null)
        {
            Debug.Log("No interaction text");
        }
        interactionText = "Press 'F' to pick up " + amount + " " + itemSO.itemName;
    }

    public override void OnInteract()
    {
        InventorySystem.Instance.PickUpItem(this);
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;   
    }
}
