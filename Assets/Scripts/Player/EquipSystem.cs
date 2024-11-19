using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSystem : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    private GameObject equippedItemInstance = null;
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
            if (equippedItemInstance.gameObject.GetComponent<Collider>() != null)
            {
                equippedItemInstance.gameObject.GetComponent<Collider>().enabled = false;
                Destroy(equippedItemInstance.gameObject.GetComponent<Rigidbody>());
            }
            currentEquippedItem = item;
            InventorySystem.Instance.UpdateEquippedItem(currentEquippedItem);
        }
    }

    public void UnequipItem()
    {
        if (equippedItemInstance != null)
        {
            Debug.Log("Unequip item called");
            Destroy(equippedItemInstance);
            currentEquippedItem = null;
        }
    }

    // Check if an item is equipped
    public bool IsItemEquipped(Item item)
    {
        return currentEquippedItem == item;
    }
}
