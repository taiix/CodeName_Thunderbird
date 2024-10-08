using System.Collections.Generic;
using UnityEngine;

public class HeightMapGenerator : MonoBehaviour
{
    [SerializeField] private Texture2D map;
    private Terrain terrain;
    public List<Vector2> posiblePositions = new();

    private Dictionary<Vector2, float> keyValuePairs = new Dictionary<Vector2, float>();

    void Awake()
    {
        terrain = GetComponent<Terrain>();
        Generate();
    }

    public Dictionary<Vector2, float> GetPossiblePositions()
    {
        return keyValuePairs;
    }

    private void Generate()
    {
        int width = map.width;
        int height = map.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float currentPixel = map.GetPixel(x, y).grayscale;
                
                keyValuePairs.Add(new Vector2(x, y), currentPixel);
            }
        }

        //StartCoroutine(ModifyTerrainAsync());
    }

    //IEnumerator ModifyTerrainAsync()
    //{
    //    TerrainData terrainData = terrain.terrainData;
    //    terrainData.heightmapResolution = map.width;

    //    int terrainRes = terrainData.heightmapResolution;
    //    float[,] terrainHeights = terrainData.GetHeights(0, 0, terrainRes, terrainRes);

    //    int chunkSize = 100; // Process a chunk of coordinates per frame

    //    for (int i = 0; i < posiblePositions.Count; i += chunkSize)
    //    {
    //        // Process a chunk of `asd` list
    //        for (int j = i; j < i + chunkSize && j < posiblePositions.Count; j++)
    //        {
    //            Vector2 mapPosition = posiblePositions[j];
    //            int xPos = Mathf.FloorToInt(mapPosition.x);
    //            int yPos = Mathf.FloorToInt(mapPosition.y);

    //            if (xPos >= 0 && xPos < terrainRes && yPos >= 0 && yPos < terrainRes)
    //            {
    //                terrainHeights[yPos, xPos] = 0.5f;
    //            }
    //        }

    //        // Apply the modified chunk back to the terrain
    //        terrainData.SetHeights(0, 0, terrainHeights);

    //        // Wait until the next frame before continuing
    //        yield return null;
    //    }

    //    Debug.Log("Terrain heights updated asynchronously.");
    //}
}