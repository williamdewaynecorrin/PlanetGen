Shader "Hidden/DepthOfField"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _CameraDepthTexture;
	sampler2D _CocTex;
	sampler2D _DofTex;

	float _FocusDistance;
	float _FocusRange;
	float _BokehRadius;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always

		Pass // coc 0
		{
			CGPROGRAM
			 
			#pragma vertex vert
			#pragma fragment frag

			half frag(v2f i) : SV_Target
			{
				half depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
				depth = LinearEyeDepth(depth);
				float coc = (depth - _FocusDistance) / _FocusRange;
				coc = clamp(coc, -1, 1) * _BokehRadius;

				return coc;
			}

			ENDCG
		}

		Pass // prefilter 1
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half Weigh(half3 c)
			{
				return 1.0 / (1 + max(c.r, max(c.g, c.b)));
			}

			half4 frag(v2f i) : SV_Target
			{
				float4 o = _MainTex_TexelSize.xyxy * float2(-0.5, 0.5).xxyy;

				half3 s0 = tex2D(_MainTex, i.uv + o.xy).rgb;
				half3 s1 = tex2D(_MainTex, i.uv + o.zy).rgb;
				half3 s2 = tex2D(_MainTex, i.uv + o.xw).rgb;
				half3 s3 = tex2D(_MainTex, i.uv + o.zw).rgb;
				half w0 = Weigh(s0);
				half w1 = Weigh(s1);
				half w2 = Weigh(s2);
				half w3 = Weigh(s3);

				half3 color = s0 * w0 + s1 * w1 + s2 * w2 + s3 * w3;
				color /= max(w0 + w1 + w2 + w3, 0.0001);

				half coc0 = tex2D(_CocTex, i.uv + o.xy).r;
				half coc1 = tex2D(_CocTex, i.uv + o.zy).r;
				half coc2 = tex2D(_CocTex, i.uv + o.xw).r;
				half coc3 = tex2D(_CocTex, i.uv + o.zw).r;

				half cocmin = min(coc0, min(coc1, min(coc2, coc3)));
				half cocmax = max(coc0, max(coc1, max(coc2, coc3)));
				half coc = cocmax >= -cocmin ? cocmax : cocmin;

				return half4(color, coc);
			}

			ENDCG
		}

		Pass // bokeh 2
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			// -- for sampling a disc
			// From https://github.com/Unity-Technologies/PostProcessing/
			// blob/v2/PostProcessing/Shaders/Builtins/DiskKernels.hlsl
			static const int kernelSampleCountSmall = 16;
			static const float2 kernelSmall[kernelSampleCountSmall] = {
				float2(0, 0),
				float2(0.54545456, 0),
				float2(0.16855472, 0.5187581),
				float2(-0.44128203, 0.3206101),
				float2(-0.44128197, -0.3206102),
				float2(0.1685548, -0.5187581),
				float2(1, 0),
				float2(0.809017, 0.58778524),
				float2(0.30901697, 0.95105654),
				float2(-0.30901703, 0.9510565),
				float2(-0.80901706, 0.5877852),
				float2(-1, 0),
				float2(-0.80901694, -0.58778536),
				float2(-0.30901664, -0.9510566),
				float2(0.30901712, -0.9510565),
				float2(0.80901694, -0.5877853),
			};

			static const int kernelSampleCountMedium = 22;
			static const float2 kernelMedium[kernelSampleCountMedium] = {
				float2(0, 0),
				float2(0.53333336, 0),
				float2(0.3325279, 0.4169768),
				float2(-0.11867785, 0.5199616),
				float2(-0.48051673, 0.2314047),
				float2(-0.48051673, -0.23140468),
				float2(-0.11867763, -0.51996166),
				float2(0.33252785, -0.4169769),
				float2(1, 0),
				float2(0.90096885, 0.43388376),
				float2(0.6234898, 0.7818315),
				float2(0.22252098, 0.9749279),
				float2(-0.22252095, 0.9749279),
				float2(-0.62349, 0.7818314),
				float2(-0.90096885, 0.43388382),
				float2(-1, 0),
				float2(-0.90096885, -0.43388376),
				float2(-0.6234896, -0.7818316),
				float2(-0.22252055, -0.974928),
				float2(0.2225215, -0.9749278),
				float2(0.6234897, -0.7818316),
				float2(0.90096885, -0.43388376),
			};

			half Weigh(half coc, half radius)
			{
				return saturate((coc - radius + 2) * 0.5);
			}

			half4 frag(v2f i) : SV_Target
			{
				half coc = tex2D(_MainTex, i.uv).a;

				// -- background and foreground colors and weight
				half3 bgcolor = 0;
				half3 fgcolor = 0;
				half3 bgweight = 0;
				half3 fgweight = 0;

				for (int k = 0; k < kernelSampleCountMedium; ++k)
				{
					float2 o = kernelMedium[k] * _BokehRadius;
					half radius = length(o);
					o *= _MainTex_TexelSize.xy;
					half4 s = tex2D(_MainTex, i.uv + o);

					// -- background weight and color
					half bgw = Weigh(max(0, min(s.a, coc)), radius);
					bgcolor += s.rgb * bgw;
					bgweight += bgw;

					// -- foreground weight and color
					half fgw = Weigh(-s.a, radius);
					fgcolor += s.rgb * fgw;
					fgweight += fgw;
				}
				bgcolor *= 1.0 / (bgweight + (bgweight == 0));
				fgcolor *= 1.0 / (fgweight + (fgweight == 0));

				half combine = min(1, fgweight * UNITY_PI / kernelSampleCountMedium);
				half3 color = lerp(bgcolor, fgcolor, combine);
				return half4(color, combine);
			}

			ENDCG
		}

		Pass // postfilter 3
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				float4 o = _MainTex_TexelSize.xyxy * float2(-0.5, 0.5).xxyy;
				half4 s =
					tex2D(_MainTex, i.uv + o.xy) +
					tex2D(_MainTex, i.uv + o.zy) +
					tex2D(_MainTex, i.uv + o.xw) +
					tex2D(_MainTex, i.uv + o.zw);

				return s * 0.25;
			}

			ENDCG
		}

		Pass // combine 4
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				half4 source = tex2D(_MainTex, i.uv);
				half coc = tex2D(_CocTex, i.uv).r;
				half4 dof = tex2D(_DofTex, i.uv);

				half dofstrength = smoothstep(0.1, 1, abs(coc));
				half3 color = lerp(source.rgb, dof.rgb, dofstrength + dof.a - dofstrength * dof.a);

				return half4(color, source.a);
			}

			ENDCG
		}

		Pass // debug 5
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				half4 source = tex2D(_MainTex, i.uv);
				half3 coc = tex2D(_CocTex, i.uv).rrr;
				half4 dof = tex2D(_DofTex, i.uv);

				half3 color = coc + dof.rgb * 0.025;

				return half4(color, source.a);
			}

			ENDCG
		}
    }
}
