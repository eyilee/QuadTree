Shader "Ng/NgCapsule2D"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Length ("Length", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _Length;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID (v);
                o.vertex = UnityObjectToClipPos (v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = float2 (i.uv.x, i.uv.y * (1 + _Length));
                float radius = 0.5;
                float2 v0 = float2 (radius, radius);
                float2 v1 = float2 (radius, radius + _Length);
                if (uv.y <= v0.y) {
                    return lerp (_Color, float4 (0, 0, 0, 0), step (radius, distance (v0, uv)));
                }
                else if (uv.y >= v1.y) {
                    return lerp (_Color, float4 (0, 0, 0, 0), step (radius, distance (v1, uv)));
                }
                else {
                    return _Color;
                }
            }
            ENDCG
        }
    }
}
