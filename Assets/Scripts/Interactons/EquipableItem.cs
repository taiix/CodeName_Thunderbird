using UnityEngine;
using UnityEngine.InputSystem;

public class EquipableItem : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] InputActionAsset actions;
    InputAction hitAction;

    [SerializeField] int damageAmount = 2;

    private Collider itemCollider;

    private Item equippedItem;
    private Vector3 hitPoint;
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
            //GetComponent<Collider>().enabled = false;
        }
    }

    private void TriggerAnimation(InputAction.CallbackContext context)
    {
        if (InventorySystem.Instance.IsInventoryOpen()) return;
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;
        animator.SetTrigger("hit");
    }

    public void PerformHit()
    {
        //Debug.Log("Perform Hit");
       
        if (animator == null) return;
        if (!InventorySystem.Instance.IsInventoryOpen())
        {
            // Handle tree interaction
            TreeInteractable treeInteractable = InteractionHandler.Instance.treeInteractable;
            equippedItem = InventorySystem.Instance.GetEquippedItem();
            Debug.Log(equippedItem.itemName);
            if (treeInteractable != null && equippedItem != null && equippedItem.type == Item.Types.axe)
            {
                treeInteractable.GetHit();
            }
        }
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (equippedItem.type == Item.Types.axe && collision.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Hitting enemy");
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            hitPoint = collision.GetContact(0).point;
            enemyAI.TakeDamage(damageAmount, hitPoint);
        }

    }
}
