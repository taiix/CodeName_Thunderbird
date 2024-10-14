using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableItem : MonoBehaviour
{
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<Animator>() != null)
        {
            animator = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (animator == null) return;
        if ((Input.GetKeyDown(KeyCode.Mouse0)))
        {
            animator.SetTrigger("hit");
        }
    }
}
