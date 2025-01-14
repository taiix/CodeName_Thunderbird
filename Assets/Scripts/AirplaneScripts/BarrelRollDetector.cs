using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRollDetector : MonoBehaviour
{
    public float rollThreshold = 360f; 
    public bool debugMode = false; 

    private float previousRollAngle = 0f;
    private float rollProgress = 0f;

    public delegate void BarrelRollAction();
    public event BarrelRollAction OnBarrelRollCompleted;

    void Update()
    {
        DetectBarrelRoll();
    }

    private void DetectBarrelRoll()
    {
        float rollAngle = transform.eulerAngles.z;
        float deltaAngle = Mathf.DeltaAngle(previousRollAngle, rollAngle);
        rollProgress += deltaAngle;

        if (Mathf.Abs(rollProgress) >= rollThreshold)
        {
            if (debugMode)
                Debug.Log("Barrel roll completed!");

            rollProgress = 0f;

            OnBarrelRollCompleted?.Invoke();
        }

        previousRollAngle = rollAngle;
    }
}
