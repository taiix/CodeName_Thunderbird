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
        interactionText = planePart.IsDamaged ? "Press F to Repair " + planePart.partName : string.Empty;
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
}
