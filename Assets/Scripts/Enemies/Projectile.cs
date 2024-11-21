using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damageAmount = 4;
    public float throwForce = 15f;

    private Rigidbody stoneRigidbody;
    private ItemInteractable item; // Reference to the interactable script
    private bool isThrown = false; // Ensure the stone is only thrown once

    private void Awake()
    {
        stoneRigidbody = GetComponent<Rigidbody>();
        item = GetComponent<ItemInteractable>();
    }

    private void Update()
    {
        if (item != null && item.isHeld && Input.GetMouseButtonUp(0) && !isThrown)
        {
            ThrowStone();
        }
    }

    private void ThrowStone()
    {
        isThrown = true; 
        item.isHeld = false;
        transform.SetParent(null);

        InventorySystem.Instance.RemoveItem(item.itemSO, 1);
        InventorySystem.Instance.InvokeItemThrown(item.itemSO);

        stoneRigidbody.GetComponent<Collider>().enabled = true;
        stoneRigidbody.isKinematic = false;

        Vector3 throwDirection = (transform.forward + transform.up * 0.5f).normalized; 
        stoneRigidbody.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isThrown && other.CompareTag("Player"))
        {
            PlayerHealth.OnPlayerDamaged?.Invoke(damageAmount);
        }
    }
}
