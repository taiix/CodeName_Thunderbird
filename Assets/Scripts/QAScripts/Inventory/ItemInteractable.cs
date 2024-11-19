using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemInteractable : Interactable
{
    public Item itemSO;
    public int amount = 1;

    [SerializeField] private Material highlightShader;

    [SerializeField] private List<Material> originalMaterials = new();
    private MeshRenderer rend;

    private void Start()
    {
        if (highlightShader == null) Debug.LogError($"No highlight matterial assigned {this.gameObject.name}");

        rend = GetComponent<MeshRenderer>();
        originalMaterials.AddRange(rend.materials);
    }


    public override void OnFocus()
    {
        if (interactionText == null) Debug.Log("No interaction text");

        interactionText = "Press 'F' to pick up " + amount + " " + itemSO.itemName;

        originalMaterials.Add(highlightShader);
        rend.materials = originalMaterials.ToArray();
    }

    public override void OnInteract()
    {
        InventorySystem.Instance?.PickUpItem(this);

        InteractionHandler.Instance?.UpdateInteractionText(string.Empty);
    }

    public override void OnLoseFocus()
    {
        interactionText = string.Empty;

        originalMaterials.Remove(highlightShader);
        rend.materials = originalMaterials.ToArray();
    }
}
