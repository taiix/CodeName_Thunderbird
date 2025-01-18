using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySystem : MonoBehaviour, ISavableData
{
    public static InventorySystem Instance { get; private set; }


    private bool isInventoryOpen = false;

    [SerializeField] private GameObject inventoryUI;

    public GameObject hotbarPanelUI;
    public GameObject itemPanelUI;
    public Image largeItemImage;
    public TextMeshProUGUI itemDescriptionText;
    [SerializeField] Button useButton;
    [SerializeField] Button dropButton;


    private InventorySlot selectedSlot;

    private Item equippedItem;
    private Transform originalHotbarPos;
    [SerializeField] private HotbarManager hotbarManager;
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    public List<Item> itemsInInventory = new List<Item>();


    private InputAction openInventory;
    public static event Action<Item, bool> OnItemUsed;
    public static event Action<Item> OnItemThrown;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(InitializeInventorySystem());
    }

    private IEnumerator InitializeInventorySystem()
    {
        CharacterMovement characterMovement = gameObject.GetComponent<CharacterMovement>();

        while (characterMovement == null || characterMovement.PlayerAction == null)
        {
            characterMovement = gameObject.GetComponent<CharacterMovement>();
            yield return null;
        }
        openInventory = characterMovement.PlayerAction.FindAction("Inventory");

        if (openInventory == null)
        {
            Debug.LogError("The 'Inventory' action was not found in the PlayerAction map.");
            yield break;
        }
        openInventory.performed += InventoryUIController;
        openInventory.Enable();
    }

    private void OnDisable()
    {
        openInventory.Disable();
        openInventory.performed -= InventoryUIController;
    }

    void Start()
    {
        hotbarPanelUI.SetActive(true);
        inventoryUI.SetActive(false);
        itemPanelUI.SetActive(false);
        dropButton.onClick.AddListener(DropSelectedItem);
        useButton.onClick.AddListener(UseSelectedItem);

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemInSlot == null)
            {
                for (int j = 0; j < slots[i].transform.childCount; j++)
                {
                    slots[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }
            else
            {
                slots[i].SetStats();
            }
        }
    }

    void InventoryUIController(InputAction.CallbackContext context)
    {
        if (GameManager.Instance && !GameManager.Instance.IsInteracting())
        {

            isInventoryOpen = !isInventoryOpen;
            if (isInventoryOpen)
            {
                GameManager.Instance.DisablePlayerControls(true);
                inventoryUI.SetActive(true);
                hotbarPanelUI.SetActive(true);
                hotbarPanelUI.transform.position = hotbarPanelUI.transform.position + new Vector3(-410, -10, 0);

            }
            else
            {
                GameManager.Instance.EnablePlayerControls();
                inventoryUI.SetActive(false);
                itemPanelUI.SetActive(false);

                hotbarPanelUI.transform.position = hotbarPanelUI.transform.position - new Vector3(-410, -10, 0);
            }
        }
        else
        {
            //Debug.Log("No Game manager in scene, can't disable player contros.");
        }
    }

    public void PickUpItem(ItemInteractable item)
    {
        Destroy(item.gameObject);
        int amountToAdd = item.amount;

        itemsInInventory.Add(item.itemSO);

        foreach (InventorySlot hotbarSlot in hotbarManager.hotbarSlots)
        {
            if (hotbarSlot.itemInSlot == item.itemSO && hotbarSlot.amountInSlot < item.itemSO.maxStack)
            {
                int availableSpace = item.itemSO.maxStack - hotbarSlot.amountInSlot;

                if (amountToAdd <= availableSpace)
                {
                    hotbarSlot.amountInSlot += amountToAdd;
                    hotbarSlot.SetStats();
                    return;
                }
                else
                {
                    hotbarSlot.amountInSlot += availableSpace;
                    amountToAdd -= availableSpace;
                    hotbarSlot.SetStats();
                }
            }
        }

        // If not stackable in hotbar, look for an empty hotbar slot
        foreach (InventorySlot hotbarSlot in hotbarManager.hotbarSlots)
        {
            if (hotbarSlot.itemInSlot == null)
            {
                int amountToSlot = Mathf.Min(amountToAdd, item.itemSO.maxStack);

                hotbarSlot.itemInSlot = item.itemSO;
                hotbarSlot.amountInSlot = amountToSlot;
                amountToAdd -= amountToSlot;

                hotbarSlot.SetStats();
                hotbarSlot.gameObject.SetActive(true);

                if (amountToAdd == 0) return;
            }
        }

        // If the hotbar is full, add to regular inventory slots
        foreach (InventorySlot inventorySlot in slots)
        {
            if (inventorySlot.itemInSlot == item.itemSO && inventorySlot.amountInSlot < item.itemSO.maxStack)
            {
                int availableSpace = item.itemSO.maxStack - inventorySlot.amountInSlot;

                if (amountToAdd <= availableSpace)
                {
                    inventorySlot.amountInSlot += amountToAdd;
                    inventorySlot.SetStats();
                    return;
                }
                else
                {
                    inventorySlot.amountInSlot += availableSpace;
                    amountToAdd -= availableSpace;
                    inventorySlot.SetStats();
                }
            }
        }

        // If not stackable in inventory, look for an empty inventory slot
        while (amountToAdd > 0)
        {
            InventorySlot emptySlot = slots.Find(slot => slot.itemInSlot == null);
            if (emptySlot != null)
            {
                int amountToSlot = Mathf.Min(amountToAdd, item.itemSO.maxStack);

                emptySlot.itemInSlot = item.itemSO;
                emptySlot.amountInSlot = amountToSlot;
                amountToAdd -= amountToSlot;

                emptySlot.SetStats();
                emptySlot.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("No empty slots available in the inventory.");
                return;
            }
        }
    }

    public void OnSlotClicked(InventorySlot slot)
    {
        if (slot.itemInSlot != null)
        {
            Debug.Log("Show Slot");
            selectedSlot = slot;

            itemPanelUI.SetActive(true);
            largeItemImage.sprite = slot.itemInSlot.itemIcon;
            itemDescriptionText.text = slot.itemInSlot.itemDescription;

            UpdateUseButtonText();
            //Debug.Log("Showing details for " + slot.itemInSlot.itemName);
        }
        else
        {
            itemPanelUI.SetActive(false);
        }
    }


    private void DropSelectedItem()
    {
        if (selectedSlot != null && selectedSlot.amountInSlot > 0)
        {
            if (equippedItem == selectedSlot.itemInSlot)
            {
                if (selectedSlot.amountInSlot == 1)
                {
                    OnItemUsed?.Invoke(selectedSlot.itemInSlot, false);
                    equippedItem = null;
                }
                else
                {
                    itemsInInventory.Remove(selectedSlot.itemInSlot);
                }
            }

            selectedSlot.amountInSlot--;
            Instantiate(selectedSlot.itemInSlot.itemPrefab, gameObject.transform.position + new Vector3(0, 1, 1.5f), Quaternion.identity);

            if (selectedSlot.amountInSlot <= 0)
            {
                selectedSlot.itemInSlot = null;
                itemPanelUI.SetActive(false);
                Debug.Log("Slot is now empty.");
            }

            selectedSlot.SetStats();
            CloseInventory();
        }
    }

    public bool HasRequiredItem(Item item, int requiredAmount)
    {
        int itemCount = GetItemCount(item);
        return itemCount >= requiredAmount;
    }

    public int GetItemCount(Item item)
    {
        int totalAmount = 0;

        // Check inventory slots
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemInSlot == item)
            {
                totalAmount += slot.amountInSlot;
            }
        }

        // Check hotbar slots
        foreach (InventorySlot hotbarSlot in hotbarManager.hotbarSlots)
        {
            if (hotbarSlot.itemInSlot == item)
            {
                totalAmount += hotbarSlot.amountInSlot;
            }
        }

        return totalAmount;
    }

    public void RemoveItem(Item item, int amountToRemove)
    {
        // First, check the hotbar slots
        foreach (InventorySlot hotbarSlot in hotbarManager.hotbarSlots)
        {
            if (hotbarSlot.itemInSlot == item)
            {
                if (hotbarSlot.amountInSlot >= amountToRemove)
                {
                    hotbarSlot.amountInSlot -= amountToRemove;
                    if (hotbarSlot.amountInSlot <= 0)
                    {
                        hotbarSlot.itemInSlot = null;
                    }
                    hotbarSlot.SetStats();
                    return; // All required items removed, exit method
                }
                else
                {
                    amountToRemove -= hotbarSlot.amountInSlot;
                    hotbarSlot.amountInSlot = 0;
                    hotbarSlot.itemInSlot = null;
                    hotbarSlot.SetStats();
                }
            }
        }

        // If not enough items were removed from the hotbar, check the inventory slots
        foreach (InventorySlot inventorySlot in slots)
        {
            if (inventorySlot.itemInSlot == item)
            {
                if (inventorySlot.amountInSlot >= amountToRemove)
                {
                    inventorySlot.amountInSlot -= amountToRemove;
                    if (inventorySlot.amountInSlot <= 0)
                    {
                        inventorySlot.itemInSlot = null;
                    }
                    inventorySlot.SetStats();
                    break; // All required items removed, exit loop
                }
                else
                {
                    amountToRemove -= inventorySlot.amountInSlot;
                    inventorySlot.amountInSlot = 0;
                    inventorySlot.itemInSlot = null;
                    inventorySlot.SetStats();
                }
            }
        }
    }

    private void UseSelectedItem()
    {
        if (selectedSlot != null && selectedSlot.amountInSlot > 0)
        {
            bool isEquipping = selectedSlot.itemInSlot != equippedItem;

            if (isEquipping)
            {
                OnItemUsed?.Invoke(selectedSlot.itemInSlot, true);
                equippedItem = selectedSlot.itemInSlot;
            }
            else
            {
                OnItemUsed?.Invoke(selectedSlot.itemInSlot, false);
                equippedItem = null;
            }

            CloseInventory();
        }
    }

    private void CloseInventory()
    {
        isInventoryOpen = false;
        inventoryUI.SetActive(false);
        itemPanelUI.SetActive(false);
        GameManager.Instance.EnablePlayerControls();
        hotbarPanelUI.transform.position = hotbarPanelUI.transform.position - new Vector3(-410, -10, 0);
    }

    private void UpdateUseButtonText()
    {
        // Check if the selected item is equipped and update the button text accordingly
        if (selectedSlot.itemInSlot == equippedItem)
        {
            useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unequip";
        }
        else
        {
            useButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
        }
    }

    public void SwapItems(InventorySlot slot1, InventorySlot slot2)
    {
        Item tempItem = slot1.itemInSlot;
        int tempAmount = slot1.amountInSlot;

        slot1.itemInSlot = slot2.itemInSlot;
        slot1.amountInSlot = slot2.amountInSlot;

        slot2.itemInSlot = tempItem;
        slot2.amountInSlot = tempAmount;
    }

    public void UpdateEquippedItem(Item pEquippedItem)
    {
        equippedItem = pEquippedItem;
    }

    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }

    public void AddSlot(InventorySlot slot)
    {
        slots.Add(slot);
    }

    public void InvokeItemThrown(Item item)
    {
        OnItemThrown?.Invoke(item);
    }

    public Item GetEquippedItem()
    {
        if (equippedItem != null)
        {
            return equippedItem;
        }
        return null;
    }

    public List<InventorySlot> GetInventorySlots()
    {
        return slots;
    }

    public string ToJson()
    {
        List<InventorySlotData> slotsData = new List<InventorySlotData>();

        foreach (var slot in slots)
        {
            if (slot.itemInSlot != null && slot.itemInSlot.itemName != null)
            {
                InventorySlotData slotData = new InventorySlotData(slot.itemInSlot.itemName, slot.amountInSlot);
                slotsData.Add(slotData);
            }
        }

        InventoryData data = new InventoryData(slotsData);
        Debug.Log($"{data.inventoryItems.Count} items saved in the inventory");
        return JsonUtility.ToJson(data, true);
    }

    public void FromJson(string json)
    {
        ClearInventory();

        InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);


        foreach (var slot in inventoryData.inventoryItems)
        {
            Item item = slot.GetItem();

            if (item != null)
            {
                for (int i = 0; i < slot.quantity; i++)
                {
                    itemsInInventory.Add(item);
                }

                int remainingQuantity = slot.quantity;

                foreach (var s in slots)
                {
                    if (s.itemInSlot == null && remainingQuantity > 0)
                    {
                        s.itemInSlot = item;
                        s.itemIcon.sprite = item.itemIcon;
                        s.amountInSlot = Mathf.Min(remainingQuantity, slot.quantity);
                        s.SetStats();

                        remainingQuantity -= s.amountInSlot;
                    }
                    if (remainingQuantity <= 0)
                    {
                        break;
                    }
                }
            }
        }
    }

    void ClearInventory()
    {
        foreach (var s in slots)
        {
            s.itemInSlot = null;
            s.itemIcon.sprite = null;
            s.amountInSlot = 0;
            s.SetStats();
        }

        itemsInInventory.Clear();
    }
}