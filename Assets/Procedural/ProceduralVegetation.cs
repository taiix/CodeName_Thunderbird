using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapGeneratorGPU), typeof(Terrain))]
public class ProceduralVegetation : MonoBehaviour
{
    [SerializeField] private int treeSpacing;
    [SerializeField] private float randPos;
    public Texture2D tex;
    public Vector2 min_max_Values;  //x = min(0-1) : y = max(0-1)

    private List<Vector3> availablePositions = new();
    [SerializeField] private List<Trees> trees = new();

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

        min_max_Values.x = Mathf.Clamp01(min_max_Values.x);
        min_max_Values.y = Mathf.Clamp01(min_max_Values.y);

        GetAvailableRegions();
        PopulateTreeObjects();
    }

    void PopulateTreeObjects()
    {
        GameObject randTree = trees[0].treePrefab;
        float treeRadius = 5f;

        List<Vector3> placedTrees = new();

        for (int y = 0; y < terrainData.size.z; y += treeSpacing)
        {
            for (int x = 0; x < terrainData.size.x; x += treeSpacing)
            {
                int randIndex = Random.Range(0, availablePositions.Count);
                Vector3 position = availablePositions[randIndex];

                availablePositions.RemoveAt(randIndex);

                float worldX = terrain.transform.position.x + ((float)position.x / (terrainData.heightmapResolution - 1)) * terrainData.size.x;
                float worldZ = terrain.transform.position.z + ((float)position.z / (terrainData.heightmapResolution - 1)) * terrainData.size.z;

                float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ)) + terrain.transform.position.y;

                Vector3 newTreePosition = new Vector3(worldX, worldY, worldZ);


                bool canPlace = true;
                foreach (var oldTree in placedTrees)
                {
                    float dist = (oldTree - newTreePosition).magnitude;
                    if (dist < treeRadius)
                    {
                        canPlace = false;
                        break;
                    }
                }
                if (canPlace)
                {
                    Instantiate(randTree, newTreePosition, Quaternion.identity);
                    placedTrees.Add(newTreePosition);
                }

            }
        }
    }

    void GetAvailableRegions()
    {
        int width = tex.width;
        int height = tex.height;

        float minHeight = min_max_Values.x;
        float maxHeight = min_max_Values.y;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currectPixel = tex.GetPixel(x, y, 0).r;
                if (currectPixel >= minHeight && currectPixel <= maxHeight)
                {
                    availablePositions.Add(new Vector3(x, 0, y));
                }
            }
        }
    }

    [System.Serializable]
    public struct Trees
    {
        public GameObject treePrefab;
    }
}
