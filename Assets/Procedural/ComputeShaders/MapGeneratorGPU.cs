using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class MapGeneratorGPU : MonoBehaviour
{
    public Material terrainMaterial;
    [SerializeField] private ComputeShader computeShader;

    private ComputeBuffer perlinData;
    private ComputeBuffer smoothingData;
    private ComputeBuffer terrainHeightsData;
    private ComputeBuffer customAreaData;

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
    [SerializeField] private float tiling;

    [SerializeField] private Texture2D chunkTexture = null;

    [Range(0, 10)][SerializeField] private int smoothStrenght;
    public int customAreaSmoothIteration;

    private void Awake()
    {
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

        Calculate();
    }

    public void Calculate()
    {
        noiseHeights = new float[width, height];

        perlinData = new ComputeBuffer(width * height, sizeof(float));
        smoothingData = new ComputeBuffer(width * height, sizeof(float));
        terrainHeightsData = new ComputeBuffer(width * height, sizeof(float));
        customAreaData = new ComputeBuffer(width * height, sizeof(float));

        UnityEngine.Random.InitState(seed);

        GeneratePerlin();
        CreateTerrain(noiseHeights);
        CreateHeightmapTexture(noiseHeights);

    }

    private void OnValidate()
    {
        noiseHeights = new float[width, height];
        terrain = GetComponent<Terrain>();
        terrainData = terrain.terrainData;

    }

    #region Compute Shader
    void GeneratePerlin()
    {
        ComputeShaderProperties();
    }


    void ComputeShaderProperties()
    {
        /////////PerlinNoise////////////////////////////////////////////////////////////////////

        int kernelIndexPerlinNoise = computeShader.FindKernel("CSMain");

        computeShader.SetBuffer(kernelIndexPerlinNoise, "perlinData", perlinData);

        computeShader.SetInt("width", width);
        computeShader.SetInt("height", height);
        computeShader.SetInt("seed", seed);

        computeShader.SetFloat("scale", scale);
        computeShader.SetFloat("octaves", octaves);
        computeShader.SetFloat("persistence", persistence);
        computeShader.SetFloat("lacunarity", lacunarity);

        computeShader.SetVector("offset", offset);

        computeShader.Dispatch(kernelIndexPerlinNoise, width / 8, height / 8, 1);

        float[] data = new float[width * height];

        perlinData.GetData(data);
        noiseHeights = GetHeightsFromShader(data);

        //////////Smoothing///////////////////////////////////////////////////////////////////////

        int kernelIndexSmoothing = computeShader.FindKernel("SmoothingWholeTerrain");

        computeShader.SetBuffer(kernelIndexSmoothing, "smoothingData", smoothingData);

        terrainHeightsData.SetData(noiseHeights);
        computeShader.SetBuffer(kernelIndexSmoothing, "terrainHeightsData", terrainHeightsData);

        computeShader.SetInt("smoothRadius", smoothStrenght);

        computeShader.Dispatch(kernelIndexSmoothing, width / 8, height / 8, 1);

        float[] smoothedHeightsData = new float[width * height];
        smoothingData.GetData(smoothedHeightsData);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                noiseHeights[x, y] = smoothedHeightsData[index];
            }
        }

        //////////////////Smoothing Custom Area///////////////////////////////////////////////////////////////////////
        int kernelIndexSmoothingCustomArea = computeShader.FindKernel("SmoothingCustomArea");
        computeShader.SetBuffer(kernelIndexSmoothingCustomArea, "customAreaData", customAreaData);

        computeShader.SetInt("customAreaSmoothIteration", customAreaSmoothIteration);


        ///Release///
        perlinData.Release();
        smoothingData.Release();
        terrainHeightsData.Release();
    }

    #endregion

    #region Surface Shader
    private void CreateHeightmapTexture(float[,] heights)
    {
        Texture2D heightmapTexture = new Texture2D(width, height, TextureFormat.RFloat, false);
        chunkTexture = new Texture2D(width, height, TextureFormat.RFloat, false);

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

        chunkTexture = heightmapTexture;

        terrainMaterial.SetTexture("_HeightmapTexture", heightmapTexture);

        terrainMaterial.SetFloat("tiling", tiling);


        SendTextureInfoToSurface();
    }

    //void SmoothingMultiplePasses(int smoothingRadius = 3, int passes = 3)
    //{
    //    for (int p = 0; p < passes; p++)
    //    {
    //        float[,] _heights = noiseHeights;
    //        float[,] smoothedHeights = (float[,])_heights.Clone();

    //        for (int y = 0; y < height; y++)
    //        {
    //            for (int x = 0; x < width; x++)
    //            {
    //                float averageHeight = 0;
    //                int validNeighbors = 0;

    //                for (int i = -smoothingRadius; i <= smoothingRadius; i++)
    //                {
    //                    for (int j = -smoothingRadius; j <= smoothingRadius; j++)
    //                    {
    //                        int neighborX = x + j;
    //                        int neighborY = y + i;

    //                        if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
    //                        {
    //                            averageHeight += _heights[neighborX, neighborY];
    //                            validNeighbors++;
    //                        }
    //                    }
    //                }

    //                smoothedHeights[x, y] = averageHeight / validNeighbors;
    //            }
    //        }

    //        // Update the terrain heights
    //        noiseHeights = smoothedHeights;
    //    }
    //}


    //void FlattenRectangle()
    //{
    //    int centerX = width / 2;
    //    int centerY = height / 2;
    //    float influenceRadius = 100f;

    //    float avgHeightAroundSpot = SampleAverageHeightAroundSpot(centerX, centerY, radius, influenceRadius);

    //    for (int y = 0; y < height; y++)
    //    {
    //        for (int x = 0; x < width; x++)
    //        {
    //            float dist = Vector3.Distance(new Vector3(x, 0, y), new Vector3(centerX, 0, centerY));

    //            if (dist <= radius)
    //            {
    //                float t = Mathf.SmoothStep(1, 0, Mathf.Clamp01((dist - radius) / (influenceRadius - radius)));

    //                float originalHeight = noiseHeights[x, y];

    //                float heightDifference = Mathf.Lerp(originalHeight, avgHeightAroundSpot, t);

    //                float blendedHeight = Mathf.Lerp(heightDifference, tt, t);

    //                noiseHeights[x, y] = blendedHeight;
    //            }
    //        }
    //    }
    //}

    float SampleAverageHeightAroundSpot(int centerX, int centerY, float innerRadius, float outerRadius)
    {
        float totalHeight = 0f;
        int sampleCount = 0;

        // Loop through points around the custom spot to get average height
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate distance from the center of the custom spot
                float dist = Vector3.Distance(new Vector3(x, 0, y), new Vector3(centerX, 0, centerY));

                // Only sample heights within the outer influence radius, but outside the inner radius
                if (dist > innerRadius && dist <= outerRadius)
                {
                    totalHeight += noiseHeights[x, y];
                    sampleCount++;
                }
            }
        }

        // Return the average height
        return totalHeight / sampleCount;
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

            Texture2D mainTexture = terrainTexture.material.mainTexture as Texture2D;

            if (mainTexture != null)
            {
                terrainMaterial.SetTexture($"_LayerTexture{i + 1}", mainTexture);
            }
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
                heights[x, y] = (data[index]);
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
        public Material material;
        public float _minHeight;
        public float _maxHeight;
    }

    public Texture2D GetHeightmapTexture()
    {
        return chunkTexture;
    }
}
