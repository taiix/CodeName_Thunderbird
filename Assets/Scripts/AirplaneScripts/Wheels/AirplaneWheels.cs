using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(WheelCollider))]
public class AirplaneWheels : MonoBehaviour
{
    private WheelCollider wheelCollider;
    public float brakePower = 4f;
    public float handBrakePower = 1000f;
    public bool isBreaking = false;
    private bool isPlayerInPlane = false;
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

    public void HandleWheel(BaseAirplaneInputs inputs, bool playerInPlane)
    {
        isPlayerInPlane = playerInPlane;

        if (wheelCollider)
        {
            // Apply handbrake when the player is not in the plane
            if (!isPlayerInPlane)
            {
                wheelCollider.brakeTorque = handBrakePower;
                wheelCollider.motorTorque = 0; // Ensure no torque is applied
                return; // Skip the rest of the update if the player isn't in the plane
            }

            // Reset motorTorque if the plane is nearly stopped
            if (wheelCollider.rpm < 1f)
            {
                wheelCollider.motorTorque = 0;
            }

            if (wheelCollider.rpm > 1f)
            {
                wheelCollider.motorTorque = 0.000000000001f;
            }

            // Apply normal brakes if the player is in the plane and using the brakes
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
