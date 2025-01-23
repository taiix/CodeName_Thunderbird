using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeSystem : MonoBehaviour
{

    [SerializeField] private GameObject requiredItemUiPrefab;
    [SerializeField] private TextMeshProUGUI notEnoughItemsText;
    [SerializeField] private Transform requiredItemTransform;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private List<PlanePart> wings = new List<PlanePart>();
    [SerializeField] private PlanePart currentPlanePart = null;
    private PartUpgrade currentUpgrade = null;
    private InventorySystem inventorySystem;

    private GameObject itemUI;
    private Image itemImage;
    private TextMeshProUGUI itemText;

    private Dictionary<Item, int> combinedRequiredItems = new Dictionary<Item, int>();

    private string missingItemsMessage = "";
    private bool hasAllItems = true;


    private void Start()
    {
        if(GameManager.Instance == null)
        {
            Debug.Log("No game manager found for Upgrade System");
            return;
        }
        
        inventorySystem = GameManager.Instance.GetInventorySystem();
        notEnoughItemsText.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        upgradeButton.onClick.AddListener(UpgradePlanePart);
    }

    private void OnDisable()
    {
        upgradeButton.onClick.RemoveListener(UpgradePlanePart);
    }

    public void SelectPlanePart(PlanePart part)
    {
        if (part == null) return;

        currentPlanePart = part;

        if (currentPlanePart.upgrades.Count > 0)
        {
            currentUpgrade = currentPlanePart.GetCurrentUpgrade();
        }
        else
        {
            currentUpgrade = null;
        }

        combinedRequiredItems.Clear();
        UpdateRequiredItemsUI(currentUpgrade);
    }
    public void UpdateRequiredItemsUI(PartUpgrade upgrade)
    {
        foreach (Transform child in requiredItemTransform)
        {
            Destroy(child.gameObject);
        }
        if (upgrade == null) return;

        foreach (RequiredItem requiredItem in upgrade.requiredItemsList)
        {        

            itemUI = Instantiate(requiredItemUiPrefab, requiredItemTransform);
            itemImage = itemUI.GetComponent<Image>();
            itemText = itemUI.GetComponentInChildren<TextMeshProUGUI>();

            itemUI.SetActive(true);
            itemImage.sprite = requiredItem.item.itemIcon;
            //Debug.Log(requiredItem.amount);
            itemText.text = requiredItem.amount.ToString() + "x"; 
        }

    }

    public void UpgradePlanePart()
    {
        if (currentPlanePart.currentUpgradeLevel > currentPlanePart.upgrades.Count - 1)
        {
            Debug.Log("No more upgrades available for this part.");
            StartCoroutine(ShowNotEnoughItemsMessage("Good Job! No more upgrades available for this part."));
            return;
        }
        if (currentPlanePart == null || inventorySystem == null) return;

        currentUpgrade = currentPlanePart.GetCurrentUpgrade();

        // Reset message
        hasAllItems = true;
        missingItemsMessage = "";

        if (combinedRequiredItems.Count == 0)
        {
            // Combine required items with the same type and accumulate their amounts
            foreach (RequiredItem requiredItem in currentUpgrade.requiredItemsList)
            {
                if (combinedRequiredItems.ContainsKey(requiredItem.item))
                {
                    combinedRequiredItems[requiredItem.item] += requiredItem.amount;
                }
                else
                {
                    combinedRequiredItems.Add(requiredItem.item, requiredItem.amount);
                }
            }
        }

        // Check if the player has enough of each item after combining
        foreach (KeyValuePair<Item, int> entry in combinedRequiredItems)
        {
            Item item = entry.Key;
            int requiredAmount = entry.Value;
            int playerItemCount = inventorySystem.GetItemCount(item);

           // Debug.Log(item.itemName + "x " + playerItemCount + " in inventory, requires " + requiredAmount);

            if (playerItemCount < requiredAmount)
            {
                hasAllItems = false;
                int missingAmount = requiredAmount - playerItemCount;
                missingItemsMessage += "Missing items:\n" + missingAmount.ToString() + "x " + item.itemName + "\n";
            }
        }

        if (!hasAllItems)
        {
            StartCoroutine(ShowNotEnoughItemsMessage(missingItemsMessage));
            return;
        }

        foreach (KeyValuePair<Item, int> entry in combinedRequiredItems)
        {
            Item item = entry.Key;
            int requiredAmount = entry.Value;
            inventorySystem.RemoveItem(item, requiredAmount);
        }

        if (currentUpgrade != null && currentPlanePart.partName != "PlaneWing")
        {
            currentPlanePart.PartUpgrade(currentUpgrade);

            combinedRequiredItems.Clear();

            UpdateRequiredItemsUI(currentPlanePart.GetCurrentUpgrade());
        }
        else if(currentUpgrade != null && currentPlanePart.partName == "PlaneWing")
        {
            foreach(PlanePart part in wings)
            {
                part.PartUpgrade(currentUpgrade);
                combinedRequiredItems.Clear();
                UpdateRequiredItemsUI(currentPlanePart.GetCurrentUpgrade());
            }
        }
    }

    private IEnumerator ShowNotEnoughItemsMessage(string message)
    {
        notEnoughItemsText.text = message;
        notEnoughItemsText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        missingItemsMessage = string.Empty;
        notEnoughItemsText.gameObject.SetActive(false);
    }
}
