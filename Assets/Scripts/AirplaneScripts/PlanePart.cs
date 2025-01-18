using System.Collections.Generic;
using UnityEngine;

public class PlanePart : MonoBehaviour, ISavableData
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 50;
    private Rigidbody planeRb;
    [SerializeField] private float damageFactor = 1f;


    public string partName;
    [SerializeField] public bool IsDamaged => currentHealth < maxHealth;

    public float CurrentHealth { get; private set; }

    public bool setCurrentHealth = false;

    private int upgradePower = 0;
    public int UpgradePower { get; private set; }

    [SerializeField] public List<PartUpgrade> upgrades = new List<PartUpgrade>();
    public int currentUpgradeLevel = 0;


    private ParticleSystem partVFX;

    private void Start()
    {
        partVFX = GetComponentInChildren<ParticleSystem>();
        planeRb = GetComponentInParent<Rigidbody>();
        if (!setCurrentHealth)
        {
            currentHealth = maxHealth;
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
                partVFX.Play();
            }
            else
            {
                partVFX.Stop();
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
        if (HasItemsForFix())
        {
            for(int i = 0; i <= upgrades[currentUpgradeLevel].itemsForFix.Count - 1; i++)
            {
                InventorySystem.Instance?.RemoveItem(upgrades[currentUpgradeLevel].itemsForFix[i].item, 
                                                    upgrades[currentUpgradeLevel].itemsForFix[i].amount);
            }
            currentHealth += repairAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            if (currentHealth >= 75f)
            {
                partVFX.Stop();
                PlayerQuest.OnQuestCompleted?.Invoke();
            }
            CurrentHealth = currentHealth;
        }
        else
        {
            Debug.Log("Dont't have enough items to fix");
        }
    }


    public bool HasItemsForFix()
    {
        for(int i = 0; i < upgrades[currentUpgradeLevel].itemsForFix.Count; i++)
        {
            if (!InventorySystem.Instance.HasRequiredItem(upgrades[i].itemsForFix[i].item, upgrades[i].itemsForFix[i].amount))
            {
                return false;
            }
        }
        return true;
    }

    public void PartUpgrade(PartUpgrade upgrade)
    {
        upgradePower = upgrade.upgradePower;
        if(partName == "Engine")
        {
            Debug.Log("Upgrade the engine");
        }
        this.maxHealth += upgrade.healthUpgrade;
        this.damageFactor -= upgrade.damageReduction;
        GameManager.Instance.GetAirplaneAerodynamics().maxLiftPower += upgradePower;
        currentUpgradeLevel++;
        Debug.Log(currentUpgradeLevel);
        Debug.Log(upgrades.Count);

        UpgradePower = upgradePower;
    }
    public PartUpgrade GetCurrentUpgrade()
    {
        if (currentUpgradeLevel < upgrades.Count )
        {
            return upgrades[currentUpgradeLevel];
        }
        else
        {
            return null;
        }
    }

    public void ResetHealth()
    {
        currentHealth = 100;
        TakeDamage(0);
    }
    public string ToJson()
    {
        PlaneData data = new PlaneData
            (currentHealth, planeRb.gameObject.transform.position.x, planeRb.gameObject.transform.position.y, planeRb.gameObject.transform.position.z);

        return JsonUtility.ToJson(data, true);
    }

    public void FromJson(string json)
    {
        PlaneData data = JsonUtility.FromJson<PlaneData>(json);
        currentHealth = data.planeDurability;
        planeRb.gameObject.transform.position = new Vector3(data.posX, data.posY, data.posZ);
    }
}