using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private TerrainResolution resolution;

    private int width;
    private int height;

    [SerializeField] private float scale;
    [SerializeField] private float decreasePercentage;
    [SerializeField] private AnimationCurve edgeCurve;
    float[,] edgeReduction;

    #region Perlin Noise Variables
    [Header("Perlin Noise")]
    [SerializeField] private int octaves;
    [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private int seed;
    [SerializeField] private float terrainHighMultiplier;

    [SerializeField] private Vector2 offset;

    private float[,] noiseHeights;
    #endregion

    #region Terrain Variables
    [Space]
    [Header("Terrain")]

    private Terrain terrain;
    private TerrainData terrainData;

    [SerializeField] private float terrainHeight;
    [SerializeField] private AnimationCurve terrainCurve;

    [Space]
    [Header("Terrain Texture")]
    [SerializeField] private List<SplatHeights> splatHeights = new();


    #endregion

    #endregion


    private void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        Stopwatch sw = Stopwatch.StartNew();
        CalculatePerlin();
        sw.Stop();
        UnityEngine.Debug.Log("Time elapsed: " + sw.ElapsedMilliseconds + " ms");
        //Smooth();
    }

    private void OnValidate()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;
    }

    #region Generation
    public void CalculatePerlin()
    {
        int resolutionValue = (int)resolution;

        terrainData.alphamapResolution = resolutionValue;

        width = resolutionValue;
        height = resolutionValue;

        noiseHeights = new float[width, height];

        if (scale <= 0) scale = 0.0001f;

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        edgeReduction = CalculateIslandBorders();

        Random.InitState(seed);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;        //aka height
                float frequency = 1;        //aka lenght
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = ((float)x / (width / 2) * scale * frequency) + offset.x + seed;
                    float yCoord = ((float)y / (height / 2) * scale * frequency) + offset.y + seed;

                    float perlin = Mathf.PerlinNoise(xCoord, yCoord);
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight < minValue) minValue = noiseHeight;
                if (noiseHeight > maxValue) maxValue = noiseHeight;
                noiseHeight = Mathf.Clamp(noiseHeight, minValue, maxValue);
                noiseHeights[(int)x, (int)y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseHeights[x, y] *= edgeReduction[x, y];
                noiseHeights[x, y] = terrainCurve.Evaluate(
                       (noiseHeights[x, y] - minValue) / (maxValue - minValue)) * terrainHighMultiplier;
            }
        }

        SplatMaps();

        ApplyTerrainSettings();
    }

    //public void Smooth()
    //{
    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int dy = -1; dy <= 1; dy++)
    //            {
    //                for (int dx = -1; dx <= 1; dx++)
    //                {
    //                    int neighborX = x + dx;
    //                    int neighborY = y + dy;

    //                    Debug.Log($"Neighbor at: ({neighborX}, {neighborY})");
    //                }
    //            }
    //        }
    //    }
    //}

    private float[,] CalculateIslandBorders()
    {
        float[,] edgeReduction = new float[width, height];

        int halfMap = width / 2;
        Vector2Int centerOfIsland = new Vector2Int(halfMap, halfMap);

        float unAffectedAreaBorder = decreasePercentage * halfMap;

        float areaToAffect = halfMap - unAffectedAreaBorder;    //The area to affect, from the edge of unAffectedArea to edge of the map

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //substract the area so inside the area will be a negative number and not gonna be affected.
                //Without substracting the edge reduction will start immediately
                float distanceToCenter = Vector2.Distance(centerOfIsland, new Vector2(x, y)) - unAffectedAreaBorder;

                if (distanceToCenter < 0)
                {
                    edgeReduction[x, y] = 1;
                }

                else if (distanceToCenter > areaToAffect)
                {
                    edgeReduction[x, y] = 0;
                }
                else edgeReduction[x, y] = edgeCurve.Evaluate(1 - distanceToCenter / areaToAffect);
            }
        }
        return edgeReduction;
    }

    private void ApplyTerrainSettings()
    {
        terrainData.heightmapResolution = (int)resolution;

        terrainData.size = new Vector3(width, terrainHeight, height);
        terrainData.SetHeights(0, 0, noiseHeights);
    }
    #endregion

    #region Splatmap Textures

    private void CreateLayers()
    {
        TerrainLayer[] newSplatPrototypes = new TerrainLayer[splatHeights.Count];

        for (int spIndex = 0; spIndex < splatHeights.Count; spIndex++)
        {
            SplatHeights sh = splatHeights[spIndex];

            TerrainLayer newLayer = new TerrainLayer
            {
                diffuseTexture = sh.texture
            };

            newLayer.diffuseTexture.Apply(true);
            string path = $"Assets/Terrain Layers/New Terrain Layer {spIndex}.terrainlayer";
            AssetDatabase.CreateAsset(newLayer, path);

            newSplatPrototypes[spIndex] = newLayer;
        }

        terrainData.terrainLayers = newSplatPrototypes;
    }

    private void SplatMaps()
    {
        CreateLayers();

        int mapResolution = terrainData.heightmapResolution;

        int alphaWidth = terrainData.alphamapWidth;
        int alphaHeight = terrainData.alphamapHeight;
        int alphaLayers = terrainData.alphamapLayers;

        float[,,] splatmapData = new float[alphaWidth, alphaHeight, alphaLayers];
        float[,] terrainHeights = terrainData.GetHeights(0, 0, mapResolution, mapResolution);

        for (int y = 0; y < alphaHeight; y++)
        {
            for (int x = 0; x < alphaWidth; x++)
            {
                float[] splatWeights = new float[alphaLayers];
                float currentPointHeight = terrainData.GetHeight(y, x) / terrainData.size.y;

                for (int j = 0; j < splatHeights.Count; j++)
                {
                    float min = splatHeights[j].minHeight;
                    float max = splatHeights[j].maxHeight;

                    if (currentPointHeight >= min && currentPointHeight <= max)
                    {
                        splatWeights[j] = 1;
                    }

                    for (int i = 0; i < splatWeights.Length; i++)
                        splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    #endregion

    #region Serializable
    public enum TerrainResolution
    {
        Resolution_33x33 = 33,
        Resolution_65x65 = 65,
        Resolution_129x129 = 129,
        Resolution_257x257 = 257,
        Resolution_513x513 = 513,
        Resolution_1025x1025 = 1025,
        Resolution_2049x2049 = 2049,
        Resolution_4097x4097 = 4097
    }

    public enum FalloffType
    {
        Square,
        Circle
    }

    [System.Serializable]
    public class SplatHeights
    {
        public Texture2D texture = null;
        public float minHeight = 0.1f;
        public float maxHeight = 0.2f;
    }
    #endregion
}