using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// This was put together for quick testing it needs to be refactored
    /// </summary>
    public int damageAmount = 4;
    public float throwForce = 15f;

    private Rigidbody stoneRigidbody;
    private ItemInteractable item;
    private bool isThrown = false;

    private GameObject user;
    private bool hasDealtDamage = false;
    private bool hasHitPlayer = false;

    private void Awake()
    {
        
        stoneRigidbody = GetComponent<Rigidbody>();
        item = GetComponent<ItemInteractable>();
    }

    private void Update()
    {
        if (item != null && item.isHeld && Input.GetMouseButtonUp(0) && !isThrown)
        {
            Throw(transform.forward * 10 + transform.up * 4.5f);
        }
    }

    public void Throw(Vector3 force, bool updateInventory = true)
    {
        hasDealtDamage = false;
        item.isHeld = false;
        transform.SetParent(null);

        if (updateInventory)
        {
            InventorySystem.Instance.RemoveItem(item.itemSO, 1);
            InventorySystem.Instance.InvokeItemThrown(item.itemSO);
        }

        stoneRigidbody.GetComponent<Collider>().enabled = true;
        stoneRigidbody.isKinematic = false;

        stoneRigidbody.AddForce(force, ForceMode.VelocityChange);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            hasDealtDamage = false;
            return;
        }
        if (other.CompareTag("Player") && !hasDealtDamage && !item.isHeld)
        {
            Debug.Log("hitPlayer");
            PlayerHealth.OnPlayerDamaged?.Invoke(damageAmount);
        }

        hasDealtDamage = true;
    }
}
