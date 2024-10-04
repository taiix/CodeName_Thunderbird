using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{

    private Dictionary<string, ParticleSystem> vfxDictionary = new Dictionary<string, ParticleSystem>();

    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void RegisterVFX(string partName, ParticleSystem vfx)
    {
        if (!vfxDictionary.ContainsKey(partName))
        {
            vfxDictionary.Add(partName, vfx);
        }
    }

    public void PlayVFX(string partName)
    {
        if (vfxDictionary.ContainsKey(partName) && !vfxDictionary[partName].isPlaying)
        {
            vfxDictionary[partName].Play();
        }
    }

    public void StopVFX(string partName)
    {
        if (vfxDictionary.ContainsKey(partName) && vfxDictionary[partName].isPlaying)
        {
            vfxDictionary[partName].Stop();
        }
    }
}
