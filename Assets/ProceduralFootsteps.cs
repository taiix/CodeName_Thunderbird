using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GrassFootsteps : MonoBehaviour
{
    [Range(0.1f, 1f)]
    public float footstepDuration = 0.3f; // Duration of each footstep
    [Range(0.1f, 1f)]
    public float footstepInterval = 0.5f; // Time between footsteps
    [Range(0.1f, 1f)]
    public float rustleVolume = 0.6f; // Volume of the rustling noise
    [Range(0.1f, 1f)]
    public float crunchVolume = 0.4f; // Volume of the crunch component

    private AudioSource audioSource;
    private float[] footstepData;
    private int sampleRate = 44100;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(FootstepRoutine());
    }

    private IEnumerator FootstepRoutine()
    {
        while (true)
        {
            PlayFootstep();
            yield return new WaitForSeconds(footstepInterval);
        }
    }

    private void PlayFootstep()
    {
        // Generate footstep audio data for grass
        footstepData = GenerateGrassFootstep(footstepDuration, rustleVolume, crunchVolume);
        AudioClip clip = AudioClip.Create("GrassFootstep", footstepData.Length, 1, sampleRate, false);
        clip.SetData(footstepData, 0);
        audioSource.PlayOneShot(clip);
    }

    private float[] GenerateGrassFootstep(float duration, float rustleVol, float crunchVol)
    {
        int sampleLength = Mathf.CeilToInt(sampleRate * duration);
        float[] data = new float[sampleLength];

        for (int i = 0; i < sampleLength; i++)
        {
            float time = (float)i / sampleRate;

            // Rustling noise (random, decaying over time)
            float rustle = Random.Range(-1f, 1f) * Mathf.Exp(-time * 10) * rustleVol;

            // Crunch noise (short bursts)
            float crunch = Mathf.Sin(2 * Mathf.PI * Random.Range(500f, 1000f) * time) * Mathf.Exp(-time * 20) * crunchVol;

            // Combine rustling and crunch
            data[i] = rustle + crunch;
        }

        return data;
    }
}
