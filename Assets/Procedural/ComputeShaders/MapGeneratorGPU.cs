using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class MapGeneratorGPU : MonoBehaviour, ISavableData
{
    public Material terrainMaterial;
    [SerializeField] private ComputeShader computeShader;

    private ComputeBuffer perlinData;
    private ComputeBuffer smoothingData;
    private ComputeBuffer terrainHeightsData;
    private ComputeBuffer customAreaData;

    private int width = 513;
    private int height = 513;

    [SerializeField] private int seed;
    public int Seed => seed;
    [Space]
    [Header("Terrain Properties")]

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

    [Space]
    [Header("Terrain Texturing")]
    [SerializeField] private List<TerrainTexture> terrainDataList = new();

    [SerializeField] private Texture2D chunkTexture = null;
    [SerializeField] private RenderTexture slopeTexture = null;

    [Space]
    [Header("Smoothing Terrain")]
    [Range(0, 10)][SerializeField] private int smoothStrenght;

    [Space]
    [Header("Smoothing Custom Area")]
    [SerializeField] private float innerRadius; // Actual Radius of the area
    [SerializeField] private float outerRadius; // Radius of where to start interpolating toward innerRadius

    [SerializeField] private int interpolationNeightbours; //Number of neightbours to check 

    [SerializeField] private float targetHeight;
    [SerializeField] private AnimationCurve customAreaCurve;
    private void Awake()
    {
        terrain = GetComponent<Terrain>();

        terrainData = terrain.terrainData;

    }
    private void Start()
    {

        if (seed == 0) seed = UnityEngine.Random.Range(-1000, 1000);

        UnityEngine.Random.InitState(seed);
        Calculate();
    }

    public void Calculate()
    {
        ComputeShader comp = Instantiate(computeShader);
        noiseHeights = new float[width, height];

        perlinData = new ComputeBuffer(width * height, sizeof(float));
        smoothingData = new ComputeBuffer(width * height, sizeof(float));
        terrainHeightsData = new ComputeBuffer(width * height, sizeof(float));
        customAreaData = new ComputeBuffer(width * height, sizeof(float));


        GeneratePerlin();

        CreateTerrain(noiseHeights);
        CreateHeightmapTexture(noiseHeights);

        DestroyImmediate(comp);
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

        PerlinNoiseGenerationGPU();

        //////////Smoothing///////////////////////////////////////////////////////////////////////

        WholeTerrainSmoothing();

        //////////////////Smoothing Custom Area///////////////////////////////////////////////////////////////////////

        CustomAreaSmooth();

        ///Release///
        perlinData.Release();
        smoothingData.Release();
        terrainHeightsData.Release();
        customAreaData.Release();
    }

    private void PerlinNoiseGenerationGPU()
    {
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
    }

    private void WholeTerrainSmoothing()
    {
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
    }

    void CustomAreaSmooth()
    {
        int centerX = width / 2; // Modify later to be on a different location
        int centerZ = height / 2; // Modify later to be on a different location


        int kernelIndexSmoothingCustomArea = computeShader.FindKernel("SmoothingCustomArea");

        computeShader.SetBuffer(kernelIndexSmoothingCustomArea, "customAreaData", customAreaData);

        terrainHeightsData.SetData(noiseHeights);
        computeShader.SetBuffer(kernelIndexSmoothingCustomArea, "terrainHeightsData", terrainHeightsData);

        computeShader.SetInt("customAreaX", centerX);
        computeShader.SetInt("customAreaZ", centerZ);
        computeShader.SetInt("interpolationNeightbours", interpolationNeightbours);

        computeShader.SetFloat("innerRadius", innerRadius);
        computeShader.SetFloat("outerRadius", outerRadius);

        computeShader.SetFloat("targetHeight", targetHeight);

        float[] data = new float[width * height];

        computeShader.Dispatch(kernelIndexSmoothingCustomArea, width / 8, height / 8, 1);
        customAreaData.GetData(data);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = x + y * width;
                noiseHeights[x, y] = (data[index]);
            }
        }
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
        if (perlinData != null) perlinData.Release();
        if (smoothingData != null) smoothingData.Release();
        if (terrainHeightsData != null) terrainHeightsData.Release();
        if (customAreaData != null) customAreaData.Release();
        if (slopeTexture != null) slopeTexture.Release();
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

    public List<Vector3> GetCustomAreaPosition()
    {
        List<Vector3> customAreaPositions = new();

        int startPosX = width / 2;
        int startPosY = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(startPosX, startPosY));
                if (distance <= innerRadius)
                {
                    customAreaPositions.Add(new Vector3(x, 0, y));
                }
            }
        }

        return customAreaPositions;
    }

    public string ToJson()
    {
        TerrainDataSave data = new TerrainDataSave(seed);
        return JsonUtility.ToJson(data);
    }

    public void FromJson(string json)
    {
        TerrainDataSave data = JsonUtility.FromJson<TerrainDataSave>(json);
        seed = data.seed;
    }
}
