using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{

    private class VFXEntry
    {
        public ParticleSystem vfx;
        public Transform location;

        public VFXEntry(ParticleSystem vfx, Transform location)
        {
            this.vfx = vfx;
            this.location = location;
        }
    }

    private Dictionary<string, VFXEntry> vfxDictionary = new Dictionary<string, VFXEntry>();

    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void RegisterVFX(string name, ParticleSystem vfx, Transform location = null)
    {
        if (!vfxDictionary.ContainsKey(name))
        {
            ParticleSystem instantiatedVFX = Instantiate(vfx);
            instantiatedVFX.gameObject.SetActive(false); 
            vfxDictionary.Add(name, new VFXEntry(instantiatedVFX, location));
        }
    }

    public void PlayVFX(string name)
    {
        if (vfxDictionary.ContainsKey(name))
        {
            VFXEntry entry = vfxDictionary[name];

            // Set position if location is specified
            if (entry.location != null)
            {
                //Debug.Log("Playing VFX: " + name + " at location " + entry.location.position);
                entry.vfx.transform.position = entry.location.position;
            }

            entry.vfx.gameObject.SetActive(true);
            if (!entry.vfx.isPlaying)
            {
                entry.vfx.Play();
            }
        }
        else
        {
            Debug.LogWarning("VFX with name "  + name + " not found.");
        }
    }

    public void StopVFX(string name)
    {
        if (vfxDictionary.ContainsKey(name))
        {
            VFXEntry entry = vfxDictionary[name];
            if (entry.vfx.isPlaying)
            {
                entry.vfx.Stop();
                entry.vfx.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("VFX with name " + name + " not found.");
        }
    }
}
