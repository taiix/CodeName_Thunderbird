using UnityEngine;

public class PlaneEngineAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource engineSound;
    [SerializeField] private AirplaneAerodynamics airplane;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2.0f;


    private AudioSource audioSource;
    private GameObject player;

    private void Start()
    {
        if (airplane == null) Debug.LogError("AirplaneAerodynamics is null. It cannot calculate the speed of the plane. Add the component or no sound for you");

        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        EngineSound();

        float dist = Vector3.Distance(this.transform.position, Camera.main.transform.position) - audioSource.maxDistance;

        if (dist < audioSource.maxDistance)
        {
            float normalizedDist = dist / audioSource.maxDistance;
            float volume = 1 - Mathf.Clamp01(normalizedDist);

            audioSource.volume = volume;
        }
        else
        {
            audioSource.volume = 0;
        }
    }

    void EngineSound()
    {
        if (airplane != null && engineSound != null)
        {
            float normalizedSpeed = airplane.kph / airplane.maxKph;
            engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);
        }
    }
}
