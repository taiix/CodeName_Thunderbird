using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarManager : MonoBehaviour
{
    public InventorySlot[] hotbarSlots; // Hotbar slots for quick item access


    private EquipSystem equipSystem;
    private InventorySystem inventorySystem;

    private void Start()
    {
        try
        {
            inventorySystem = GetComponent<InventorySystem>();
            equipSystem = GetComponent<EquipSystem>();
        }
        catch
        {
            Debug.LogError("No InventorySystem or EquipSystem attached to player. Hotbar unable to work.");
            return;
        }

        InventorySystem.Instance.OnItemThrown += UpdateHotbarAfterThrow;
        UpdateHotbar();
    }

    private void Update()
    {
        ChooseItem();
    }

    // Update the hotbar based on the inventory
    public void UpdateHotbar()
    {
        // Fill the hotbar with items from the inventory
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (i < inventorySystem.itemsInInventory.Count)
            {
                hotbarSlots[i].itemInSlot = inventorySystem.itemsInInventory[i]; // Assign item from inventory to hotbar
                hotbarSlots[i].amountInSlot = inventorySystem.GetItemCount(inventorySystem.itemsInInventory[i]);
                hotbarSlots[i].SetStats();
            }
            else
            {
                hotbarSlots[i].itemInSlot = null; // Clear hotbar slot if no item available
                hotbarSlots[i].amountInSlot = 0;
                hotbarSlots[i].SetStats();
            }
        }
    }

    // Listen for key inputs to equip items
    void ChooseItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipItem(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) EquipItem(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) EquipItem(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) EquipItem(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) EquipItem(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6)) EquipItem(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7)) EquipItem(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8)) EquipItem(7);
    }

    // Equip an item from the hotbar based on the selected slot index
    private void EquipItem(int slotIndex)
    {

        if (hotbarSlots[slotIndex].itemInSlot != null)
        {
            Debug.Log("Equip item");
            Item itemToEquip = hotbarSlots[slotIndex].itemInSlot;

            equipSystem.EquipItem(itemToEquip);
            inventorySystem.RemoveItem(itemToEquip, 1);

        }
        else
        {
            equipSystem.UnequipItem();
        }
    }
    private void UpdateHotbarAfterThrow(Item thrownItem)
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            if (hotbarSlots[i].itemInSlot == thrownItem)
            {
                hotbarSlots[i].amountInSlot--;
                if (hotbarSlots[i].amountInSlot <= 0)
                {
                    hotbarSlots[i].itemInSlot = null; 
                }
                hotbarSlots[i].SetStats();
                break;
            }
        }
    }

}
