using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    [SerializeField]private GameObject equippedItemInstance = null;

    private ItemInteractable equippedItem;
    private Item currentEquippedItem = null;



    private void Start()
    {
        InventorySystem.OnItemUsed += HandleItemUsed;
        GameManager.Instance.OnPlayerEnterPlane += UnequipItem;
    }

    private void OnDestroy()
    {
        InventorySystem.OnItemUsed -= HandleItemUsed;
        GameManager.Instance.OnPlayerEnterPlane -= UnequipItem;
    }

    // Method to handle item usage from the inventory
    private void HandleItemUsed(Item item, bool isEquipping)
    {
        if (isEquipping)
        {
            EquipItem(item);
        }
        else
        {
            UnequipItem();
        }
    }

    // Equip an item in the player's hand
    public void EquipItem(Item item)
    {
        if (equippedItemInstance != null)
        {
            UnequipItem();
        }

        if (item != null && item.itemPrefab != null)
        {
            equippedItemInstance = Instantiate(item.itemPrefab, handTransform);
            equippedItem = equippedItemInstance.GetComponent<ItemInteractable>();
            equippedItem.isHeld = true;
            if (equippedItemInstance.gameObject.GetComponent<Collider>() != null && equippedItemInstance.gameObject.GetComponent<Rigidbody>() != null)
            {
                equippedItemInstance.gameObject.GetComponent<Collider>().enabled = false;
                equippedItemInstance.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            }

            currentEquippedItem = item;
            InventorySystem.Instance.UpdateEquippedItem(currentEquippedItem);
        }
    }

    public void UnequipItem()
    {
        if (equippedItem != null && !equippedItem.isThrown)
        {
            Debug.Log("Unequip item called");
            equippedItem.isHeld = false;
            Destroy(equippedItemInstance.gameObject);
            currentEquippedItem = null;
        }
    }

    public bool IsItemEquipped(Item item)
    {
        return currentEquippedItem == item;
    }
}
