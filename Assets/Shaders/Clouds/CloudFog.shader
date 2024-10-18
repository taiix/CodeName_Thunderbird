Shader "Custom/CloudFog"
{
    Properties
    {
        _FogCenter("Fog Center/Radius", Vector) = (0,0,0,0.5)
        _FogColor ("Fog Color", Color) = (1,1,1,1)
        _InnerRatio("Inner Ration", Range(0.0, 0.9)) = 0.5
        _Density("Fog Density", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {


        Tags
        {
            "Queue"="Transparent"
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert

        #pragma target 3.0

        float4 _FogCenter;
        float4 _FogColor;

        float _InnerRatio;
        float _Density;

        sampler2D _CameraDepthTexture;

        float CalculateFogIntensity(float3 shpereCenter, float sphereRadius, float innerRatio, float density,
                                    float3 cameraPosition, float3 viewDir, float maxDistance)
        {
            //calculate ray-sphere intersection
            float3 localCam = cameraPosition - shpereCenter;

            float a = dot(viewDir, viewDir);
            float b = 2 * dot(viewDir, localCam);
            float c = dot(localCam, localCam) - sphereRadius * sphereRadius;

            float d = b * b - 4 * a * c;

            if (d <= 0)
                return 0;

            float dSqrt = sqrt(d);
            float dist = max((-b - dSqrt) / 2 * a, 0);
            float dist2 = max((-b + dSqrt) / 2 * a, 0);

            float backDepth = min(maxDistance, dist2);
            //float sample = dist;
            float stepDist = (backDepth - dist) / 10;
            float stepContribution = density;

            float centerValue = 1 / (1 - innerRatio);

            float clarity = 1;
            for (int i = 0; i < 10; i++)
            {
                float3 position = localCam + viewDir * dist;
                float val = saturate(centerValue * (1 - length(position) / sphereRadius));
                float fogAmount = saturate(val * stepContribution);
                clarity *= (1 - fogAmount);
                dist += stepDist;
            }

            return 1 - clarity;
        }

        struct Input
        {
            float3 viewDir;
            float4 pos;
            float4 projPos;
        };

        void vert(inout appdata_full v, out Input i)
        {
            float4 wPos = mul(unity_ObjectToWorld, v.vertex);
            i.pos = UnityObjectToClipPos(v.vertex);
            i.viewDir = wPos.xyz - _WorldSpaceCameraPos;
            i.projPos = ComputeScreenPos(i.pos);

            float inFrontOF = (i.pos.z / i.pos.w) > 0;
            i.pos.z *= inFrontOF;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 col = float4(1, 1, 1, 1);
            float depth = LinearEyeDepth(
                UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projPos))));
            float3 viewDir = normalize(IN.viewDir);
            float fog = CalculateFogIntensity(_FogCenter.xyz, _FogCenter.w, _InnerRatio, _Density,
                                                               _WorldSpaceCameraPos, viewDir, depth);


            col.rgb = _FogColor.rgb;
            col.a = fog;
            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}