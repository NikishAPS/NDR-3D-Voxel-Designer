Shader "Voxels/Extractor"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Off

        Pass
        {
CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct vIn
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;

            uniform float4 _Color;

            v2f vert(vIn i)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(i.vertex);
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);

                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.uv);

                float alpha = mainTex.a * _Color.a;

                float4 color;
                color.rgb = lerp(mainTex.rgb, _Color.rgb, alpha);
                color.a = alpha;

                return color;
            }
ENDCG
        }
    }
}
