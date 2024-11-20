using JetBrains.Annotations;
using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [SerializeField] private float sunriseHour = 8; //lets say 8 am 
    [SerializeField] private float sundownHour = 20; //lets say 20 pm

    [SerializeField] private float timeMultiplier;

    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] private Light sun;
    [SerializeField] private Light moon;

    [SerializeField] private float maxSunIntensity;
    [SerializeField] private float maxMoonIntensity;

    [SerializeField] private AnimationCurve lightCurve;

    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;

    private DateTime currentTime;

    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    //PUBLIC
    public DateTime CurrentTime => currentTime;
    public Light Sun => sun;

    private void Start()
    {
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(sunriseHour);

        sunriseTime = TimeSpan.FromHours(sunriseHour);
        sunsetTime = TimeSpan.FromHours(sundownHour);
    }

    private void Update()
    {
        TimeOfDay();
        SunRotation(); 
        LightSettings();
    }

    void TimeOfDay()
    {
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);
        if (timeText != null)
            timeText.text = currentTime.ToString("HH:mm");
    }

    void SunRotation()
    {
        float sunRot;

        bool isDayTime = currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime;

        if (isDayTime)
        {
            float dayLenght = (float)(sunsetTime - sunriseTime).TotalMinutes;
            float timeElapsed = (float)(currentTime.TimeOfDay - sunriseTime).TotalMinutes;
            sunRot = Mathf.Lerp(0, 180, timeElapsed / dayLenght);
        }
        else
        {
            float nightLenght = (float)(sunriseTime - sunsetTime).TotalMinutes;
            float timeElapsed = (float)(currentTime.TimeOfDay - sunsetTime).TotalMinutes;
            sunRot = Mathf.Lerp(180, 360, timeElapsed / nightLenght);
        }
        sun.transform.rotation = Quaternion.AngleAxis(sunRot, Vector3.right);
    }

    void LightSettings()
    {
        float dotProduct = Vector3.Dot(sun.transform.forward, Vector3.down);

        sun.intensity = Mathf.Lerp(0, maxSunIntensity, lightCurve.Evaluate(dotProduct));

        moon.intensity = Mathf.Lerp(maxMoonIntensity, 0, lightCurve.Evaluate(dotProduct));

        RenderSettings.ambientLight = Color.Lerp(nightAmbientLight, dayAmbientLight, lightCurve.Evaluate(dotProduct));
    }


}


