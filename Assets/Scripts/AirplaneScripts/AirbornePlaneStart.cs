using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AirbornePlaneStart : MonoBehaviour
{
    public float initialAltitude = 1000f; 
    public float initialSpeed = 300f; 
    public float initialThrottle = 1.0f; 
    public float initialRPM = 3200f; 
    public float initialPitchAngle = 5f; 

    public Rigidbody planeRigidbody;
    public AirplaneAerodynamics airplaneAerodynamics;
    public AirplaneEngine airplaneEngine;
    public BaseAirplaneInputs airplaneInputs;

    [SerializeField]private Transform startPos;


    void Start()
    {
        SetPlane();
    }

    void Update()
    {
   
    }

   

    void SetPlane()
    {
        startPos.position = new Vector3(transform.position.x, initialAltitude, transform.position.z);
        startPos.rotation = Quaternion.Euler(initialPitchAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        // Set the plane's initial position and rotation
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;

        // Set the Rigidbody velocity for initial speed
        if (planeRigidbody)
        {
            planeRigidbody.velocity = transform.forward * initialSpeed;
        }

        // Set airspeed in the aerodynamics system
        if (airplaneAerodynamics)
        {
            airplaneAerodynamics.kph = initialSpeed;
        }

        // Set engine RPM
        if (airplaneEngine)
        {
            airplaneEngine.CurrentRPM = initialRPM;
        }

        // Set throttle to maximum
        if (airplaneInputs)
        {
            airplaneInputs.StickyThrottle = initialThrottle;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the plane collides with water
        if (other.gameObject.CompareTag("WaterSurface"))
        {
            SetPlane();
        }
    }

}
