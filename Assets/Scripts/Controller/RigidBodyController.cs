using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyController : MonoBehaviour
{

    public Rigidbody rb;
    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb)
        {
            HandlePhysics();
        }
    }

    protected virtual void HandlePhysics()
    {

    }
}
