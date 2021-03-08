Shader "RobotCrossing/PlanetHeightmapColor"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
			float2 uv_custom : TEXCOORD0;
			float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;

		// -- color by elevation
		float2 _ElevationMinMax;
		bool _HasBiomes;

        UNITY_INSTANCING_BUFFER_START(Props)
        // -- put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float mag(float3 v)
		{
			return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
		}

		float ilerp(float a, float b, float v)
		{
			return (v - a) / (b - a);
		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float3 vertexpos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			float min = _ElevationMinMax.x;
			float max = _ElevationMinMax.y;
			float elevation = ilerp(min, max, mag(vertexpos));

			float2 uvs = float2(elevation, elevation);
			if (_HasBiomes)
				uvs.y = IN.uv_custom.x;

			fixed4 c = tex2D(_MainTex, uvs);
			o.Albedo = c.xyz;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}