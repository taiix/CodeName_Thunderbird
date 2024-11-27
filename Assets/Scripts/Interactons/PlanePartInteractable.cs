using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePartInteractable : Interactable
{
    private PlanePart planePart;
    [SerializeField] private int repairAmount = 50;

    public override void Awake()
    {
        base.Awake();
        planePart = GetComponent<PlanePart>();

        if (planePart == null)
        {
            Debug.LogError("DamageablePart component is missing on this GameObject!");
        }

        //interactionText = planePart.IsDamaged ? "Press F to Repair" : "Part is Healthy";
    }
    public override void OnFocus()
    {
        interactionText = planePart.IsDamaged ? RepairMessage() : string.Empty;
    }

    public override void OnInteract()
    {
        if (planePart.IsDamaged)
        {
            planePart.Repair(repairAmount); 

            Debug.Log("Repaired " + gameObject.name);

            if (!planePart.IsDamaged)
            {
                InteractionHandler.Instance.HideInteractionUI();
            }
            else
            {
                interactionText = "Press F to Repair " + planePart.partName; 
            }
        }

    }

    public override void OnLoseFocus()
    {
    }

    private string RepairMessage()
    {


        string itemsRequired = string.Empty;

        for (int i = 0; i < planePart.upgrades[planePart.currentUpgradeLevel].itemsForFix.Count; i++)
        {
            itemsRequired += " " + planePart.upgrades[planePart.currentUpgradeLevel].itemsForFix[i].item.itemName + "x" + planePart.upgrades[planePart.currentUpgradeLevel].itemsForFix[i].amount + "\n";
        }

        return "You need " + itemsRequired + " to repair the " + planePart.partName;
    }
}
