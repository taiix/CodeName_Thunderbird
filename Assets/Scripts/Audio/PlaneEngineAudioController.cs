using UnityEngine;

public class PlaneEngineAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AirplaneAerodynamics airplane;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2.0f;

    private void Start()
    {
        if (airplane == null) Debug.LogError("AirplaneAerodynamics is null. It cannot calculate the speed of the plane. Add the component or no sound for you");
    }

    void Update()
    {
        EngineSound();
    }

    void EngineSound() {
        if (airplane != null && engineSound != null)
        {
            float normalizedSpeed = airplane.kph / airplane.maxKph;
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        }
    }
}
