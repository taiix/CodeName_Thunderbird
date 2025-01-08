using UnityEngine;

public class TreeInteractable : Interactable
{
    [SerializeField] private GameObject choppedTreePrefab;

    public float treeMaxHealth = 10;
    public float treeHealth;

    public bool canBeChopped = false;
    public bool playerInRange = false;

    void Start()
    {
        treeHealth = treeMaxHealth;
    }
    public override void OnFocus()
    {
    }

    public override void OnInteract()
    {

    }

    public override void OnLoseFocus()
    {
    }


    // Update is called once per frame
    void Update()
    {
        if (canBeChopped)
        {
            GlobalTreeHealthbar.Instance.interactableHealth = treeHealth;
            GlobalTreeHealthbar.Instance.interactableMaxHealth = treeMaxHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit()
    {
        treeHealth -= 1;

        if (treeHealth <= 0)
        {
            OnTreeDeath();
        }
    }

    void OnTreeDeath()
    {
        InteractionHandler.Instance.HideInteractionUI();
        this.gameObject.SetActive(false);
        Instantiate(choppedTreePrefab, this.transform.position, this.transform.rotation);
        RemoveObject(this.gameObject);
    }
}
