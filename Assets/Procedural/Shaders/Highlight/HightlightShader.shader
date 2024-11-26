Shader "Unlit/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness", Vector) = (0,0,0,0)
    }
    SubShader
    {

        //OUTLINE PASS
        Pass
        {
            Tags
            {
                "Queue"="Transparent"
            }
            Cull Front
            
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
            };

            fixed4 _OutlineColor;
            float4 _OutlineThickness;
            

            float3 outline(float4 vertPos)
            {
                float3x3 scale = float3x3(
                    1 + _OutlineThickness.x, 0, 0,
                    0, 1 + _OutlineThickness.y, 0,
                    0, 0, 1 + _OutlineThickness.z
                );

                return mul(scale, vertPos);
            }

            v2f vert(appdata v)
            {
                v2f o;
                float3 scale = outline(v.vertex);

                o.pos = UnityObjectToClipPos(scale);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}