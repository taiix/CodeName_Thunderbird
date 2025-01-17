using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarrelRollHandler : MonoBehaviour
{
    [SerializeField] BarrelRollDetector barrelRollDetector;

    [SerializeField] TextMeshProUGUI barrelRollText;

    int barrelRollCounter = 0;

    void Start()
    {
        barrelRollText.text = "Barrel rolls completed 0/3";
        //barrelRollDetector = GetComponent<BarrelRollDetector>();
        if (barrelRollDetector != null)
        {
            barrelRollDetector.OnBarrelRollCompleted += HandleBarrelRoll;
        }
    }

    private void HandleBarrelRoll()
    {
        barrelRollCounter++;
        barrelRollText.text = "Barrel rolls completed " + barrelRollCounter + "/3";

        if(barrelRollCounter >= 3)
        {
            barrelRollText.text = "Good job! You are now a pilot!";
        }
        Debug.Log("Barrel roll detected! Perform some action here.");
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (barrelRollDetector != null)
        {
            barrelRollDetector.OnBarrelRollCompleted -= HandleBarrelRoll;
        }
    }
}
