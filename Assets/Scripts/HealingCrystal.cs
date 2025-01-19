using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealingCrystal : MonoBehaviour
{
    private ItemInteractable itemInteractable;
    private EquipSystem equipSystem;

    void Start()
    {
        equipSystem = FindObjectOfType<EquipSystem>();
        itemInteractable = GetComponent<ItemInteractable>();

        if (itemInteractable.itemHoldText)
        {
            itemInteractable.itemHoldText.GetComponentInChildren<TextMeshProUGUI>().text = "Press F to heal";
        }
    }

    void Update()
    {
        if (itemInteractable && itemInteractable.itemHoldText)
        {
            if (itemInteractable.isHeld )
            {
                itemInteractable.itemHoldText.SetActive(true);
            }
            if (itemInteractable.isHeld && Input.GetKeyDown(KeyCode.F))
            {
                PlayerHealth.OnPlayerDamaged?.Invoke(-10f);
                equipSystem.UnequipItem();
                InventorySystem.Instance.RemoveItem(itemInteractable.itemSO, 1);
            }
        }
    }
}
