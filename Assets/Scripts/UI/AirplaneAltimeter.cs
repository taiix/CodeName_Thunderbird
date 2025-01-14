using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneAltimeter : MonoBehaviour, IAirplaneUI
{
    public AirplaneController airplane;
    public RectTransform hundredsPointer;
    public RectTransform thousandsPointer;

    private float altitude;

    public void HandleAirplaneUI()
    {
        if (airplane)
        {
            float currentAltitude = airplane.CurrentMSL;

            float currentThousands = currentAltitude / 1000f;
            currentThousands = Mathf.Clamp(currentThousands,0f, 10f);

            float currentHundreds = currentAltitude - (Mathf.Floor(currentThousands) * 1000f);
            currentHundreds = Mathf.Clamp(currentHundreds, 0f, 1000f);

            if (thousandsPointer)
            {
                float normalizedThousands = Mathf.InverseLerp(0f, 10f, currentThousands);
                float thousandsRotation = 360f * normalizedThousands;
                thousandsPointer.rotation = Quaternion.Euler(0f, 0f, -thousandsRotation);
            }

            if (hundredsPointer)
            {
                float normalizedHundreds = Mathf.InverseLerp(0f, 1000f, currentHundreds);
                float hundredsRotation = 360 * normalizedHundreds;
                hundredsPointer.rotation = Quaternion.Euler(0f, 0f, -hundredsRotation);
            }

            altitude = currentAltitude;
        }
    }

    public float GetAltitude() { return altitude; }
}
