using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private InputAction openInventory;

    private bool isInventoryOpen = false;

    public GameObject inventoryUI;

    public GameObject itemPanelUI;
    public Image largeItemImage;
    public TextMeshProUGUI itemDescriptionText;
    public Button dropButton;

    private InventorySlot selectedSlot;

    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();

    public List<Item> itemsInInventory = new List<Item>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.SetActive(false);
        itemPanelUI.SetActive(false);
        dropButton.onClick.AddListener(DropSelectedItem);


        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemInSlot == null)
            {
                for (int j = 0; j < slots[i].transform.childCount; j++)
                {
                    slots[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InventoryUIController(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null)
        {

            isInventoryOpen = !isInventoryOpen;
            if (isInventoryOpen)
            {
                GameManager.Instance.DisablePlayerControls();
                inventoryUI.SetActive(true);
            }
            else
            {
                GameManager.Instance.EnablePlayerControls();
                inventoryUI.SetActive(false);
                itemPanelUI.SetActive(false);
            }
        }
        else
        {
            Debug.Log("No Game manager in scene, can't disable player contros."); 
        }
    }


    public void PickUpItem(ItemInteractable item)
    {
        Destroy(item.gameObject);
        int amountToAdd = item.amount;

        itemsInInventory.Add(item.itemSO);

        foreach (InventorySlot slot in slots)
        {
            if (slot.itemInSlot == item.itemSO && slot.amountInSlot < item.itemSO.maxStack)
            {
                int availableSpace = item.itemSO.maxStack - slot.amountInSlot;

                if (amountToAdd <= availableSpace)
                {
                    slot.amountInSlot += amountToAdd;
                    slot.SetStats();
                    return;
                }
                else
                {
                    slot.amountInSlot += availableSpace;
                    amountToAdd -= availableSpace;
                    slot.SetStats();
                }
            }
        }

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
            selectedSlot = slot;

            itemPanelUI.SetActive(true);
            largeItemImage.sprite = slot.itemInSlot.itemIcon;
            itemDescriptionText.text = slot.itemInSlot.itemDescription;

            Debug.Log("Showing details for " + slot.itemInSlot.itemName);
        }
    }


    private void DropSelectedItem()
    {
        if (selectedSlot != null && selectedSlot.amountInSlot > 0)
        {
            itemsInInventory.Remove(selectedSlot.itemInSlot);
            selectedSlot.amountInSlot--;

            if (selectedSlot.amountInSlot <= 0)
            {
                selectedSlot.itemInSlot = null;
                itemPanelUI.SetActive(false);
                //selectedSlot.gameObject.SetActive(false);
                Debug.Log("Slot is now empty.");
            }
            selectedSlot.SetStats();
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

        foreach (InventorySlot slot in slots)
        {
            if (slot.itemInSlot == item)
            {
                totalAmount += slot.amountInSlot;
            }
        }
        return totalAmount;
    }

    public void RemoveItem(Item item, int amountToRemove)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.itemInSlot == item)
            {
                if (slot.amountInSlot >= amountToRemove)
                {
                    slot.amountInSlot -= amountToRemove;
                    if (slot.amountInSlot <= 0)
                    {
                        slot.itemInSlot = null;

                    }
                    slot.SetStats();
                    break;
                }
                else
                {
                    amountToRemove -= slot.amountInSlot;
                    slot.amountInSlot = 0;
                    slot.itemInSlot = null;
                    slot.SetStats();
                    //slot.gameObject.SetActive(false);
                }
            }
        }
    }

}