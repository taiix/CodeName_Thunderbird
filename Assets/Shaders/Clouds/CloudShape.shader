Shader "Unlit/CloudShape"
{
    Properties
    {
        _Scale ("Scale", Range(0.1, 10.0)) = 2
        _StepScale ("Step Scale", Range(0.1, 100.0)) = 1
        _Steps ("Steps", Range(1, 200)) = 60

        _MinHeight("Minimum height", Range(0.0, 5.0)) = 0
        _MaxHeight("Maximum height", Range(6.0, 10.0)) = 10

        _FadeDist("Fade distance", Range(6.0, 10.0)) = 10
        _SunDir("Sun direction", Vector) = (1,0,0,0)
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

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 view : TEXCOORD0;
                float2 projPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float _MinHeight, _MaxHeight, _FadeDist, _Scale, _StepScale, _Steps;
            float4 _SunDir;
            sampler2D _CameraDepthTexture;

            float rand1d(float3 value, float3 dotDir)
            {
                float3 smallV = sin(value);
                float rand = dot(smallV, dotDir);
                rand = frac(sin(rand) * 123574.43212);
                return rand;
            }

            float rand3d(float3 value)
            {
                return float3(
                    rand1d(value, float3(12.456, 68.53, 37.5312)),
                    rand1d(value, float3(65.754, 23.76, 76.1234)),
                    rand1d(value, float3(34.235, 43.23, 87.6342)));
            }

            float noise3d(float3 value)
            {
                value *= _Scale;
                float3 interpolationValue = frac(value);
                interpolationValue = smoothstep(0.0, 1.0, interpolationValue);

                float3 zV[2];
                for (int z = 0; z <= 1; z++)
                {
                    float3 yV[2];
                    for (int y = 0; y <= 1; y++)
                    {
                        float3 xV[2];
                        for (int x = 0; x <= 1; x++)
                        {
                            float3 cell = floor(value) + float3(x, y, z);
                            xV[x] = rand3d(cell);
                        }
                        yV[y] = lerp(xV[0], xV[1], interpolationValue.x);
                    }
                    zV[z] = lerp(yV[0], yV[1], interpolationValue.y);
                }
                float noise = lerp(zV[0], zV[1], interpolationValue.z) * 2 - 1;
                return noise;
            }

            fixed4 integrate(fixed4 sum, float diffuse, float density, fixed4 bgcol, float t)
            {
                fixed3 lighting = fixed3(0.65, 0.68, 0.7) * 1.3 + 0.5 * fixed3(0.7, 0.5, 0.3) * diffuse;
                fixed3 colrgb = lerp(fixed3(1.0, 0.95, 0.8), fixed3(0.65, 0.65, 0.65), density);
                fixed4 col = fixed4(colrgb.r, colrgb.g, colrgb.b, density);
                col.rgb *= lighting;
                col.rgb = lerp(col.rgb, bgcol.rgb, 1.0 - exp(-0.003 * t * t));
                col.a *= 0.5;
                col.rgb *= col.a;
                return sum + col * (1.0 - sum.a);
            }

            #define MARCH(steps, noiseMap, camPos, viewDir, bgcol, sum, depth, t){ \
                for(int i = 0; i < steps + 1; i++) \
                { \
                    if(t > depth) break; \
                    \
                    float3 pos = camPos + t * viewDir; \
                    \
                    if(pos.y < _MinHeight || pos.y > _MaxHeight || sum.a > 0.99) \
                    {\
                        t+= max(0.1, 0.02 * t);\
                        continue;\
                    }\
                    \
                    float density = noiseMap(pos);\
                    if(density > 0.01)\
                    {\
                    float diffuse = clamp((density - noiseMap(pos + 0.3 * _SunDir)) / 0.6, 0.0, 1.0);\
                    sum = integrate(sum, diffuse, density, bgcol, t);\
            \
                    }\
                    t+= max(0.1, 0.02 * t);\
                } \
            }

            #define noiseProcessing(N, P) 1.75 * N * saturate((_MaxHeight - P.y) / _FadeDist)

            float valueMap(float3 pos)
            {
                float3 p = pos;
                float f;
                f = 0.5 * noise3d(pos);
                return noiseProcessing(f, p);
            }

            fixed4 raymarch(float3 camPos, float3 viewDir, fixed4 bgcolor, float depth)
            {
                fixed4 col = fixed4(0, 0, 0, 0);
                float accumulate = 0;
                
                MARCH(_Steps, valueMap, camPos, viewDir, bgcolor, col, depth, accumulate);

                return clamp(col, 0, 1);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.view = o.worldPos - _WorldSpaceCameraPos;
                o.projPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float depth = 1;
                depth *= length(i.view);

                fixed4 col = fixed4(1, 1, 1, 0);
                fixed4 clouds = raymarch(_WorldSpaceCameraPos, normalize(i.view) * _StepScale, col, depth);
                fixed3 res = col * (1.0 - clouds.a) + clouds.xyz;
                return fixed4(res, clouds.a);
            }
            ENDCG
        }
    }
}