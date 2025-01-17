using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// This was put together for quick testing it needs to be refactored or completely removed
    /// </summary>
    public int damageAmount = 3;
    public float throwForce = 15f;

    private Rigidbody stoneRigidbody;
    private ItemInteractable item;

    private GameObject player;
    [SerializeField] private GameObject owner;

    private bool isThrown = false;
    private bool hasDealtDamage = false;

    private void Awake()
    {
        player = FindObjectOfType<PlayerHealth>().gameObject;
        owner = player;
        stoneRigidbody = GetComponent<Rigidbody>();
        item = GetComponent<ItemInteractable>();
    }

    private void Update()
    {
        if (item != null && item.isHeld && Input.GetMouseButtonUp(0) && !isThrown)
        {
            Throw(transform.forward * 10 + transform.up * 4.5f, true, owner);
        }
    }

    public void Throw(Vector3 force, bool updateInventory, GameObject pOwner)
    {
        owner = pOwner;
        hasDealtDamage = false;
        transform.SetParent(null);
        item.isHeld = false;
        item.isThrown = true;

        if (updateInventory)
        {
            InventorySystem.Instance.RemoveItem(item.itemSO, 1);
            InventorySystem.Instance.InvokeItemThrown(item.itemSO);
        }

        stoneRigidbody.GetComponent<Collider>().enabled = true;
        stoneRigidbody.isKinematic = false;

        stoneRigidbody.AddForce(force, ForceMode.VelocityChange);
        isThrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"Collision detected with: {collision.gameObject.name}");
        //if (collision.gameObject == owner.gameObject) return;

        if (collision.gameObject.CompareTag("Player") && !hasDealtDamage && owner != player)
        {
            //Debug.Log("hitPlayer");
            PlayerHealth.OnPlayerDamaged?.Invoke(damageAmount);
            hasDealtDamage = true;
        }
        if (collision.gameObject.CompareTag("Enemy") && owner == player && !item.isHeld)
        {
            //Debug.Log("Rock hit enemey");
            Vector3 collisionPoint = collision.GetContact(0).point;
            collision.gameObject.GetComponent<EnemyAI>().TakeDamage(damageAmount,collisionPoint);
            hasDealtDamage = true;
            return;
        }
    }
}