using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePart : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth = 50;

    private Rigidbody planeRb;
    public float damageFactor = 1f;

    public string partName;
    public bool IsDamaged => currentHealth < maxHealth;

    public float CurrentHealth { get; private set; }

    public bool setCurrentHealth = false;

    private int upgradePower = 0;
    public int UpgradePower { get; private set; }

    [SerializeField] public List<PartUpgrade> upgrades = new List<PartUpgrade>();

    public int currentUpgradeIndex = 0;

    private void Start()
    {
        planeRb = GetComponentInParent<Rigidbody>();
        if (!setCurrentHealth)
        {
            currentHealth = maxHealth;
        }

        // Register the smoke effect with the VFX Manager
        var smokeEffect = GetComponentInChildren<ParticleSystem>();
        if (smokeEffect != null)
        {
            VFXManager.Instance.RegisterVFX(partName, smokeEffect);
        }
        TakeDamage(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damageable"))
        {
            Debug.Log("COLLIIDIDNG");
            if (planeRb != null)
            {
                float impactSpeed = planeRb.velocity.magnitude;
                float damageAmount = impactSpeed * damageFactor;
                TakeDamage(damageAmount);
            }
        }
    }


    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        //currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth < 75f)
        {
            VFXManager.Instance.PlayVFX(partName);
        }
        else
        {
            VFXManager.Instance.StopVFX(partName);
        }
        //Debug.Log("Current health of " + partName + ": " + currentHealth);
        CurrentHealth = currentHealth;
    }

    public void Repair(float repairAmount)
    {
        currentHealth += repairAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth >= 75f)
        {
            VFXManager.Instance.StopVFX(partName);
        }
        CurrentHealth = currentHealth;
    }

    public void PartUpgrade(int upgradeAmount)
    {
        upgradePower += upgradeAmount;
        UpgradePower = upgradePower;
    }
}
