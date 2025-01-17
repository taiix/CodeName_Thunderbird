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

    public bool isCompleted = false;

    private void OnEnable()
    {
        barrelRollText.gameObject.SetActive(true);
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

        if (barrelRollCounter >= 3)
        {
            isCompleted = true;
        }
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
