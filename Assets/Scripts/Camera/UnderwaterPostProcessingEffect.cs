using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UnderwaterPostProcessingEffect : MonoBehaviour
{
    [Header("Attach this to the main camera")]
    [Space]


    [SerializeField] private PostProcessVolume globalVolume;

    [SerializeField] private PostProcessProfile underwaterEffectProfile;
    [SerializeField] private PostProcessProfile globalProfile;

    [SerializeField] private bool isActive;
    List<Transform> waterSurface;
    private void Start()
    {
        
    }

    void Update()
    {
        float cameraHeightPosition = this.transform.position.y;

        if (cameraHeightPosition < 8f) isActive = true;
        else isActive = false;

        if (isActive)
        {
            globalVolume.profile = underwaterEffectProfile;
        }
        else
        {
            globalVolume.profile = globalProfile;
        }
    }
}
