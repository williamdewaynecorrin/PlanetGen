Shader "RobotCrossing/PlanetHeightmapColorFrag"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
				float3 localvertex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			// -- color by elevation
			float2 _ElevationMinMax;
			bool _HasBiomes;

			float mag(float3 v)
			{
				return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
			}

			float ilerp(float a, float b, float v)
			{
				return (v - a) / (b - a);
			}

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float elevation = ilerp(_ElevationMinMax.x, _ElevationMinMax.y, mag(i.localvertex));
				float2 uvs = float2(elevation, elevation);
				if (_HasBiomes)
					uvs.y = i.uv.x;

				fixed4 c = tex2D(_MainTex, uvs);
                return c;
            }
            ENDCG
        }
    }
}