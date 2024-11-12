using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float dayDuration = 120f; 
    private float time; 

    [Header("Sun Settings")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Skybox Settings")]
    public Material skyboxMaterial;

    void Start() { 
        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        time += (Time.deltaTime / dayDuration);
        if (time >= 1)
            time = 0;

        sun.transform.localRotation = Quaternion.Euler((time * 360f) - 90f, 170f, 0);

        sun.intensity = sunIntensity.Evaluate(time);
        sun.color = sunColor.Evaluate(time);

        skyboxMaterial.SetFloat("_TimeOfDay", time);
    }
}
