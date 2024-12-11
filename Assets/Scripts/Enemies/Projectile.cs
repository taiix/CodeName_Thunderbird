using System.Collections;
using System.Collections.Generic;
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
    private GameObject owner;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other == owner.gameObject) return;

        if (other.CompareTag("Player") && !hasDealtDamage && owner != player)
        {
            Debug.Log("hitPlayer");
            PlayerHealth.OnPlayerDamaged?.Invoke(damageAmount);
            hasDealtDamage = true;
        }
        if (other.CompareTag("Enemy") && owner == player && !item.isHeld)
        {
            other.GetComponent<EnemyAI>().TakeDamage(damageAmount);
            hasDealtDamage = true;
            return;
        }
    }
}