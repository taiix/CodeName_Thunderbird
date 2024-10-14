using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanePart : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 50;
    [SerializeField] private Rigidbody planeRb;
    [SerializeField] private float damageFactor = 1f;


    public string partName;
    public bool IsDamaged => currentHealth < maxHealth;

    public float CurrentHealth { get; private set; }

    public bool setCurrentHealth = false;

    private int upgradePower = 0;
    public int UpgradePower { get; private set; }

    [SerializeField] public List<PartUpgrade> upgrades = new List<PartUpgrade>();

    public int currentUpgradeLevel = 0;

    private void Start()
    {
        planeRb = GetComponentInParent<Rigidbody>();
        if (!setCurrentHealth)
        {
            currentHealth = maxHealth;
        }

        // Register the smoke effect with the VFX Manager
        var smokeEffect = GetComponentInChildren<ParticleSystem>();
        if (smokeEffect != null && VFXManager.Instance != null)
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
        CurrentHealth = currentHealth;
        //currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (VFXManager.Instance != null)
        {
            if (currentHealth < 75f)
            {
                VFXManager.Instance.PlayVFX(partName);
            }
            else
            {
                VFXManager.Instance.StopVFX(partName);
            }
        }
        else
        {
            Debug.Log("No VFX Manager in scene, can't play VFX");
        }

        //Debug.Log("Current health of " + partName + ": " + currentHealth);
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

    public void PartUpgrade(PartUpgrade upgrade)
    {
        this.maxHealth += upgrade.upgradePower;
        GameManager.Instance.GetAirplaneAerodynamics().maxLiftPower += upgradePower;
        currentUpgradeLevel++;

        UpgradePower = upgradePower;
    }

    public PartUpgrade GetCurrentUpgrade()
    {
        if (currentUpgradeLevel < upgrades.Count)
        {
            return upgrades[currentUpgradeLevel];
        }
        else
        {
            return null;
        }
    }
}
