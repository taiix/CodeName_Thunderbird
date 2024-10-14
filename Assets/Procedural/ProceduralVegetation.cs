using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapGeneratorGPU))]
public class ProceduralVegetation : MonoBehaviour
{
    private Texture2D tex;

    private List<Vector3> availablePositions = new();
    [SerializeField] private List<Vegetation> vegetations = new();

    #region References
    private MapGeneratorGPU _mapGenerator;
    private Terrain terrain;
    private TerrainData terrainData;
    #endregion

    private void Start()
    {
        _mapGenerator = GetComponent<MapGeneratorGPU>();
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        tex = _mapGenerator.GetHeightmapTexture();

        PopulateTreeObjects();
    }

    public void PopulateTreeObjects()
    {
        GetAvailableRegions();
        List<Vector3> placedVegetation = new();

        Debug.Log($"terrainData.size.x : {terrainData.heightmapResolution}");

        foreach (var vegetation in vegetations)
        {
            for (int y = 0; y < terrainData.size.z; y += vegetation.spacing)
            {
                for (int x = 0; x < terrainData.size.x; x += vegetation.spacing)
                {
                    //normalized to world height. minHeight = 0.2 and terrainData.size.y = 100 is 20%
                    //returns heights in world space
                    float minHeight = vegetation.minHeight * terrainData.size.y;
                    float maxHeight = vegetation.maxHeight * terrainData.size.y;

                    float radius = vegetation.radius;

                    Vector3 position = GetRandomPosition();

                    Vector3 vegetationWorldPosition = ConvertTerrainToWorldPosition(position);

                    float currentPositionHeight = vegetationWorldPosition.y;

                    if (currentPositionHeight < minHeight || currentPositionHeight > maxHeight)
                        continue;

                    bool canPlace = true;

                    foreach (var oldVegetation in placedVegetation)
                    {
                        float dist = (oldVegetation - vegetationWorldPosition).magnitude;
                        if (dist < vegetation.radius)
                        {
                            canPlace = false;
                            break;
                        }
                    }

                    if (canPlace)
                    {
                        GameObject go = Instantiate
                            (vegetation.prefab, vegetationWorldPosition,
                            Quaternion.identity, this.gameObject.transform);

                        go.transform.localScale = new Vector3
                            (go.transform.localScale.x, 2, go.transform.localScale.z);

                        placedVegetation.Add(go.transform.position);
                    }
                }
            }
        }
    }

    private Vector3 GetRandomPosition()
    {
        int randIndex = Random.Range(0, availablePositions.Count);
        Vector3 position = availablePositions[randIndex];

        availablePositions.RemoveAt(randIndex);
        return position;
    }

    void GetAvailableRegions()
    {
        int width = tex.width;
        int height = tex.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currectPixel = tex.GetPixel(x, y, 0).r;

                availablePositions.Add(new Vector3(x, 0, y));
            }
        }
    }

    Vector3 ConvertTerrainToWorldPosition(Vector3 position)
    {
        float worldX = (float)position.x + terrain.transform.position.x;  // + terrain.transform.position.x if there are islands with x pos > 0

        float worldZ = (float)position.z + terrain.transform.position.z;  // + terrain.transform.position.z if there are islands with z pos > 0

        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)); // + + terrain.transform.position.z if there are islands with y > 0

        Vector3 newVegetationPosition = new Vector3(worldX, worldY, worldZ);

        return newVegetationPosition;
    }

    [System.Serializable]
    public struct Vegetation
    {
        public GameObject prefab;
        public float minHeight;
        public float maxHeight;
        public float radius;
        public int spacing;
    }
}
