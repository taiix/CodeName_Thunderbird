using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] private GameObject requiredItemUiPrefab;
    [SerializeField] private Transform requiredItemTransform;
    [SerializeField] Button upgradeButton;


    PlanePart currentPlanePart;

    private PartUpgrade currentUpgrade;
    private int currentUpgradeIndex = 0;

    private void Start()
    {
        currentPlanePart = null;
        currentUpgrade = null;
    }

    public void SelectPlanePart(PlanePart part)
    {
        if (part == null) return;

        currentPlanePart = part;
        currentUpgradeIndex = 0;

        if (currentPlanePart.upgrades.Count > 0)
        {
            currentUpgrade = currentPlanePart.upgrades[currentUpgradeIndex];
        }
        else
        {
            currentUpgrade = null;
        }
        UpdateRequiredItemsUI();
    }
    public void UpdateRequiredItemsUI()
    {
        foreach (Transform child in requiredItemTransform)
        {
            Destroy(child.gameObject);
        }
        if (currentUpgrade == null) return; 

        foreach (RequiredItem requiredItem in currentUpgrade.requiredItemsList)
        {
            // Create a UI element for each required item
            GameObject itemUI = Instantiate(requiredItemUiPrefab, requiredItemTransform);
            // Assuming the prefab has an Image and a Text component
            Image itemImage = itemUI.GetComponent<Image>();
            TextMeshProUGUI itemText = itemUI.GetComponentInChildren<TextMeshProUGUI>();

            itemUI.SetActive(true);
            itemImage.sprite = requiredItem.item.itemIcon;
            Debug.Log(requiredItem.amount);
            itemText.text = requiredItem.amount.ToString() + "x"; 
        }

    }


}
