Shader "Custom/Water_Lit"
{
    Properties
    {
        _DeepColor ("Deep Color", Color) = (0, 0.5, 1, 1)
        _ShallowColor ("Shallow Color", Color) = (0, 0.2, 0.5, 1)

        _NormalMap1 ("Normal map 1", 2D) = "bump" {}
        _NormalMap2 ("Normal map 2", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.0

        _WaveHeight ("Wave Height", Float) = 0.1
        _WaveSpeed ("Wave Speed", Float) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 0.1
        _WaveLenght ("Wave Lenght", Float) = 0.1

        _SpeedMap1 ("Wave Speed 1", Float) = 0.1
        _SpeedMap2 ("Wave Speed 2", Float) = 0.2
        _Scale ("Wave Scale", Range(0, 1)) = 0.1
        _Amplitude ("Wave Amplitude", Range(0, 1)) = 0.1

        [HDR]_FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamIntensity ("Foam Intensity", Float) = 0.5
        _FoamScale ("Foam Scale", Float) = 1.0
        _FoamCutoff ("Foam Cutoff", Float) = 1.0

        _DepthFactor ("Depth Factor", Float) = 1.0
        _CenterPoint ("Center Point", Vector) = (0,0,0,0)


        _RefractoringNormal("Refractoring Normal", 2D) = "bump" {}
        _RefractionStrength ("Refraction Strengt", Float) = 1.0
        _RefractionSpeed ("Refraction Speed", Float) = 1.0
    }
    SubShader
    {
        GrabPass
        {
            "_GrabTexture"
        }
        Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha vertex:vert
        #pragma target 3.0

        #include "UnityCG.cginc"

        sampler2D _NormalMap1;
        sampler2D _NormalMap2;
        sampler2D _GrabTexture;
        sampler2D _CameraDepthTexture;
        sampler2D _RefractoringNormal;

        struct Input
        {
            float2 uv_NormalMap1;
            float2 uv_NormalMap2;
            float2 uv_GrabTexture;
            float2 uv_RefractoringNormal;

            float3 worldNormal;
            float3 worldPos;

            float4 screenPos;
        };


        half _Glossiness;
        half _Metallic;
        float4 _DeepColor;
        float4 _ShallowColor;

        float _WaveHeight, _WaveSpeed, _WaveFrequency, _WaveLenght;


        float _SpeedMap1, _SpeedMap2, _Scale, _Amplitude;

        float4 _FoamColor;

        float _FoamIntensity, _FoamScale, _FoamCutoff;

        float _DepthFactor;

        float4 _CenterPoint;

        float _RefractionStrength;
        float _RefractionSpeed;

        void vert(inout appdata_full v, out Input i)
        {
            UNITY_INITIALIZE_OUTPUT(Input, i);
            i.screenPos = ComputeScreenPos(v.vertex);

            float3 p = v.vertex.xyz;
            
            float k = 2 * UNITY_PI / _WaveLenght;
            float c = sqrt(9.8 / k); 
            float f = k * (p.x - c * _Time.y);
            float a = _WaveHeight / k;
            p.x += a * cos(f);
            p.y = a * sin(f);
            
            
            //v.vertex.xyz = p;
           
        }

        ///////////////////////////////////////////HELPERS////////////////////////////////////////////
        float4 ComputeScreenCoords(Input i)
        {
            float4 screenUV = i.screenPos;
            screenUV.xy /= screenUV.w;
            return screenUV;
        }

        float CalculateDepth(Input IN, float scaleFactor)
        {
            float4 screenUV = ComputeScreenCoords(IN);

            float cameraToUnderwaterDist = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV.xy));
            float cameraToSurfaceDist = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenUV.z);

            float depthDiff = saturate((cameraToUnderwaterDist - cameraToSurfaceDist) / scaleFactor);

            return depthDiff;
        }

        float3 blendedNormals(Input IN)
        {
            float t1 = _Time * _SpeedMap1;
            float t2 = _Time * _SpeedMap2;

            float2 offset1 = float2(t1 * _Scale, 0);
            float2 offset2 = float2(0, t2 * _Scale);

            float2 uv1 = IN.uv_NormalMap1 + offset1;
            float2 uv2 = IN.uv_NormalMap2 + offset2;

            float3 normal1 = UnpackNormal(tex2D(_NormalMap1, uv1));
            float3 normal2 = UnpackNormal(tex2D(_NormalMap2, uv2));

            float3 blendedNormal = normalize(normal1 + normal2);

            return blendedNormal;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////GRADIENT NOISE ////////////////////////////////////////////
        float2 unity_gradientNoise_dir(float2 p)
        {
            p = p % 289;
            float x = (34 * p.x + 1) * p.x % 289 + p.y;
            x = (34 * x + 1) * x % 289;
            x = frac(x / 41) * 2 - 1;
            return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
        }

        float unity_gradientNoise(float2 p)
        {
            float2 ip = floor(p);
            float2 fp = frac(p);
            float d00 = dot(unity_gradientNoise_dir(ip), fp);
            float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
            float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
            float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
            fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
            return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
        }

        void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
        {
            Out = unity_gradientNoise(UV * Scale) + 0.5;
        }

        float GradientNoiseGeneration(float foamScale, Input IN)
        {
            float3 blendUVs = blendedNormals(IN);
            float gradientNoise;
            Unity_GradientNoise_float(blendUVs.xy, foamScale, gradientNoise);

            return (gradientNoise);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////FOAM//////////////////////////////////////////////////////
        float foam(Input IN, float foamAmount, float foamCutoff)
        {
            float depth = CalculateDepth(IN, foamAmount);

            float gradNoise = GradientNoiseGeneration(_FoamScale, IN);
            float foamThreshold = depth * foamCutoff;
            float foamPlacement = step(foamThreshold, gradNoise);
            float res = foamPlacement * _FoamColor.a;
            return res;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////////////REFRACTION////////////////////////////////////////////
        //Snell's Law
        float3 Refraction(Input IN)
        {
            float n1 = 1.0; // air
            float n2 = 1.33; // water

            float t1 = _Time * _SpeedMap1;
            float t2 = _Time * _SpeedMap2;

            float2 offset1 = float2(t1 * _Scale, 0);
            float2 offset2 = float2(0, t2 * _Scale);

            float2 uv1 = IN.uv_RefractoringNormal + offset1 + offset2;
            float3 lightPos = _WorldSpaceLightPos0;
            float3 normal = normalize(UnpackNormal(tex2D(_RefractoringNormal, uv1)));

            float3 incidentRay = normalize(lightPos - IN.worldPos);

            float3 refractionRay = normalize(refract(incidentRay, normal, n1 / n2));
            return refractionRay;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float depth = CalculateDepth(IN, _DepthFactor);

            float3 refractionRay = Refraction(IN);
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

            float2 distortedUV = screenUV + refractionRay.xy * _RefractionStrength * 0.02;

            float3 refractedColor = tex2D(_GrabTexture, distortedUV);

            float4 waterColor = lerp(_ShallowColor, _DeepColor, depth);

            float foamAmount = foam(IN, _FoamIntensity, _FoamCutoff);
            float4 waterFoamColor = lerp(waterColor, _FoamColor, foamAmount);

            float3 finalColor = lerp(refractedColor, waterFoamColor, 0.4);

            o.Albedo = finalColor;

            o.Alpha = _DeepColor.a;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            o.Normal = blendedNormals(IN);
        }
        ENDCG
    }
}