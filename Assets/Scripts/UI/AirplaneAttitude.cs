using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneAttitude : MonoBehaviour, IAirplaneUI
{
    public AirplaneController airplane;
    public RectTransform bgRect;
    public RectTransform arrowRect;
    

    public void HandleAirplaneUI()
    {
        if (airplane)
        {
            float bankAngle = Vector3.Dot(airplane.transform.right, Vector3.up) * Mathf.Rad2Deg;
            //Debug.Log(bankAngle);
            float pitchAngle = Vector3.Dot(airplane.transform.forward, Vector3.up) * Mathf.Rad2Deg;

            if (bgRect)
            {
                Quaternion bankRot = Quaternion.Euler(0f, 0f, bankAngle);
                bgRect.transform.rotation = bankRot;

                Vector3 wantedPosition = new Vector3(0f, -pitchAngle, 0f);
                bgRect.anchoredPosition = wantedPosition;

                if (arrowRect)
                {
                    arrowRect.transform.rotation = bankRot;
                }
            }
        }
    }
}
