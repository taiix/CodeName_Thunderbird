using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeWindEffect : MonoBehaviour
{
    public Transform crown; 
    public float swayStrength = 1f; 
    public float swaySpeed = 1f; 
    public Vector3 windDirection = Vector3.right; 
    public bool useRotation = true; 
    public bool usePosition = false; 

    private Vector3 initialPosition;

    private void Start()
    {
        if (crown == null)
        {
            Debug.LogError("Crown Transform is not assigned.");
            return;
        }

        initialPosition = crown.localPosition; 
    }

    private void Update()
    {
        SimulateWind();
    }

    private void SimulateWind()
    {
        if (crown == null) return;

     
        float swayFactor = Mathf.Sin(Time.time * swaySpeed) * swayStrength;

        if (useRotation)
        {
          
            Vector3 swayRotation = new Vector3(0, 0, swayFactor);
            crown.localRotation = Quaternion.Euler(swayRotation);
        }

        if (usePosition)
        {
           
            Vector3 swayOffset = windDirection.normalized * swayFactor;
            crown.localPosition = initialPosition + swayOffset;
        }
    }
}
