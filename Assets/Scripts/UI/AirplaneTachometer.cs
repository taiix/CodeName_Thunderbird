using UnityEngine;

public class AirplaneTachometer : MonoBehaviour, IAirplaneUI
{
    public AirplaneEngine engine;
    public RectTransform pointer;
    public float maxRPM = 3500f;
    public float maxRotation = 312;
    public float pointerSpeed = 2f;

    private float finalRotation = 0f; 

    public void HandleAirplaneUI()
    {
        if (engine && pointer)
        {
            float normalizedRPM = Mathf.InverseLerp(0f, maxRPM, engine.CurrentRPM);

            float wantedRotation = maxRotation * -normalizedRPM;
            finalRotation = Mathf.Lerp(finalRotation, wantedRotation, Time.deltaTime * pointerSpeed); 
            pointer.rotation = Quaternion.Euler(0f, 0f, finalRotation);
        }
    }

    public float GetRotation() { 
        return pointer.rotation.eulerAngles.z;
    }
}
