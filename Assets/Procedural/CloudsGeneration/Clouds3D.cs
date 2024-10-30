using UnityEditor;
using UnityEngine;

public class Clouds3D : MonoBehaviour
{
    public int textureSize = 64; // Size of the 3D texture
    public float scale = 1.0f; // Scale factor for noise
    public Texture3D cloudTexture; // The generated 3D texture
    public Material cloudMaterial; // Material to visualize the 3D texture

    private void Start()
    {
        GenerateTexture();
    }

    public void SaveTexture() {
        GenerateTexture();
        AssetDatabase.CreateAsset(cloudTexture, "Assets/Example3DTexture.asset");
    }

    public void GenerateTexture()
    {
        // Create the 3D texture if it doesn't exist
        if (cloudTexture == null)
        {
            cloudTexture = new Texture3D(textureSize, textureSize, textureSize, TextureFormat.ARGB32, false);
            cloudTexture.wrapMode = TextureWrapMode.Clamp;
        }

        // Create a 3D array for color data
        Color[] colors = new Color[textureSize * textureSize * textureSize];

        // Populate the color array using Perlin noise
        for (int z = 0; z < textureSize; z++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                for (int x = 0; x < textureSize; x++)
                {
                    // Calculate coordinates for multiple layers of noise
                    float xCoord = (float)x / textureSize * scale;
                    float yCoord = (float)y / textureSize * scale;
                    float zCoord = (float)z / textureSize * scale;

                    // Generate multiple layers of Perlin noise
                    float noiseValue1 = Mathf.PerlinNoise(xCoord * 0.5f, yCoord * 0.5f); // Layer 1 (larger scale)
                    float noiseValue2 = Mathf.PerlinNoise(xCoord * 2.0f, yCoord * 2.0f); // Layer 2 (smaller scale)
                    float noiseValue3 = Mathf.PerlinNoise(xCoord * 4.0f, yCoord * 4.0f); // Layer 3 (even smaller scale)

                    // Combine the noise values for a cloud-like appearance
                    float combinedNoise = (noiseValue1 + noiseValue2 * 0.5f + noiseValue3 * 0.25f) / 1.75f;

                    // Set alpha based on a threshold to create soft edges
                    float alpha = Mathf.Clamp01(combinedNoise * 2.0f - 0.5f); // Adjust alpha threshold

                    // Store color based on noise value (more white for clouds)
                    colors[x + y * textureSize + z * textureSize * textureSize] = new Color(combinedNoise, combinedNoise, combinedNoise, alpha);

                }
            }
        }

        // Set the pixels and apply the texture
        cloudTexture.SetPixels(colors);
        cloudTexture.Apply();

        // Assign the texture to the material if it's set
        if (cloudMaterial != null)
        {
            cloudMaterial.SetTexture("_Cloud3D", cloudTexture); // Assuming your shader uses this property name
        }
    }
}
