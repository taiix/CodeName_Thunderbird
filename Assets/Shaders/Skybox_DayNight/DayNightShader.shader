Shader "Skybox/DayNightShader"
{
    Properties
    {
        _MainDay ("Main Day", Cube) = "" {}
        _MainMidDay ("Main Mid Day", Cube) = "" {}
        _MainBeforeNight ("Main Before Night", Cube) = "" {}
        _MainEarlyNight ("Main Early Night", Cube) = "" {}
        _MainMidNight ("Main Mid Night", Cube) = "" {}
        _MainBeforeDay ("Main Before Day", Cube) = "" {}
        _TimeOfDay ("Time of Day", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Background" "Queue"="Background" }
        Cull Off
        ZWrite Off
        Fog { Mode Off }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            samplerCUBE _MainDay;
            samplerCUBE _MainMidDay;
            samplerCUBE _MainBeforeNight;
            samplerCUBE _MainEarlyNight;
            samplerCUBE _MainMidNight;
            samplerCUBE _MainBeforeDay;

            float _TimeOfDay;

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 direction : TEXCOORD0;
            };

            v2f vert (float4 vertex : POSITION)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);

                float3 worldDir = mul(unity_ObjectToWorld, vertex).xyz;

                o.direction = worldDir;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 dir = normalize(i.direction);

                // Normalize _TimeOfDay to [0,1]
                float time = frac(_TimeOfDay);

                // Calculate the total number of intervals
                float intervals = 6.0;

                // Scale time to intervals
                float t = time * intervals; // t ranges from 0 to 6
                float index = floor(t);
                float blendB = frac(t);
                float blendA = 1.0 - blendB;

                // Initialize blend factors for each cubemap
                float blend0 = step(0.0, 1.0 - abs(index - 0.0)) * blendA;
                float blend1 = step(0.0, 1.0 - abs(index - 1.0)) * blendA;
                float blend2 = step(0.0, 1.0 - abs(index - 2.0)) * blendA;
                float blend3 = step(0.0, 1.0 - abs(index - 3.0)) * blendA;
                float blend4 = step(0.0, 1.0 - abs(index - 4.0)) * blendA;
                float blend5 = step(0.0, 1.0 - abs(index - 5.0)) * blendA;

                // Add blendB to the next cubemap in sequence
                blend0 += step(0.0, 1.0 - abs(index - (-1.0))) * blendB;
                blend1 += step(0.0, 1.0 - abs(index - 0.0)) * blendB;
                blend2 += step(0.0, 1.0 - abs(index - 1.0)) * blendB;
                blend3 += step(0.0, 1.0 - abs(index - 2.0)) * blendB;
                blend4 += step(0.0, 1.0 - abs(index - 3.0)) * blendB;
                blend5 += step(0.0, 1.0 - abs(index - 4.0)) * blendB;

                // Sample all cubemaps
                fixed4 col0 = texCUBE(_MainMidNight, dir);
                fixed4 col1 = texCUBE(_MainBeforeDay, dir);
                fixed4 col2 = texCUBE(_MainDay, dir);
                fixed4 col3 = texCUBE(_MainMidDay, dir);
                fixed4 col4 = texCUBE(_MainBeforeNight, dir);
                fixed4 col5 = texCUBE(_MainEarlyNight, dir);

                // Blend the sampled colors
                fixed4 col = col0 * blend0 + col1 * blend1 + col2 * blend2 + col3 * blend3 + col4 * blend4 + col5 * blend5;

                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
