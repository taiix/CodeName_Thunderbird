using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject itemHeld;
    
    public void SetHeldItem(GameObject item)
    {
        itemHeld = item;
        itemHeld.transform.position = transform.position;
    }
}
