using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneEngine : MonoBehaviour
{
    public float maxForce = 200;
    public float maxRPM = 2550;
    public float healthThreshold = 50;

    public AnimationCurve powerCurve = AnimationCurve.Linear(0, 0, 1, 1);

    PlanePart engine;


    private void Start()
    {
        engine = GetComponent<PlanePart>();
    }

    bool CanGenerateForwardSpeed()
    {
        if (engine != null)
        {
            return engine.CurrentHealth > healthThreshold;
        }
        return false;
    }

    public Vector3 CalculateForce(float throttle)
    {
        if (CanGenerateForwardSpeed())
        {

            float finalThrottle = Mathf.Clamp01(throttle);

            finalThrottle = powerCurve.Evaluate(finalThrottle);

            float finalPower = finalThrottle * maxForce;

            Vector3 finalForce = transform.forward * finalPower;

            return finalForce;
        }
        return Vector3.zero;
    }
}
