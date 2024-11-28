using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipableItem : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] InputActionAsset actions;
    InputAction hitAction;

    [SerializeField] int damageAmount = 2;
    public List<GameObject> enemiesInRange = new List<GameObject>();

    private Collider itemCollider;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<Animator>() != null)
        {

            animator = GetComponent<Animator>();
        }
    }


    private void OnEnable()
    {
        //Assign the hitAction and subscribe to it
        if (actions != null)
        {
            hitAction = actions.FindAction("Hit");
        }
        hitAction.performed += TriggerAnimation;
    }

    private void OnDisable()
    {
        hitAction.performed -= TriggerAnimation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            GetComponent<Collider>().enabled = false;
        }
    }

    private void TriggerAnimation(InputAction.CallbackContext context)
    {
        if (InventorySystem.Instance.IsInventoryOpen()) return;
        animator.SetTrigger("hit");
    }

    public void PerformHit()
    {
        Debug.Log("Hit");
        if (animator == null) return;
        if (!InventorySystem.Instance.IsInventoryOpen())
        {
            // Handle tree interaction
            TreeInteractable treeInteractable = InteractionHandler.Instance.treeInteractable;
            Item equippedItem = InventorySystem.Instance.GetEquippedItem();


            // Handle enemy damage
            GetComponent<Collider>().enabled = true;
            if (equippedItem != null && equippedItem.type == Item.Types.axe && enemiesInRange.Count > 0)
            {
                foreach (GameObject enemy in enemiesInRange)
                {
                    if (enemy != null)
                    {
                        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
                        if (enemyAI != null)
                        {
                            Debug.Log("Do dmg to enemy");
                            enemyAI.TakeDamage(damageAmount);
                        }
                    }
                }
                enemiesInRange.Clear();
            }
            else if (treeInteractable != null && equippedItem != null && equippedItem.type == Item.Types.axe)
            {
                treeInteractable.GetHit();
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Detected: {other.name}");

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy in range");
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null && !enemiesInRange.Contains(other.gameObject))
            {
                enemiesInRange.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exited trigger: " + other.name);

        if (enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }
}
