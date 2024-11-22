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
        _TopLine("_TopLine", Float) = 0.01

        _NoiseTex("_NoiseTex", 2D) = "white"{}

        _WaveFrequency("_WaveFrequency", Float) = 0.1
        _WaveAmplitude("_WaveAmplitude", Float) = 0.1
        _Threshold("_Threshold", Float) = 0.1

        //SNOW
        [HDR] _SnowColor ("Snow Color", Color) = (1,1,1,1)

        _SnowHeightStart ("_SnowHeightStart", Range(0,1)) = 1.0

        _SnowTexture("Snow texture", 2D) = "white"{}
        _SnowNormal("Snow normal", 2D) = "bumb"{}

        _SnowMetallic("Snow Metallic", Float) = 0
        _SnowSmoothness("Snow Smoothness", Float) = 0.5

        _SnowBlend("Snow Blend", Float) = 0.5
        _SnowStrenght("Snow Strenght", Float) = 0.5
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

        float _TopLine;
        float _WaveFrequency;
        float _WaveAmplitude;
        float _Threshold;
        sampler2D _NoiseTex;

        //Snow
        float _SnowHeightStart, _SnowBlend, _SnowStrenght;
        sampler2D _SnowTexture, _SnowNormal;
        float _SnowMetallic, _SnowSmoothness;
        float4 _SnowColor;

        ///////////////////////////////////////////////////////////////////////////////////////
        struct Input
        {
            INTERNAL_DATA
            float2 uv_HeightmapTexture;
            float2 uv_NoiseTex;
            float2 uv_SnowTexture;
            float3 worldNormal;
        };

        void initProperties(Input IN, out float terrainHeight,
                            out float4 sandTex, out float3 sandNormal,
                            out float4 grassTex, out float3 grassNormal,
                            out float4 rockTex, out float3 rockNormal,
                            out float4 snowTex, out float3 snowNormal,
                            out float noiseFactor)
        {
            terrainHeight = saturate(tex2D(_HeightmapTexture, IN.uv_HeightmapTexture).r);

            float2 uvSand = IN.uv_HeightmapTexture * _LayerTexTilingOffset1.xy + _LayerTexTilingOffset1.zw;
            float2 uvGrass = IN.uv_HeightmapTexture * _LayerTexTilingOffset2.xy + _LayerTexTilingOffset2.zw;
            float2 uvRock = IN.uv_HeightmapTexture * _LayerTexTilingOffset3.xy + _LayerTexTilingOffset3.zw;

            sandTex = tex2D(_LayerTexture1, uvSand);
            sandNormal = UnpackNormal(tex2D(_LayerNormal1, uvSand));

            grassTex = tex2D(_LayerTexture2, uvGrass);
            grassNormal = UnpackNormal(tex2D(_LayerNormal2, uvGrass));

            rockTex = tex2D(_LayerTexture3, uvRock);
            rockNormal = UnpackNormal(tex2D(_LayerNormal3, uvRock));

            snowTex = tex2D(_SnowTexture, IN.uv_SnowTexture);
            snowNormal = UnpackNormal(tex2D(_SnowNormal, IN.uv_SnowTexture));

            noiseFactor = tex2D(_HeightmapTexture, IN.uv_HeightmapTexture).r;
        }

        void setTexture(float4 mainTexture, float3 mainNormal, float metallic, float smoothness,
                   out float4 finalColor,
                   out float3 finalNormal,
                   out float finalMetallic,
                   out float finalSmoothness)
        {
            finalColor = mainTexture;
            finalNormal = mainNormal;
            finalMetallic = metallic;
            finalSmoothness = smoothness;
        }

        void colorInterp(
            float4 startTex, float3 startNormal, float metallicStart, float smoothnessStart,
            float4 endTex, float3 endNormal, float metallicEnd, float smoothnessEnd,
            float blendValue,
            out float4 finalColor, out float3 finalNormal, out float finalMetallic, out float finalSmoothness)
        {
            blendValue = pow(blendValue, _blendLine);

            finalColor = lerp(startTex, endTex, blendValue);
            finalNormal = normalize(lerp(startNormal, endNormal, blendValue));
            finalMetallic = lerp(metallicStart, metallicEnd, blendValue);
            finalSmoothness = lerp(smoothnessStart, smoothnessEnd, blendValue);
        }

        void processLayers(
            float terrainHeight, int startHeight, int endHeight,
            float4 startTex, float3 startNormal, float metallicStart, float smoothnessStart,
            float4 endTex, float3 endNormal, float metallicEnd, float smoothnessEnd,
            float blendFactor, float noiseFactor, float offset, out float4 finalColor,
            out float3 finalNormal, out float finalMetallic, out float finalSmoothness)
        {
            // NB!: LEAVE OFFSET 0 IF IT'S NOT NEEDED
            if (terrainHeight >= _MaxHeights[startHeight] - blendFactor &&
                terrainHeight < _MinHeights[endHeight] + blendFactor)
            {
                float blend = smoothstep(
                    _MaxHeights[startHeight] - blendFactor,
                    _MinHeights[endHeight] + blendFactor,
                    terrainHeight + noiseFactor * 0.05);

                colorInterp(
                    startTex, startNormal, metallicStart, smoothnessStart,
                    endTex, endNormal, metallicEnd, smoothnessEnd,
                    blend,
                    finalColor, finalNormal, finalMetallic, finalSmoothness);
            }
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float terrainHeight;
            float4 sandTex, grassTex, rockTex, snowTex;
            float3 sandNormal, grassNormal, rockNormal, snowNormal;
            float noiseFactor;

            initProperties(IN, terrainHeight, sandTex, sandNormal, grassTex, grassNormal, rockTex, rockNormal, snowTex,
          snowNormal,
          noiseFactor);

            float4 finalColor = float4(0, 0, 0, 0);
            float3 finalNormal = float3(0, 0, 0);
            float finalMetallic = 0;
            float finalSmoothness = 0;

            if (terrainHeight >= _MinHeights[0] && terrainHeight <= _MaxHeights[0])
            {
                //SAND TO GRASS
                processLayers(
                    terrainHeight, 0, 1,
                    sandTex, sandNormal, _LayerMetallic1, _LayerSmoothness1,
                    grassTex, grassNormal, _LayerMetallic2, _LayerSmoothness2,
                    _blendFactorSandToGrass / 5000, noiseFactor, 0,
                    finalColor, finalNormal, finalMetallic, finalSmoothness);
            }
            else if (terrainHeight > _MinHeights[1] - _TopLine && terrainHeight <= _MaxHeights[1] + _TopLine)
            {
                //GRASS TO ROCK
                setTexture(grassTex, grassNormal, _LayerMetallic2, _LayerSmoothness2,
                                        finalColor, finalNormal, finalMetallic,
                                        finalSmoothness);

                if (terrainHeight >= _MaxHeights[1] - _blendFactorGrassToRock / 5000 &&
                    terrainHeight < _MinHeights[2] + _blendFactorGrassToRock / 5000)
                {
                    float2 uvNoise = IN.uv_NoiseTex * _WaveFrequency;
                    float noiseOffset = tex2D(_NoiseTex, uvNoise).r * (_Threshold / 5000);

                    float wave = sin(IN.uv_HeightmapTexture.x * _WaveFrequency) * (_WaveAmplitude / 5000);

                    float combinedOffset = noiseOffset + wave;

                    float blend = smoothstep(
                        _MaxHeights[1] - _blendFactorGrassToRock / 5000 + combinedOffset,
                        _MinHeights[2] + _blendFactorGrassToRock / 5000 + combinedOffset,
                        terrainHeight + noiseFactor * 0.05);

                    colorInterp(
                        grassTex, grassNormal, _LayerMetallic2, _LayerSmoothness2,
                        rockTex, rockNormal, _LayerMetallic3, _LayerSmoothness3,
                        blend,
                        finalColor, finalNormal, finalMetallic, finalSmoothness);
                }
            }
            else if (terrainHeight > _MinHeights[2] && terrainHeight <= _MaxHeights[2])
            {
                //ROCK TO SNOW
                setTexture(snowTex, snowNormal, _SnowMetallic, _SnowSmoothness,
                                    finalColor, finalNormal, finalMetallic,
                                    finalSmoothness);

                if (terrainHeight >= _MaxHeights[1] - _SnowBlend / 5000 &&
                    terrainHeight < _SnowHeightStart + _SnowBlend / 5000)
                {
                    float2 uvNoise = IN.uv_NoiseTex * _WaveFrequency;
                    float noiseOffset = tex2D(_NoiseTex, uvNoise).r * (_Threshold / 5000);

                    float wave = sin(IN.uv_HeightmapTexture.x * _WaveFrequency) * (_WaveAmplitude / 5000);

                    float combinedOffset = noiseOffset + wave;

                    float blend = smoothstep(
                        _MaxHeights[1] - _SnowBlend / 5000 + combinedOffset,
                        _SnowHeightStart + _SnowBlend / 5000 + combinedOffset,
                        terrainHeight + noiseFactor * _SnowStrenght);

                    colorInterp(rockTex, rockNormal, _LayerMetallic3, _LayerSmoothness3,
                                                                             snowTex * _SnowColor, snowNormal, _SnowMetallic,
                                                                             _SnowSmoothness,
                                                                             blend,
                                                                             finalColor, finalNormal, finalMetallic,
                                                                             finalSmoothness);
                }
                else if (terrainHeight > _MinHeights[2] && terrainHeight <= _MaxHeights[2])
                {
                    setTexture(rockTex, rockNormal, _LayerMetallic3, _LayerSmoothness3,
                                                    finalColor, finalNormal, finalMetallic,
                                                    finalSmoothness);
                }
            }

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