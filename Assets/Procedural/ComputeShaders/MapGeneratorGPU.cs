using System.Diagnostics;
using UnityEngine;

public class MapGeneratorGPU : MonoBehaviour
{
    public Material terrainMaterial;
    [SerializeField] private ComputeShader computeShader;
    private ComputeBuffer perlinData;

    private int width = 513;
    private int height = 513;

    [Header("Perlin Noise")]
    [SerializeField] private int seed;

    [SerializeField] private float scale;

    [SerializeField] private int octaves;
    [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;

    [SerializeField] private AnimationCurve terrainCurve;

    [SerializeField] private float terrainHighMultiplier;

    [SerializeField] private Vector2 offset;

    private Texture2D texture;

    private float[,] noiseHeights;

    [SerializeField] private Terrain terrain;
    [SerializeField] private TerrainData terrainData;
    [SerializeField] private float decreasePercentage;
    [SerializeField] private AnimationCurve edgeCurve;

    public void Calculate()
    {
        Stopwatch sw = Stopwatch.StartNew();
        noiseHeights = new float[width, height];
        perlinData = new ComputeBuffer(width * height, sizeof(float));

        Random.InitState(seed);

        GeneratePerlin();
        CreateTerrain(noiseHeights);

        sw.Stop();
        UnityEngine.Debug.Log("Time elapsed: " + sw.ElapsedMilliseconds + " ms");
    }

    void GeneratePerlin()
    {
        int kernelIndex = computeShader.FindKernel("CSMain");

        computeShader.SetBuffer(kernelIndex, "perlinData", perlinData);

        computeShader.SetInt("width", width);
        computeShader.SetInt("height", height);
        computeShader.SetInt("seed", seed);
        computeShader.SetFloat("scale", scale);
        computeShader.SetFloat("octaves", octaves);
        computeShader.SetFloat("persistence", persistence);
        computeShader.SetFloat("lacunarity", lacunarity);

        computeShader.SetVector("offset", offset);

        computeShader.Dispatch(0, width / 8, height / 8, 1);

        float[] data = new float[width * height];

        perlinData.GetData(data);

        ComputeBuffer heightBuffer = new ComputeBuffer (data.Length, sizeof(float));
        heightBuffer.SetData(data);
        terrainMaterial.SetBuffer("terrainHeights", heightBuffer);
        terrainMaterial.SetInt("_TerrainWidth", width);
        terrainMaterial.SetInt("_TerrainHeight", height);

        noiseHeights = GetHeightsFromShader(data);
    }

    private float[,] GetHeightsFromShader(float[] data)
    {
        float[,] heights = new float[width, height];
        float[,] edgeReduction = CalculateIslandBorders();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                heights[x, y] = data[index];
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                heights[x, y] *= edgeReduction[x, y];
                heights[x, y] = terrainCurve.Evaluate(heights[x, y]) * terrainHighMultiplier;
            }
        }

        return heights;
    }

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

    private void CreateTerrain(float[,] heights)
    {
        terrainData.SetHeights(0, 0, heights);
    }

    private void OnDestroy()
    {
        if (perlinData != null)
            perlinData.Release();
    }
}
