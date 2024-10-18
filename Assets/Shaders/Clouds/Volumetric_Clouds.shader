// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Volumetric_Clouds"
{
    Properties
    {
        _StepSize ("Ray Step Size", Range(0.01, 1)) = 0.05
        _NumSteps ("Number of Steps", Range(10, 200)) = 100
        _DensityScale ("Density Scale", Range(0, 1)) = 0.5
        _SphereCenter ("Sphere Center", Vector) = (0, 0, 0, 1)
        _SphereRadius ("Sphere Radius", Float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

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

            float _StepSize, _DensityScale, _SphereRadius;
            int _NumSteps;
            float4 _SphereCenter;

            struct v2f
            {
                float3 wPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            float RayMarching(float3 rayOrigin, float3 rayDir, float4 sphere, int numSteps, float stepSize)
            {
                for (int i = 0; i < numSteps; i++)
                {
                    bool reachedSphere = distance(rayOrigin, sphere.xyz) < sphere.w;

                    if (reachedSphere)
                        return length(rayOrigin);

                    rayOrigin += rayDir * stepSize;
                }
                return 0;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float3 viewDir = normalize(i.wPos - _WorldSpaceCameraPos);
                float3 worldPos = i.wPos;

                float rayDepth = RayMarching(viewDir, worldPos, _SphereCenter, _NumSteps, _StepSize);

                if (rayDepth != 0)
                    return float4(1 * rayDepth, 0, 0, 1);
                else
                {
                    return float4(1, 1, 1, 0);
                }
            }
            ENDCG
        }
    }
}