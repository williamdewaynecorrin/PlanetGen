Shader "RobotCrossing/VoxelTerrain"
{
	Properties
	{
		_GroundTex("Ground Texture", 2D) = "white" {}
		_GroundTint("Ground Tint Color", Vector) = (1, 1, 1, 1)
		_CliffTex("Cliff Texture", 2D) = "white" {}
		_CliffTint("Cliff Tint Color", Vector) = (1, 1, 1, 1)
		_TranslucentGain("Translucent Gain", Float) = 1
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	#include "Autolight.cginc"

	// -- variables and vertex structs / shader are global to all passes
	sampler2D _GroundTex;
	float4 _GroundTex_ST;
	sampler2D _CliffTex;
	float4 _CliffTex_ST;

	float4 _GroundTint;
	float4 _CliffTint;
	float _TranslucentGain;

	struct appdata
	{
		float4 pos : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uvground : TEXCOORD0;
		float2 uvcliff : TEXCOORD1;
		LIGHTING_COORDS(2,3)
		float3 normal : NORMAL;
		float4 pos : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v.uv.x = v.pos.x / 20;
		v.uv.y = v.pos.z / 20;

		v2f o;
		o.pos = UnityObjectToClipPos(v.pos);
		o.uvground = TRANSFORM_TEX(v.uv, _GroundTex);
		o.uvcliff = TRANSFORM_TEX(v.uv, _CliffTex);
		o.normal = normalize(mul((float3x3)UNITY_MATRIX_M, v.normal));
		TRANSFER_VERTEX_TO_FRAGMENT(o);

		return o;
	}

	ENDCG

	// -- main pass
	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"LightMode" = "ForwardBase"
		}

		//UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_fwdbase // allow receiving shadows

			#include "Lighting.cginc"

			float4 frag(v2f i) : SV_Target
			{
				float3 normal = i.normal;
				float shadow = LIGHT_ATTENUATION(i);
				float ndotl = saturate(saturate(dot(normal, _WorldSpaceLightPos0)) + _TranslucentGain) * shadow;
				float3 ambient = ShadeSH9(float4(normal, 1));
				float4 intensity = ndotl * _LightColor0 + float4(ambient, 1);

				float4 groundsample = tex2D(_GroundTex, i.uvground);
				float4 groundcol = groundsample * intensity * _GroundTint;
				float4 cliffsample = tex2D(_CliffTex, i.uvcliff);
				float4 cliffcol = cliffsample * intensity * _CliffTint;

				float gn = saturate(dot(normal, float3(0, 1, 0)));
				float4 texcol = abs(gn) > 0.9 ? groundcol : cliffcol;
				float4 col = texcol * (unity_AmbientSky) + unity_FogColor * unity_FogParams.x; // assumes exp^2 fog

				if (ndotl < 0.25)
					col *= 0.25;
				else if (ndotl < 0.5)
					col *= 0.65;
				else if (ndotl < 0.8)
					col *= 0.9;

				return col;
			}

			ENDCG
		}

		//Pass // -- shadow pass
		//{
		//	Tags
		//	{
		//		"LightMode" = "ShadowCaster"
		//	}

		//	CGPROGRAM

		//	#pragma vertex vert // included in CustomTessellation.cginc
		//	#pragma fragment frag
		//	#pragma target 4.6
		//	#pragma multi_compile_shadowcaster

		//	float4 frag(v2f input) : SV_TARGET
		//	{
		//		SHADOW_CASTER_FRAGMENT(input)
		//	}

		//	ENDCG
		//}
	}
}