Shader "Ng/NgOutline"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Bound ("Bound", Vector) = (0, 0, 1, 1)
        _Size ("Size", Float) = 0.01
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float4 _Bound;
            float _Size;

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dx = min(distance(i.worldPos.x, _Bound.x - (_Bound.z * 0.5)),
                    distance(i.worldPos.x, _Bound.x + (_Bound.z * 0.5)));
                float dy = min(distance(i.worldPos.y, _Bound.y + (_Bound.w * 0.5)),
                    distance(i.worldPos.y, _Bound.y - (_Bound.w * 0.5)));
                float d = min(dx, dy);
                return lerp(_Color, fixed4(0, 0, 0, 0), step(_Size, d));
            }
            ENDCG
        }
    }
}
