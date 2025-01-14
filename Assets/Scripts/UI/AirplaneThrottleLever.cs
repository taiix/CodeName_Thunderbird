using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AirplaneThrottleLever : MonoBehaviour, IAirplaneUI
{

    public BaseAirplaneInputs inputs;
    public RectTransform parentRect;
    public RectTransform handleRect;
    public float handleSpeed = 2f;

    public void HandleAirplaneUI()
    {
        if (inputs && parentRect && handleRect)
        {
            float height = parentRect.rect.height;
            Vector2 wantedHandlePosition = new Vector2(0f, height * inputs.StickyThrottle);
            handleRect.anchoredPosition = Vector2.Lerp(handleRect.anchoredPosition, wantedHandlePosition, Time.deltaTime * handleSpeed);

        }

    }
}
