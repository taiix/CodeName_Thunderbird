Shader "Custom/Water_Lit"
{
    Properties
    {
        _DeepColor ("Deep Color", Color) = (0, 0.5, 1, 1) // Default deep color
        _ShallowColor ("Shallow Color", Color) = (0, 0.2, 0.5, 1) // Default shallow color
        _NormalMap1 ("Normal map 1", 2D) = "bump" {}
        _NormalMap2 ("Normal map 2", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.0

        _SpeedMap1 ("Wave Speed 1", Float) = 0.1
        _SpeedMap2 ("Wave Speed 2", Float) = 0.2
        _Scale ("Wave Scale", Range(0, 1)) = 0.1
        _Amplitude ("Wave Amplitude", Range(0, 1)) = 0.1

        _DepthFactor ("Depth Factor", Float) = 1.0
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType"="Transparent"
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha vertex:vert

        #include "UnityCG.cginc"

        sampler2D _NormalMap1;
        sampler2D _NormalMap2;
        sampler2D _CameraDepthTexture;

        struct Input
        {
            float2 uv_NormalMap1;
            float2 uv_NormalMap2;

            float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        float4 _DeepColor;
        float4 _ShallowColor;

        float _SpeedMap1;
        float _SpeedMap2;
        float _Scale;
        float _Amplitude;

        float _DepthFactor;

        void vert(inout appdata_full v, out Input i)
        {
            UNITY_INITIALIZE_OUTPUT(Input, i);
            i.screenPos = ComputeScreenPos(v.vertex); // Assign screen position for depth
        }

        // Function to compute screen-space UV coordinates
        float4 ComputeScreenCoords(Input i)
        {
            float4 screenUV = i.screenPos; // Screen position passed from vert
            screenUV.xy /= screenUV.w; // Normalize by w for NDC
            return screenUV;
        }


        float3 e(Input IN)
        {
            float t1 = _Time * _SpeedMap1; // Continuous wave motion
            float t2 = _Time * _SpeedMap2;

            float2 offset1 = float2(t1 * _Scale, 0); // Horizontal wave movement
            float2 offset2 = float2(0, t2 * _Scale); // Vertical wave movement

            float2 uv1 = IN.uv_NormalMap1 + offset1;
            float2 uv2 = IN.uv_NormalMap2 + offset2;

            float3 normal1 = UnpackNormal(tex2D(_NormalMap1, uv1));
            float3 normal2 = UnpackNormal(tex2D(_NormalMap2, uv2));

            float3 blendedNormal = normalize(normal1 + normal2); // Blending normals

            return blendedNormal;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 screenUV = ComputeScreenCoords(IN);
        
            float cameraToUnderwaterDist = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV.xy));
            float cameraToSurfaceDist = UNITY_Z_0_FAR_FROM_CLIPSPACE(screenUV.z);
            
            float depthDiff = saturate((cameraToUnderwaterDist - cameraToSurfaceDist) / _DepthFactor);

            o.Albedo = lerp(_ShallowColor, _DeepColor, depthDiff);

            o.Alpha = _DeepColor.a;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            o.Normal = e(IN);
        }
        ENDCG
    }
}