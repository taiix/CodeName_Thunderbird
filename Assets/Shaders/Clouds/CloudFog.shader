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

        float CalculateFogIntensity(float3 rayOrigin, float3 sphereCenter, float radius, float stepSize,
                                    float maxDistance, float innerRatio, float density)
        {
            //RayOrigin = O, RayDirection = D
            float3 O = rayOrigin;
            float3 D = normalize(sphereCenter - rayOrigin);
            //P^2 - R^2 expressed as (O + tD)^2 - R^2 = 0
            /*
             * dot(O, O) + 2t*dot(O,D) + t^2*dot(D,D) - R2 = 0
             * t^2*dot(D,D) + 2t*dot(O,D) + dot(O, O) = 0
             * a = dot(D, D), b = 2 * dot(O, D), c = dot(O, O) - R2
             * d = b^2 - 4ac
             */
            float a = dot(D, D);
            float b = 2 * dot(O, D);
            float c = dot(O, O) - radius * radius;

            float d = b * b - 4 * a * c;

            if (d <= 0) return 0;

            float entryPoint = max((-b + sqrt(d)) / (2 * a), 0);
            float exitPoint = max((-b - sqrt(d)) / (2 * a), 0);


            float backDepth = min(maxDistance, exitPoint );
            float sample1 = entryPoint; // march from start to end
            float stepDist = (exitPoint - entryPoint) / 10;
            float stepContribution = density;
            float clarity = 1;
             float centerValue = 1 / (1 - innerRatio);
            for (int i = 0; i < 10; i++)
            {
                float3 positionOnRay = O + D * sample1; // Use 'sample' instead of 'entryPoint'
                float val = saturate(centerValue * (1 - length(positionOnRay) / radius));

                float fogIntensity = saturate(val * density);
                clarity *= 1 - fogIntensity;
                sample1 += stepDist;
            }
            float fogIntensity = 1 - clarity;
            return fogIntensity;
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

            float fog = CalculateFogIntensity(_WorldSpaceCameraPos, _FogCenter.xyz, _FogCenter.w, 0.1, 100.0,
                                    _InnerRatio, _Density);

            col.rgb = _FogColor.rgb;
            col.a = fog; // Apply fog intensity to alpha

            o.Albedo = col.rgb;
            o.Alpha = col.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}