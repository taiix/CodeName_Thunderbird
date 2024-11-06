using UnityEngine;

public class Noise : MonoBehaviour
{
    [Header("Noise Settings")]
    public int textureSize = 64;
    public float noiseScale = 10.0f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public int seed = 42;

    public Texture3D noiseTexture;

    void Start()
    {
        Generate3DNoiseTexture();
    }

    void Generate3DNoiseTexture()
    {
        // Create a 3D texture
        noiseTexture = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.RFloat, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        Color[] colors = new Color[textureSize * textureSize * textureSize];

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                for (int z = 0; z < textureSize; z++)
                {
                    float xCoord = x / (float)textureSize * noiseScale;
                    float yCoord = y / (float)textureSize * noiseScale;
                    float zCoord = z / (float)textureSize * noiseScale;

                    // Use the Noise3D function to get a noise value
                    float noiseValue = Noise3D(xCoord, yCoord, zCoord, 1.0f, 1.0f, persistence, octaves, seed);

                    colors[x + y * textureSize + z * textureSize * textureSize] = new Color(noiseValue, noiseValue, noiseValue, 1);
                }
            }
        }

        noiseTexture.SetPixels(colors);
        noiseTexture.Apply();

        GetComponent<Renderer>().material.SetTexture("_CloudNoiseTex", noiseTexture);
    }

    public static float Noise3D(float x, float y, float z, float frequency, float amplitude, float persistence, int octave, int seed)
    {
        float noise = 0.0f;

        for (int i = 0; i < octave; ++i)
        {
            float noiseXY = Mathf.PerlinNoise(x * frequency + seed, y * frequency + seed) * amplitude;
            float noiseXZ = Mathf.PerlinNoise(x * frequency + seed, z * frequency + seed) * amplitude;
            float noiseYZ = Mathf.PerlinNoise(y * frequency + seed, z * frequency + seed) * amplitude;

            float noiseYX = Mathf.PerlinNoise(y * frequency + seed, x * frequency + seed) * amplitude;
            float noiseZX = Mathf.PerlinNoise(z * frequency + seed, x * frequency + seed) * amplitude;
            float noiseZY = Mathf.PerlinNoise(z * frequency + seed, y * frequency + seed) * amplitude;

            // Average the noise values from different permutations
            noise += (noiseXY + noiseXZ + noiseYZ + noiseYX + noiseZX + noiseZY) / 6.0f;

            amplitude *= persistence;
            frequency *= 2.0f;
        }

        return noise / octave;  // Average the final noise across octaves
    }
}
