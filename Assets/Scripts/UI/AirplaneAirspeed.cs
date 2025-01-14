using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneAirspeed : MonoBehaviour, IAirplaneUI
{

    public AirplaneAerodynamics airplaneAerodynamics;
    public RectTransform pointer;
    public float maxIndicatedKnots = 160f;

    public const float kphToKnts = 0.539957f;
    public void HandleAirplaneUI()
    {
       if(airplaneAerodynamics && pointer)
        {
            float currentKnots = airplaneAerodynamics.kph * kphToKnts;
            float normalizedKnots = Mathf.InverseLerp(0, maxIndicatedKnots, currentKnots);
            float wantedRotation = 360f * normalizedKnots;
            pointer.rotation = Quaternion.Euler(0f, 0f, -wantedRotation);
        }
    }
}
