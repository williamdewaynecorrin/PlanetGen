Shader "RobotCrossing/Camera/Bloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _SourceTex;

	half4 _Filter;
	half _Intensity;
	half2 _SampleDelta;

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

	half3 Sample(float2 uv)
	{
		return tex2D(_MainTex, uv).rgb;
	}

	half3 SampleBox(float2 uv, float delta)
	{
		float4 o = _MainTex_TexelSize.xyxy * float2(-delta, delta).xxyy;
		half3 s = Sample(uv + o.xy) + Sample(uv + o.zy) +
				  Sample(uv + o.xw) + Sample(uv + o.zw);
		return s * 0.25;
	}

	half3 Prefilter(half3 color)
	{
		half brightness = max(color.r, max(color.g, color.b));
		half soft = brightness - _Filter.y;
		soft = clamp(soft, 0, _Filter.z);
		soft = soft * soft * _Filter.w;
		half contribution = max(soft, brightness - _Filter.x);
		contribution /= max(0.00001, brightness);

		return color * contribution;
	}

	ENDCG

	SubShader
	{
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always

		Pass // prefilter pass 0
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				return half4(Prefilter(SampleBox(i.uv, _SampleDelta.x)), 1);
			}

			ENDCG
		}

		Pass // downsample pass 1
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

            half4 frag (v2f i) : SV_Target
            {
				return half4(SampleBox(i.uv, _SampleDelta.x), 1);
            }

            ENDCG
        }

		Pass // upsample pass 2
		{
			Blend One One
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				return half4(SampleBox(i.uv, _SampleDelta.y), 1);
			}

			ENDCG
		}

		Pass // final upsample pass 3
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				half4 c = tex2D(_SourceTex, i.uv);
				c.rgb += _Intensity * half3(SampleBox(i.uv, _SampleDelta.y));
				return c;
			}

			ENDCG
		}

		Pass // debug visualize pass 4
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			half4 frag(v2f i) : SV_Target
			{
				return half4(_Intensity * SampleBox(i.uv, 0.5), 1);
			}

			ENDCG
		}
    }
}
