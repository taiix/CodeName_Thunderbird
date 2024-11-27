// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Volumetric_Clouds"
{
    Properties
    {
        _FogColor("Fog Color", Color) = (1,1,1,1)
        _DensityScale ("Density Scale", Range(0, 1)) = 0.5
        _SphereCenter ("Sphere Center", Vector) = (0, 0, 0, 1)
        _InnerRatio ("Inner Ratio", Float) = 0.5

        _Cloud3D ("Cloud 3d texture", 3D) = ""{}
        _CloudScale ("Cloud Scale", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off Lighting Off ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            float _StepSize, _DensityScale, _SphereRadius, _MaxDistance, _InnerRatio;
            int _NumSteps;
            float4 _SphereCenter;
            float4 _FogColor;

            sampler2D _CameraDepthTexture;
            sampler3D _Cloud3D;

            float _CloudScale;

            float CloudDensity(float3 position)
            {
                float3 scaledPos = position * _CloudScale;
                float density = tex3D(_Cloud3D, scaledPos).r;
                return density * _DensityScale;
            }
            
            struct v2f
            {
                float3 wPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD1;
            };

            float FogDensity(float3 rayOrigin, float4 sphereCenter,
                             float maxDistance, float density, float innerRatio, float3 viewDir)
            {
                //RayOrigin = O, RayDirection = D
                float3 _rayOrigin = rayOrigin;
                float3 _viewDir = viewDir;

                float radius = sphereCenter.w;
                //P^2 - R^2 expressed as (O + tD)^2 - R^2 = 0
                /*
                 * dot(O, O) + 2t*dot(O,D) + t^2*dot(D,D) - R2 = 0
                 * t^2*dot(D,D) + 2t*dot(O,D) + dot(O, O) = 0
                 * a = dot(D, D), b = 2 * dot(O, D), c = dot(O, O) - R2
                 * d = b^2 - 4ac
                 */
                float a = dot(_viewDir, _viewDir);
                float b = 2 * dot(_rayOrigin, _viewDir);
                float c = dot(_rayOrigin, _rayOrigin) - radius * radius;

                float d = b * b - 4 * a * c;

                if (d <= 0) return 0;

                float entryPoint = max((-b - sqrt(d)) / (2 * a), 0);
                float exitPoint = max((-b + sqrt(d)) / (2 * a), 0);


                float maximumDepth = min(maxDistance, exitPoint);
                float rayMarchingStartingPoint = entryPoint; // march from start to end

                float stepSize = (maximumDepth - entryPoint) / 10;
                float densityFactor = density;

                float fogGradient = 1 / (1 - innerRatio); //Center to outer

                float fogTransparency = 1;

                for (int i = 0; i < 10; i++)
                {
                    float3 positionOnRay = _rayOrigin + _viewDir * rayMarchingStartingPoint;
                    // Use 'sample' instead of 'entryPoint'
                     float cloudDensity = CloudDensity(positionOnRay);
                    float distanceFactor = saturate(fogGradient * (1 - length(positionOnRay) / radius));
                    float fogAccumulation = saturate(distanceFactor * densityFactor * cloudDensity);

                    fogTransparency *= 1 - fogAccumulation;

                    rayMarchingStartingPoint += stepSize;
                }

                return 1 - fogTransparency;
            }

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.projPos = ComputeScreenPos(o.pos);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 worldPos = i.wPos;

                float depth = LinearEyeDepth(
                    UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));


                float3 viewDir = normalize(i.wPos - _WorldSpaceCameraPos);

                float fogDensity = FogDensity(_WorldSpaceCameraPos,
                                                _SphereCenter, depth,
                                                _DensityScale, _InnerRatio, viewDir);

                half4 color = half4(1, 1, 1, 1);

                color.rgb = _FogColor.rgb;
                color.a = fogDensity;
                return color;
            }
            ENDCG
        }
    }
}