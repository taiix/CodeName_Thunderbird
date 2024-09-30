using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WheelCollider))]
public class AirplaneWheels : MonoBehaviour
{


    private WheelCollider wheelCollider;

    public float brakePower = 4f;
    public bool isBreaking = false;
    private void Start()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    public void InitializeWheel()
    {
        if (wheelCollider)
        {
            //Make sure the collider stays active 
            wheelCollider.motorTorque = 0.000000000001f;
        }
    }

    public void HandleWheel(BaseAirplaneInputs inputs)
    {
        if(wheelCollider)
        {
            // Reset motorTorque if the plane is nearly stopped
            if (wheelCollider.rpm < 1f)
            {
                wheelCollider.motorTorque = 0;
            }

            if(wheelCollider.rpm > 1f)
            {
                wheelCollider.motorTorque = 0.000000000001f;
            }

            if (inputs.Break > 0f)
            {
                wheelCollider.brakeTorque = inputs.Break * brakePower;
            }
            else
            {
                wheelCollider.brakeTorque = 0f;
               
            }
        }
    }
}
