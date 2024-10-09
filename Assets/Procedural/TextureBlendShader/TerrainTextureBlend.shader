Shader "Custom/TerrainTextureBlend"
{
    Properties
    {
        _HeightmapTexture("Heightmap Texture", 2D) = "white"{}

        _LayerTexture1("Layer 1 texture", 2D) = "white"{}
        _LayerTexture2("Layer 2 texture", 2D) = "white"{}
        _LayerTexture3("Layer 3 texture", 2D) = "white"{}
        _LayerTexture4("Layer 4 texture", 2D) = "white"{}

        _blendFactor("Blend factor", Float) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "TerrainCompatible" = "true"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0
        ///////////////////////////////////////Properties//////////////////////////////////////
        sampler2D _HeightmapTexture;

        sampler2D _LayerTexture1;
        sampler2D _LayerTexture2;
        sampler2D _LayerTexture3;
        sampler2D _LayerTexture4;

        int _texSize;
        float _MinHeights[4];
        float _MaxHeights[4];
        float tiling;

        float _blendFactor;

        ///////////////////////////////////////////////////////////////////////////////////////
        struct Input
        {
            float2 uv_HeightmapTexture;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float terrainHeight = saturate(tex2D(_HeightmapTexture, IN.uv_HeightmapTexture).r);

            float4 waterTex = tex2D(_LayerTexture1, IN.uv_HeightmapTexture * tiling);
            float4 sandTex = tex2D(_LayerTexture2, IN.uv_HeightmapTexture * tiling);
            float4 grassTex = tex2D(_LayerTexture3, IN.uv_HeightmapTexture * tiling);
            float4 rockTex = tex2D(_LayerTexture4, IN.uv_HeightmapTexture * tiling);

            float4 finalColor = float4(0, 0, 0, 0);
            float blendF = 0;

            if (terrainHeight <= _MaxHeights[0]) // Water
            {
                finalColor = waterTex;
            }
            else if (terrainHeight > _MaxHeights[0] && terrainHeight <= _MaxHeights[1])
            // Interpolate between water and sand
            {
                blendF = smoothstep(_MinHeights[1], _MaxHeights[1], terrainHeight);
                finalColor = lerp(waterTex, sandTex, blendF);
            }
            else if (terrainHeight > _MaxHeights[1] && terrainHeight <= _MaxHeights[2])
            // Interpolate between sand and grass
            {
                blendF = smoothstep(_MinHeights[2], _MaxHeights[2], terrainHeight);
                finalColor = lerp(sandTex, grassTex, blendF);
            }
            else if (terrainHeight > _MaxHeights[2] && terrainHeight <= _MaxHeights[3])
            // Interpolate between grass and rock
            {
                blendF = smoothstep(_MinHeights[3], _MaxHeights[3], terrainHeight);
                finalColor = lerp(grassTex, rockTex, blendF);
            }
            else // Rock (at the top)
            {
                finalColor = rockTex;
            }
            o.Albedo = finalColor.rgb;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}