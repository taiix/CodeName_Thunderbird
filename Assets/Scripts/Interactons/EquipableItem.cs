using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipableItem : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] InputActionAsset actions;
    InputAction hitAction;
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
        if(actions != null)
        {
            hitAction = actions.FindAction("Hit");
        }
        hitAction.performed += PerformHit;
    }

    private void OnDisable()
    {
        hitAction.performed -= PerformHit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PerformHit(InputAction.CallbackContext context)
    {
        Debug.Log("Hit");
        if (animator == null) return;
        if (hitAction.WasPressedThisFrame())
        {
            animator.SetTrigger("hit");
        }
    }
}
