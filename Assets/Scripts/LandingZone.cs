using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingZone : MonoBehaviour
{
    [SerializeField] private Island parentIsland; 

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collider with " + other.name);
        if (other.CompareTag("Plane")) 
        {
            Debug.Log("Plane landed in the zone on island: " + parentIsland.name);
            parentIsland.MarkAsLastIsland();
        }
    }
}
