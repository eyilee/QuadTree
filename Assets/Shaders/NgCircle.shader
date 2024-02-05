Shader "Ng/NgCircle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
                float3 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float4 _Center;
            float _Radius;

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
                return lerp(_Color, fixed4(0, 0, 0, 0), step(_Radius, distance(_Center.xy, i.worldPos.xy)));
            }
            ENDCG
        }
    }
}
