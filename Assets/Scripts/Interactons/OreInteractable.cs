using UnityEngine;

public class OreInteractable : Interactable
{
    [SerializeField] private Item oreItem;
    //[SerializeField] private MiningMiniGame miningMiniGame; 
    private bool isBeingMined = false;

    public override void OnFocus()
    {
        if (!isBeingMined)
        {
            interactionText = "Press 'F' to start mining " + oreItem.itemName;
        }
    }

    public override void OnInteract()
    {
        if (!isBeingMined)
        {
            interactionText = string.Empty;
            isBeingMined = true;
            CircleMinigame.OnItemReceived?.Invoke(oreItem, this.gameObject);
            InteractionHandler.Instance.HideInteractionUI();
        }
     
    }

    public override void OnLoseFocus()
    {
        if (!isBeingMined)
        {
            interactionText = string.Empty;
        }
    }

    public void BreakOre(int spawnAmount)
    {
        isBeingMined = false;
        SpawnMinedItems(spawnAmount);
        Destroy(gameObject); 
    }

    private void SpawnMinedItems(int amountToSpawn)
    {
        if (oreItem.itemPrefab != null)
        {
            for (int i = 0; i < amountToSpawn; i++)
            {
                Instantiate(oreItem.itemPrefab, transform.position + new Vector3(Random.Range(-2f,2f), 1,Random.Range(-2f, 2f)), Quaternion.Euler(new Vector3(0,Random.Range(0, 360),0)));
            }
        }
    }
}
