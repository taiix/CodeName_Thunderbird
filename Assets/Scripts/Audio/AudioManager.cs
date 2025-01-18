using UnityEngine;
using UnityEngine.Events;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public UnityAction<int, float> onPlayFootstep { get; private set; }
    public UnityAction onPlayAxeHit { get; private set; }

    [SerializeField] private SoundEffects[] sources;
    [SerializeField] private AudioClip[] axeSoundsSources;

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
        onPlayAxeHit += PlayAxeHit;
    }
    private void OnDestroy()
    {
        onPlayFootstep -= PlayFootsteps;
        onPlayAxeHit -= PlayAxeHit;
    }

    public void PlayAxeHit()
    {
        AudioClip clip = axeSoundsSources[Random.Range(0, axeSoundsSources.Length)];
        OneshotAudioFX(clip, 1, false);
    }

    void PlayFootsteps(int id, float interval)
    {
        SoundEffects sf = GetSoundEffectByID(id);

        if (sf.audio.Length > 0)
        {
            AudioClip footStep = sf.audio[Random.Range(0, sf.audio.Length)];
            OneshotAudioFX(footStep, interval, true);
        }
        else Debug.LogWarning($"No audio found at {id}");

    }

    public void OneshotAudioFX(AudioClip clip, float interval, bool hasPitch = false)
    {
        if (!audioSource.isPlaying)
        {
            if (hasPitch)
            {
                audioSource.pitch = interval / 5;
            }
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
