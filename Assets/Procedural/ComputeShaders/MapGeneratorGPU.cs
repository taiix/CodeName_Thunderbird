using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
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

    private float[,] noiseHeights;

    private Terrain terrain;
    private TerrainData terrainData;

    [SerializeField] private float decreasePercentage;
    [SerializeField] private AnimationCurve edgeCurve;

    [SerializeField] private List<TerrainTexture> terrainDataList = new();
    public float tiling;

    private void Start()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        Stopwatch sw = Stopwatch.StartNew();

        Calculate();

        sw.Stop();

        UnityEngine.Debug.Log("Time elapsed: " + sw.ElapsedMilliseconds + " ms");
    }

    public void Calculate()
    {
        noiseHeights = new float[width, height];
        perlinData = new ComputeBuffer(width * height, sizeof(float));

        UnityEngine.Random.InitState(seed);

        GeneratePerlin();
        CreateTerrain(noiseHeights);

    }

    #region Compute Shader
    void GeneratePerlin()
    {
        ComputeShaderProperties();

        float[] data = new float[width * height];

        perlinData.GetData(data);

        noiseHeights = GetHeightsFromShader(data);

        CreateHeightmapTexture(noiseHeights);
    }

    void ComputeShaderProperties()
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

    }

    #endregion

    #region Surface Shader
    private void CreateHeightmapTexture(float[,] heights)
    {
        Texture2D heightmapTexture = new Texture2D(width, height, TextureFormat.RFloat, false);
        heightmapTexture.filterMode = FilterMode.Trilinear;
        heightmapTexture.wrapMode = TextureWrapMode.Repeat;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float heightValue = (heights[y, x]);
                heightmapTexture.SetPixel(x, y, new Color(heightValue, heightValue, heightValue));
            }
        }

        heightmapTexture.Apply();

        terrainMaterial.SetTexture("_HeightmapTexture", heightmapTexture);

        terrainMaterial.SetFloat("tiling", tiling);

        SendTextureInfoToSurface();
    }

    private void SendTextureInfoToSurface()
    {
        int getSize = terrainDataList.Count;

        terrainMaterial.SetInt("_texSize", getSize);

        float[] minHeights = new float[getSize];
        float[] maxHeights = new float[getSize];

        for (int i = 0; i < getSize; i++)
        {
            TerrainTexture terrainTexture = terrainDataList[i];

            minHeights[i] = terrainTexture._minHeight;
            maxHeights[i] = terrainTexture._maxHeight;

            terrainMaterial.SetTexture($"_LayerTexture{i + 1}", terrainTexture.texture);
        }

        terrainMaterial.SetFloatArray("_MinHeights", minHeights);
        terrainMaterial.SetFloatArray("_MaxHeights", maxHeights);
    }
    #endregion

    #region Terrain Generation
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
    #endregion

    private void OnDestroy()
    {
        if (perlinData != null)
            perlinData.Release();
    }

    [Serializable]
    public struct TerrainTexture
    {
        public Texture2D texture;
        public float _minHeight;
        public float _maxHeight;
    }
}
