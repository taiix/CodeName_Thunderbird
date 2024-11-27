using UnityEditor;
using UnityEngine;

public class Clouds3D : MonoBehaviour
{
    public enum Type { Perlin, Worley, Combined }

    public Type textureType;

    [Header("3D Texture Settings")]
    public int textureSize = 64; // Typically smaller for 3D textures to save memory

    [Header("Perlin Noise Settings")]
    public int octaves = 4;
    public float scale = 10.0f;
    public float persistence = 0.5f;
    public float lacunarity = 2.0f;

    [Header("Worley Noise Settings")]
    public float worleyStrength = 1.0f;
    public int numPoints = 30;
    public Vector3[] points;

    [Header("Combination Settings")]
    [Range(0, 1)] public float blendFactor = 0.5f; // 0 = full Perlin, 1 = full Worley

    public Texture3D cloudTexture;
    public Material cloudMaterial;

    private void Start()
    {
        GenerateTexture();
    }

    public void GenerateTexture()
    {
        if (textureType == Type.Perlin) GeneratePerlin3DTexture();
        else if (textureType == Type.Worley) GenerateWorley3DTexture();
        else if (textureType == Type.Combined) GenerateCombined3DTexture();
    }

    // Generate Perlin Noise 3D Texture
    void GeneratePerlin3DTexture()
    {
        cloudTexture = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.RGBA32, false);

        Color[] colors = new Color[textureSize * textureSize * textureSize];
        for (int z = 0; z < textureSize; z++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float perlinValue = GeneratePerlin3D(x, y, z);
                    colors[x + y * textureSize + z * textureSize * textureSize] = new Color(perlinValue, perlinValue, perlinValue, 1.0f);
                }
            }
        }

        cloudTexture.SetPixels(colors);
        cloudTexture.Apply();
        cloudMaterial.SetTexture("_CloudTexture", cloudTexture);
    }

    float GeneratePerlin3D(int x, int y, int z)
    {
        float frequency = scale;
        float amplitude = 1.0f;
        float perlinValue = 0.0f;

        for (int i = 0; i < octaves; i++)
        {
            float nx = x * frequency / textureSize;
            float ny = y * frequency / textureSize;
            float nz = z * frequency / textureSize;

            perlinValue += Mathf.PerlinNoise(nx, ny) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return Mathf.Clamp01(perlinValue);
    }

    // Generate Worley Noise 3D Texture
    void GenerateWorley3DTexture()
    {
        InitPoints();

        cloudTexture = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] colors = new Color[textureSize * textureSize * textureSize];
        float maxDistance = Mathf.Sqrt(textureSize * textureSize * 3);

        for (int z = 0; z < textureSize; z++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float worleyValue = CalculateWorleyValue(x, y, z, maxDistance);
                    colors[x + y * textureSize + z * textureSize * textureSize] = new Color(worleyValue, worleyValue, worleyValue, 1.0f);
                }
            }
        }

        cloudTexture.SetPixels(colors);
        cloudTexture.Apply();
        cloudMaterial.SetTexture("_CloudTexture", cloudTexture);
    }

    void InitPoints()
    {
        points = new Vector3[numPoints];
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(
                Random.Range(0, textureSize),
                Random.Range(0, textureSize),
                Random.Range(0, textureSize)
            );
        }
    }

    float CalculateWorleyValue(int x, int y, int z, float maxDistance)
    {
        float[] distances = new float[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 point = points[i];
            float dist = Vector3.Distance(new Vector3(x, y, z), point);
            distances[i] = dist;
        }

        System.Array.Sort(distances);
        float worleyValue = 1 - Mathf.InverseLerp(0, maxDistance, distances[0]);
        worleyValue = Mathf.Pow(worleyValue, worleyStrength);

        return worleyValue;
    }

    // Generate Combined 3D Texture using Perlin and Worley Noise
    void GenerateCombined3DTexture()
    {
        cloudTexture = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] colors = new Color[textureSize * textureSize * textureSize];
        float maxDistance = Mathf.Sqrt(textureSize * textureSize * 3);

        for (int z = 0; z < textureSize; z++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    float perlinValue = GeneratePerlin3D(x, y, z);
                    float worleyValue = CalculateWorleyValue(x, y, z, maxDistance);

                    // Blend Perlin and Worley based on blendFactor
                    float combinedValue = Mathf.Lerp(perlinValue, worleyValue, blendFactor);
                    colors[x + y * textureSize + z * textureSize * textureSize] = new Color(combinedValue, combinedValue, combinedValue, 1.0f);
                }
            }
        }

        cloudTexture.SetPixels(colors);
        cloudTexture.Apply();
        cloudMaterial.SetTexture("_CloudTexture", cloudTexture);
    }

    public void SaveTexture()
    {
        GenerateTexture();
        AssetDatabase.CreateAsset(cloudTexture, "Assets/Generated3DCloudTexture.asset");
        Debug.Log("3D texture saved as 'Generated3DCloudTexture.asset' in the Assets folder.");
    }
}
