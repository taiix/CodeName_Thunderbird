using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeSystem : MonoBehaviour
{

    [SerializeField] private GameObject requiredItemUiPrefab;
    [SerializeField] private Transform requiredItemTransform;
    [SerializeField] private Button upgradeButton;

    private PlanePart currentPlanePart = null;
    private PartUpgrade currentUpgrade = null;
    private InventorySystem inventorySystem;

    private void Start()
    {
        if(GameManager.Instance == null)
        {
            Debug.Log("No game manager found for Upgrade System");
            return;
        }
        
        inventorySystem = GameManager.Instance.GetInventorySystem();
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

            GameObject itemUI = Instantiate(requiredItemUiPrefab, requiredItemTransform);
            Image itemImage = itemUI.GetComponent<Image>();
            TextMeshProUGUI itemText = itemUI.GetComponentInChildren<TextMeshProUGUI>();

            itemUI.SetActive(true);
            itemImage.sprite = requiredItem.item.itemIcon;
            //Debug.Log(requiredItem.amount);
            itemText.text = requiredItem.amount.ToString() + "x"; 
        }

    }

    public void UpgradePlanePart()
    {
        if (currentPlanePart == null || inventorySystem == null) return;
        currentUpgrade = currentPlanePart.GetCurrentUpgrade();

        foreach (RequiredItem requiredItem in currentUpgrade.requiredItemsList)
        {
            if (!inventorySystem.HasRequiredItem(requiredItem.item, requiredItem.amount))
            {
                Debug.Log("Not enough of required item: " + requiredItem.item.itemName);
                return; 
            }

            inventorySystem.RemoveItem(requiredItem.item, requiredItem.amount);
        }

        if (currentUpgrade != null)
        {
            // Perform the upgrade
            currentPlanePart.PartUpgrade(currentUpgrade);

            UpdateRequiredItemsUI(currentPlanePart.GetCurrentUpgrade());
        }
        else
        {
            Debug.Log("No more upgrades available for this part.");
        }
    }
}
