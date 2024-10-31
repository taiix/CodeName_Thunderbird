Shader "Unlit/Clouds"
{
    Properties
    {
        _Cloud3D ("Texture", 3D) = "white" {}
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler3D _Cloud3D;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
             
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 uv = float3(i.uv, 0.5);
                float cloudDensity = tex3D(_Cloud3D, uv).r;
                return float4(cloudDensity, cloudDensity, cloudDensity, cloudDensity);
            }
            ENDCG
        }
    }
}
