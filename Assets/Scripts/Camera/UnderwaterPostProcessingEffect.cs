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

    [SerializeField] private GameObject[] waterSurface;
    private bool isUnderwater = false;
    private void Start()
    {
        globalVolume = GetComponent<PostProcessVolume>();


        waterSurface = GameObject.FindGameObjectsWithTag("WaterSurface");
        RenderSettings.fog = false;
        globalVolume.profile = globalProfile;
    }

    private void Update()
    {
        CheckWaterTriggers();
    }

    private void CheckWaterTriggers()
    {
        foreach (var water in waterSurface)
        {
            if (water == null) continue;

            Collider waterCollider = water.GetComponent<Collider>();
            if (waterCollider == null || !waterCollider.isTrigger)
            {
                Debug.LogWarning($"{water.name} does not have a trigger collider!");
                continue;
            }

            if (waterCollider.bounds.Contains(transform.position))
            {
                ApplyUnderwaterEffects();
                return;
            }
        }

        ApplyGlobalEffects();
    }

    private void ApplyUnderwaterEffects()
    {
        if (!isUnderwater)
        {
            Debug.Log("Underwater effects applied.");
            isUnderwater = true;
            RenderSettings.fog = true;
            globalVolume.profile = underwaterEffectProfile;
        }
    }

    private void ApplyGlobalEffects()
    {
        if (isUnderwater)
        {
            Debug.Log("Global effects applied.");
            isUnderwater = false;
            RenderSettings.fog = false;
            globalVolume.profile = globalProfile;
        }
    }
}
