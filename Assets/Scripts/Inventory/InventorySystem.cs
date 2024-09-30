using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; } 

    public GameObject inventoryUI;
    [SerializeField] public List<InventorySlot> slots;


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

    // Start is called before the first frame update
    void Start()
    {
        foreach(InventorySlot slot in Resources.FindObjectsOfTypeAll<InventorySlot>())
        {
            slots.Add(slot);
        }  

        for(int i = 0; i < slots.Count; i++)
        {
            if(slots[i].itemInSlot == null)
            {
                for(int j = 0; j < slots[i].transform.childCount; j++)
                {
                    slots[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (!inventoryUI.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            inventoryUI.SetActive(true);
        }
        else if(inventoryUI.activeInHierarchy && Input.GetKeyDown(KeyCode.E))
        {
            inventoryUI.SetActive(false);  
        }
        
    }

   public void PickUpItem(ItemInteractable item)
    {
        InventorySlot existingSlot = slots.Find(slot => slot.itemInSlot == item.itemSO);
        if (existingSlot != null)
        {
           
            existingSlot.amountInSlot += item.amount;
            existingSlot.SetStats();
            Debug.Log($"Increased amount of {item.itemSO.itemName} to {existingSlot.amountInSlot}.");
        }
        else
        {
            
            InventorySlot emptySlot = slots.Find(slot => slot.itemInSlot == null);
            if (emptySlot != null)
            {
               
                emptySlot.itemInSlot = item.itemSO;
                emptySlot.amountInSlot = item.amount;

                
                emptySlot.SetStats();
                emptySlot.gameObject.SetActive(true); 

                Debug.Log($"Added {item.amount} {item.itemSO.itemName}(s) to the inventory.");
            }
            else
            {
                Debug.LogWarning("No empty slots available in the inventory.");
            }
        }
    }

}
