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
                    // Calculate coordinates for noise
                    float xCoord = (float)x / textureSize * scale;
                    float yCoord = (float)y / textureSize * scale;
                    float zCoord = (float)z / textureSize * scale;

                    // Generate Perlin noise value
                    float noiseValue = Mathf.PerlinNoise(xCoord, yCoord) * Mathf.PerlinNoise(yCoord, zCoord);

                    // Store color based on noise value
                    colors[x + y * textureSize + z * textureSize * textureSize] = new Color(noiseValue, noiseValue, noiseValue, 1.0f);
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
