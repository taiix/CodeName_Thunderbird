using UnityEngine;

public class WaterAudio : MonoBehaviour
{
    private AudioSource audioSource;
    private GameObject player;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogError("No player found");
            return;
        }

        float dist = Vector3.Distance(this.transform.position, player.transform.position);

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
}
