using JetBrains.Annotations;
using UnityEngine;

public class AirplaneEngine : MonoBehaviour
{
    public float maxForce = 200;
    public float maxRPM = 2550;
    public float healthThreshold = 50;

    public AnimationCurve powerCurve = AnimationCurve.Linear(0, 0, 1, 1);

    PlanePart engine;

    private bool isEngineEnabled = true;

    private float currentRPM;

    public float CurrentRPM
    {
        get { return currentRPM;}
        set { currentRPM = Mathf.Clamp(value, 0, maxRPM); }
    }

    private void Start()
    {
        engine = GetComponent<PlanePart>();
    }

    bool CanGenerateForwardSpeed()
    {
        if (engine)
        {
            return engine.CurrentHealth > healthThreshold;
        }
        return false;
    }

    public Vector3 CalculateForce(float throttle)
    {
        float finalThrottle = Mathf.Clamp01(throttle);

        if (isEngineEnabled && CanGenerateForwardSpeed())
        {

            finalThrottle = powerCurve.Evaluate(finalThrottle);

            float finalPower = finalThrottle * maxForce;

            Vector3 finalForce = transform.forward * finalPower;

            currentRPM = finalThrottle * maxRPM;

            return finalForce;
        }
        currentRPM = 0;
        return Vector3.zero;
    }

    public void DisableEngine()
    {
        isEngineEnabled = false;
    }

    public void EnableEngine()
    {
        isEngineEnabled = true;
    }
}
