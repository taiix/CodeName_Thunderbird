Shader "Custom/BlendSkybox"
{
    Properties
    {
        _Skybox1 ("Skybox 1", Cube) = ""
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Skybox1;
            samplerCUBE _Skybox2;
            float _BlendFactor;

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldDir = normalize(mul((float3x3)unity_ObjectToWorld, v.vertex.xyz));
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color1 = texCUBE(_Skybox1, i.worldDir);
              
                return color1;
            }
            ENDCG
        }
    }
}
