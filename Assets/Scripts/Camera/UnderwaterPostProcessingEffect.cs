using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Camera), typeof(PostProcessVolume), typeof(PostProcessLayer))]
public class UnderwaterPostProcessingEffect : MonoBehaviour
{
    [Header("Attach this to the main camera")]
    [Space]
    private PostProcessVolume globalVolume;

    [SerializeField] private PostProcessProfile underwaterEffectProfile;
    [SerializeField] private PostProcessProfile globalProfile;

    private bool isActive;

    [SerializeField] private Transform waterSurface;

    private void Start()
    {
        globalVolume = GetComponent<PostProcessVolume>();
    }

    void Update()
    {
        float cameraHeightPosition = this.transform.position.y;

        if (cameraHeightPosition < waterSurface.position.y) isActive = true;
        else isActive = false;

        if (isActive)
        {
            RenderSettings.fog = true;
            globalVolume.profile = underwaterEffectProfile;
        }
        else
        {
            RenderSettings.fog = false;
            globalVolume.profile = globalProfile;
        }
    }
}
