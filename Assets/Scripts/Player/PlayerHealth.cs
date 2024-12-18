using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, ISavableData
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthBar;

    public static UnityAction<float> OnPlayerDamaged;
    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    private void OnEnable()
    {
        OnPlayerDamaged += TakeDamage;
    }

    private void OnDestroy()
    {
        OnPlayerDamaged -= TakeDamage;
    }

    private void Update()
    {
        if (currentHealth <= 0) PlayerDead();
        //if (Input.GetKeyDown(KeyCode.F)) TakeDamage(10f);
    }

    void PlayerDead()
    {
        Debug.Log("player is dead");
        SceneManager.LoadScene(0);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthBar();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthBar();
    }
    private void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.value = currentHealth / maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public string ToJson()
    {
        PlayerData data = new PlayerData
            (currentHealth, transform.position.x, transform.position.y, transform.position.z);
        
        return JsonUtility.ToJson(data, true);
    }

    public void FromJson(string json)
    {
        PlayerData data = JsonUtility.FromJson<PlayerData>(json);
        currentHealth = data.playerHealth;
        transform.position = new Vector3(data.posX, data.posY, data.posZ);
        UpdateHealthBar();
    }
}
