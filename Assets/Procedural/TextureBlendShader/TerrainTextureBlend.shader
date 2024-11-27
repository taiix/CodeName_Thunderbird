Shader "Custom/TerrainTextureBlend"
{
    Properties
    {
        _HeightmapTexture("Heightmap Texture", 2D) = "white"{}

        [noScaleOffset] _LayerTexture1("Layer 1 texture", 2D) = "white"{}
        [noScaleOffset] _LayerNormal1("Layer 1 texture", 2D) = "bumb"{}
        _LayerMetallic1("Layer 1 Metallic", Float) = 0
        _LayerSmoothness1("Layer 1 Smoothness", Float) = 0.5

        _LayerTexTilingOffset1("Layer 1 Tiling/Offset", Vector) = (1, 1, 0, 0)

        [noScaleOffset] _LayerTexture2("Layer 2 texture", 2D) = "white"{}
        [noScaleOffset] _LayerNormal2("Layer 2 texture", 2D) = "bumb"{}
        _LayerMetallic2("Layer 2 Metallic", Float) = 0
        _LayerSmoothness2("Layer 2 Smoothness", Float) = 0.5

        _LayerTexTilingOffset2("Layer 2 Tiling/Offset", Vector) = (1, 1, 0, 0)

        [noScaleOffset] _LayerTexture3("Layer 3 texture", 2D) = "white"{}
        [noScaleOffset] _LayerNormal3("Layer 3 texture", 2D) = "bumb"{}
        _LayerMetallic3("Layer 3 Metallic", Float) = 0
        _LayerSmoothness3("Layer 3 Smoothness", Float) = 0.5

        _LayerTexTilingOffset3("Layer 3 Tiling/Offset", Vector) = (1, 1, 0, 0)

        _blendFactorGrassToRock("Blend factor grass to rock", Float) = 0.1
        _blendFactorSandToGrass("_blendFactorSandToGrass", Float) = 0.1

        _blendLine("Blend Line", Float) = 2
        _b("e", Float) = 2
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
        sampler2D _SlopeTexture;

        sampler2D _LayerTexture1, _LayerNormal1;
        float _LayerMetallic1, _LayerSmoothness1;

        float4 _LayerTexTilingOffset1;

        sampler2D _LayerTexture2, _LayerNormal2;
        float _LayerMetallic2, _LayerSmoothness2;

        float4 _LayerTexTilingOffset2;

        sampler2D _LayerTexture3, _LayerNormal3;
        float _LayerMetallic3, _LayerSmoothness3;

        float4 _LayerTexTilingOffset3;

        sampler2D _LayerTexture4, _LayerNormal4;
        float _LayerMetallic4, _LayerSmoothness4;

        float4 _LayerTexTilingOffset4;

        int _texSize;
        float _MinHeights[4];
        float _MaxHeights[4];
        float tiling;

        float _blendFactorSandToGrass;
        float _blendFactorGrassToRock;
        float _blendLine;
        float _b;

        ///////////////////////////////////////////////////////////////////////////////////////
        struct Input
        {
            float2 uv_HeightmapTexture;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float terrainHeight = saturate(tex2D(_HeightmapTexture, IN.uv_HeightmapTexture).r);

            // Independent UVs for each texture
            float2 uvSand = IN.uv_HeightmapTexture * _LayerTexTilingOffset1.xy + _LayerTexTilingOffset1.zw;
            float2 uvGrass = IN.uv_HeightmapTexture * _LayerTexTilingOffset2.xy + _LayerTexTilingOffset2.zw;
            float2 uvRock = IN.uv_HeightmapTexture * _LayerTexTilingOffset3.xy + _LayerTexTilingOffset3.zw;

            // Sample textures using their individual UVs
            float4 sandTex = tex2D(_LayerTexture1, uvSand);
            float3 sandNormal = UnpackNormal(tex2D(_LayerNormal1, uvSand));

            float4 grassTex = tex2D(_LayerTexture2, uvGrass);
            float3 grassNormal = UnpackNormal(tex2D(_LayerNormal2, uvGrass));

            float4 rockTex = tex2D(_LayerTexture3, uvRock);
            float3 rockNormal = UnpackNormal(tex2D(_LayerNormal3, uvRock));

            float4 finalColor = float4(0, 0, 0, 0);
            float3 finalNormal = float3(0, 0, 0);
            float finalMetallic = 0;
            float finalSmoothness = 0;
            float noiseFactor = tex2D(_HeightmapTexture, IN.uv_HeightmapTexture).r;


            if (terrainHeight >= _MinHeights[0] && terrainHeight <= _MaxHeights[0])
            {
                finalColor = sandTex;
                finalNormal = sandNormal;
                finalMetallic = _LayerMetallic1;
                finalSmoothness = _LayerSmoothness1;

                if (terrainHeight >= _MaxHeights[0] - _blendFactorSandToGrass / 5000 && terrainHeight < _MinHeights[1] +
                    _blendFactorSandToGrass / 5000)
                {
                    float blend = smoothstep(_MaxHeights[0] - _blendFactorSandToGrass / 5000,
         _MinHeights[1] + _blendFactorSandToGrass / 5000,
         terrainHeight + noiseFactor * 0.05);

                    blend = pow(blend, _blendLine);

                    finalColor = lerp(sandTex, grassTex, blend);
                    finalNormal = lerp(sandNormal, grassNormal, blend);
                    finalMetallic = lerp(_LayerMetallic1, _LayerMetallic2, blend);
                    finalSmoothness = lerp(_LayerSmoothness1, _LayerSmoothness2, blend);
                }
            }
            else if (terrainHeight > _MinHeights[1] && terrainHeight <= _MaxHeights[1])
            {
                finalColor = grassTex;
                finalNormal = grassNormal;
                finalMetallic = _LayerMetallic2;
                finalSmoothness = _LayerSmoothness2;

                if (terrainHeight >= _MaxHeights[1] - _blendFactorGrassToRock / 5000 && terrainHeight < _MinHeights[2] +
                    _blendFactorGrassToRock / 5000)
                {
                    float blend = smoothstep(_MaxHeights[1] - _blendFactorGrassToRock / 5000,
                                                               _MinHeights[2] + _blendFactorGrassToRock / 5000,
                                                               terrainHeight + noiseFactor * 0.05);

                    blend = pow(blend, _blendLine);

                    finalColor = lerp(grassTex, rockTex, blend);
                    finalNormal = lerp(grassNormal, rockNormal, blend);
                    finalMetallic = lerp(_LayerMetallic2, _LayerMetallic3, blend);
                    finalSmoothness = lerp(_LayerSmoothness2, _LayerSmoothness3, blend);
                }
            }
            else if (terrainHeight > _MinHeights[2] && terrainHeight <= _MaxHeights[2])
            {
                finalColor = rockTex;
                finalNormal = rockNormal;
                finalMetallic = _LayerMetallic3;
                finalSmoothness = _LayerSmoothness3;
            }

            // Output final values
            o.Albedo = finalColor.rgb;
            o.Normal = finalNormal;
            o.Metallic = finalMetallic;
            o.Smoothness = finalSmoothness;
            o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}