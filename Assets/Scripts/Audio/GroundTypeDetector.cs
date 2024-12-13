using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class GroundTypeDetector : MonoBehaviour
{
    private CharacterMovement characterMovement;

    private AudioManager m_AudioManager;

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainTexture[] terrainDataList;

    private string currentGroundType = "Unknown";
    private int currentGroundIndex = -1;

    private void Start()
    {
        m_AudioManager = FindObjectOfType<AudioManager>();
        characterMovement = GetComponent<CharacterMovement>();

        if (m_AudioManager == null) Debug.LogError("Audio manager not found. Make sure audio manager is in the scene");
    }

    void Update()
    {
        if (m_AudioManager == null) return;
        CheckCurrentGroundType(characterMovement.isGrounded);
        CheckGroundType();
    }

    private void CheckGroundType()
    {
        if (terrain == null) return;

        Vector3 playerPosition = transform.position;
        float terrainHeight = terrain.SampleHeight(playerPosition) / terrain.terrainData.size.y;

        foreach (var terrainData in terrainDataList)
        {
            if (terrainHeight >= terrainData.MinHeight && terrainHeight <= terrainData.MaxHeight)
            {
                currentGroundType = terrainData.Material.name;
                currentGroundIndex = terrainData.soundID;
                return;
            }
        }

        currentGroundType = "Unknown";
        currentGroundIndex = -1;
    }

    void CheckCurrentGroundType(bool isCharacterGrounded)
    {
        if (!isCharacterGrounded)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f))
            {
                Terrain detectedTerrain = hit.collider.GetComponent<Terrain>();

                if (detectedTerrain != null)
                {
                    terrain = detectedTerrain;
                    return;
                }
            }
            else
            {
                terrain = null;
            }
        }
    }

    public int GetCurrentGroundID()
    {
        return currentGroundIndex;
    }
}

[System.Serializable]
public struct TerrainTexture
{
    public Material Material;
    public float MinHeight;
    public float MaxHeight;

    public int soundID;
}