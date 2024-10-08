Shader "Custom/TerrainTextureBlend"
{
    Properties
    {
        _LayerTexture1("Layer 1 texture", 2D) = "white"{}
        _minHeight1("Layer 1 min height", Float) = 0.0
        _maxHeight1("Layer 1 max height", Float) = 0.0

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

        sampler2D _LayerTexture1;
        StructuredBuffer<float> terrainHeights;
        int _TerrainWidth;
        int _TerrainHeight;

        float _minHeight1;
        float _maxHeight1;
        float _blendFactor;

        struct TextureLayer
        {
            sampler2D tex;
            float minHeight;
            float maxHeight;
        };

        struct Input
        {
            float2 uv;
        };

        TextureLayer CreateLayer(sampler2D tex, float minHeight, float maxHeight)
        {
            TextureLayer layer;
            layer.tex = tex;
            layer.minHeight = minHeight;
            layer.maxHeight = maxHeight;

            return layer;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            TextureLayer layer1 = CreateLayer(_LayerTexture1, _minHeight1, _maxHeight1);

            int scaledUV_x = (int)(IN.uv.x * _TerrainWidth);
            int scaledUV_y = (int)(IN.uv.y * _TerrainHeight);

            int index = scaledUV_x + scaledUV_y * _TerrainWidth;

            float terrainHeight = terrainHeights[index];

            float blend = smoothstep(layer1.minHeight, layer1.maxHeight + _blendFactor, terrainHeight);
            float4 tex = tex2D(layer1.tex, IN.uv);

            float4 final = tex * blend;

            o.Albedo = float3(terrainHeight,terrainHeight,terrainHeight);
            o.Alpha = final.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}