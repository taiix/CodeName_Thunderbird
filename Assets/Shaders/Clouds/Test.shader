Shader "Unlit/Test"
{
    Properties
    {
        _CloudNoiseTex("3D Noise Texture", 3D) = "" {}
        _DensityThreshold("Density Threshold", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
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
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            sampler3D _CloudNoiseTex;
            float _DensityThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 texCoords = frac(i.worldPos * 0.5 + 0.5);

                float density = tex3D(_CloudNoiseTex, texCoords).r;

                return density > _DensityThreshold ? float4(density, density, density, 1) : float4(0, 0, 0, 0);
            }
            ENDCG
        }
    }
}
