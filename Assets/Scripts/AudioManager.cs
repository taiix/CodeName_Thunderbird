using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public UnityAction<int, float> onPlayFootstep { get; private set; }

    [SerializeField] private SoundEffects[] sources;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance.gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        onPlayFootstep += PlayFootsteps;
    }

    void PlayFootsteps(int id, float interval)
    {
        SoundEffects sf = GetSoundEffectByID(id);

        if (sf.audio.Length > 0)
        {
            AudioClip footStep = sf.audio[Random.Range(0, sf.audio.Length)];
            Footsteps(footStep, interval);
        }
        else Debug.LogWarning($"No audio found at {id}");

    }

    void Footsteps(AudioClip clip, float interval) {
        if (!audioSource.isPlaying)
        {
            audioSource.pitch = interval / 5;
            audioSource.PlayOneShot(clip);
        }
    }

    SoundEffects GetSoundEffectByID(int id)
    {
        foreach (SoundEffects effect in sources)
        {
            if (effect.id == id)
            {
                return effect;
            }
        }

        Debug.LogWarning($"No SoundEffects found for ID {id}.");
        return new SoundEffects(); // Return an empty struct if no match found
    }
}

[System.Serializable]
public struct SoundEffects
{
    public int id;
    public string audioName;
    public AudioClip[] audio;
}
