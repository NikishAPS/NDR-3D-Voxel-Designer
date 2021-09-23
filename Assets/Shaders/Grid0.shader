Shader "Voxels/Grid0"
{
    Properties
    {
        _Size("Size", float) = 1.0
        _Width("Width", float) = 1.0

        [Space(8.0)]

        _Color("Grid Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _BackgroundColor("Background Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Cull Off
        ZTest LEqual
        Offset -1, -1
        Blend SrcAlpha OneMinusSrcAlpha

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

            uniform float _Size;
            uniform float _Width;

            uniform float4 _Color;
            uniform float4 _BackgroundColor;

            v2f vert (vIn i)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(i.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, i.vertex);
                o.uv = worldPos.xz;

                return o;
            }

            float Grid(float2 uv, float size, float width)
            {
                width *= 0.025;
                uv = smoothstep(width, width * 0.1, abs(frac(uv / size + width) - width));
                return max(uv.x, uv.y);
            }

            float4 frag(v2f i) : SV_Target
            {   
                float grid = Grid(i.uv, _Size, _Width);
                float gridIntensity = grid * _Color.a;

                float4 gridColor;
                gridColor.rgb = lerp(float3(gridIntensity, gridIntensity, gridIntensity), _Color.rgb, gridIntensity);
                gridColor.a = _Color.a * gridIntensity;

                float4 backgroundColor;
                backgroundColor.rgb = _BackgroundColor.rgb;
                backgroundColor.a = _BackgroundColor.a * (1.0 - gridIntensity);

                return lerp(gridColor, backgroundColor, lerp(backgroundColor.a, 1.0 - gridColor.a, gridIntensity));
            }
ENDCG
        }
    }
}