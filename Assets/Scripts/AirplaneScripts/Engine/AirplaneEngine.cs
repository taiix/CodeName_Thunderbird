using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneEngine : MonoBehaviour
{
    public float maxForce = 200;
    public float maxRPM = 2550;

    public AnimationCurve powerCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public Vector3 CalculateForce(float throttle)
    {
        float finalThrottle = Mathf.Clamp01(throttle);

        finalThrottle = powerCurve.Evaluate(finalThrottle);

        float finalPower = finalThrottle * maxForce;
        
        Vector3 finalForce = transform.forward * finalPower;

        return finalForce;
    }
}
