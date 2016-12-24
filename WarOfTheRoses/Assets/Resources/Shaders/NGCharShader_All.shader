Shader "Character/NGCharShader_All" 
{
	Properties 
	{
	}
	CGINCLUDE
	#include "UnityCG.cginc"

	#if _SELFILLUMINE_ON
	sampler2D _IlluminTex;
	float4 _IlluminTex_ST;
	fixed4 _IlluminColor;
	float _IlluminPower;
	float _IlluminSpeed;
	#endif
	struct appdata_spec
	{
		fixed4 vertex : POSITION;
		fixed3 normal : NORMAL;
		fixed4 texcoord : TEXCOORD0;
	};
	struct v2f_spec
	{
		fixed4 position : SV_POSITION;
		fixed2 uv : TEXCOORD0;
		fixed3 wposition : TEXCOORD1;
		fixed3 wnormal : TEXCOORD2;
		fixed3 SH_shading : TEXCOORD3;
		#if _SELFILLUMINE_ON
		fixed3 illumColor : COLOR0;
		#endif
	};
	fixed3 ungamma20 (fixed3 input)
	{
		return input*input;
	}
	fixed3 gamma20 (fixed3 input)
	{	
		return sqrt(input);
	}
	v2f_spec vert_spec(appdata_spec v)
	{
		v2f_spec o;
		o.position = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;

		float3 worldNormal = normalize(mul(fixed4(v.normal.xyz, 0.0), _World2Object).xyz);
		o.SH_shading = ShadeSH9(float4(worldNormal,1));
				
		o.wnormal = worldNormal;
		o.wposition = mul(_Object2World, v.vertex);
	#if _SELFILLUMINE_ON
		o.illumColor.rgb = _IlluminColor.rgb * _IlluminPower * sin(3.141592653*frac(_Time.y / _IlluminSpeed));
	#endif	
		return o;
	}

	ENDCG
	SubShader 
	{
		Pass 
		{
			Name "BASE"
			Tags { "RenderType"="Opaque" "LIGHTMODE"="ForwardBase" }
			Fog {Mode off}
			Cull Back
			LOD 200
			CGPROGRAM
			#pragma vertex vert_spec
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma multi_compile _SELFILLUMINE_OFF _SELFILLUMINE_ON

			sampler2D _MainTex;
			fixed4 _DiffuseColor;
			fixed _Specular;
			fixed _Exposure;
			fixed _LightProbeIntensity;

			fixed4 frag (v2f_spec i) : COLOR
			{
				fixed4 Dt = tex2D(_MainTex, i.uv);

				Dt.rgb = ungamma20(Dt.rgb);

				fixed3 Diffuse = Dt.rgb * ungamma20(_DiffuseColor);
				
				fixed3 lightDir = -normalize(fixed3(-0.3, 0.2, 0.4));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.wposition);
				fixed3 halfView = normalize(lightDir + viewDir);
				fixed cosTh = saturate(dot(i.wnormal, halfView));
				fixed cosTi = saturate(dot(i.wnormal, lightDir));

				fixed3 Ci = Diffuse * ((1 + _Specular * pow(cosTh, 16)) * cosTi + i.SH_shading * _LightProbeIntensity);
			
				Ci = gamma20(Ci * pow(2, _Exposure));

				#if _SELFILLUMINE_ON
				Ci.rgb += tex2D(_IlluminTex, TRANSFORM_TEX(i.uv, _IlluminTex)).r * i.illumColor.rgb;
				#endif
				fixed4 col = fixed4(Ci,Dt.a);
				return col;
			}
			ENDCG
		}
		Pass 
		{
			Name "ZONLY"
			Tags {"Queue"="Transparent" "RenderType"="Transparent"}
			Cull Back
			Lighting Off
			ColorMask 0
			ZWrite On
		}
		Pass 
		{
			Name "TRANSPARENT"
			Tags { "Queue"="Transparent + 1" "RenderType"="Transparent" "LIGHTMODE"="ForwardBase"}
			Fog {Mode off}
			Cull Back

			ZWrite Off
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert_spec
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma multi_compile _SELFILLUMINE_OFF _SELFILLUMINE_ON

			sampler2D _MainTex;
			fixed4 _DiffuseColor;
			fixed _Specular;
			fixed _Exposure;
			fixed _LightProbeIntensity;
			fixed _Alpha;

			fixed4 frag (v2f_spec i) : COLOR
			{
				fixed4 Dt = tex2D(_MainTex, i.uv);

				Dt.rgb = ungamma20(Dt.rgb);

				fixed3 Diffuse = Dt.rgb * ungamma20(_DiffuseColor);
				
				fixed3 lightDir = -normalize(fixed3(-0.3, 0.2, 0.4));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.wposition);
				fixed3 halfView = normalize(lightDir + viewDir);
				fixed cosTh = saturate(dot(i.wnormal, halfView));
				fixed cosTi = saturate(dot(i.wnormal, lightDir));

				fixed3 Ci = Diffuse * ((1 + _Specular * pow(cosTh, 16)) * cosTi + i.SH_shading * _LightProbeIntensity);
			
				Ci = gamma20(Ci * pow(2, _Exposure));
				#if _SELFILLUMINE_ON
				Ci.rgb += tex2D(_IlluminTex, TRANSFORM_TEX(i.uv, _IlluminTex)).r * i.illumColor.rgb;
				#endif
				fixed4 col = fixed4(Ci,Dt.a * _Alpha);
				return col;
			}

			ENDCG
		} 
		Pass 
		{
			Name "OUTLINE"
			Tags { "Queue"="Transparent + 1" "RenderType"="Transparent" "LIGHTMODE"="Always" }
			Cull Front
			ZWrite On
			Lighting Off
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			struct appdata 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
			{
				float4 pos : POSITION;
				float4 color : COLOR;
			};
	
			uniform float _Outline;
			uniform float4 _OutlineColor;
	
			v2f vert(appdata v) 
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				float3 norm = mul ((float3x3)UNITY_MATRIX_MVP, v.normal);
				//float2 offset = TransformViewToProjection(norm.xy);
				o.pos.xy += norm.xy * o.pos.z * _Outline;
				o.color = _OutlineColor;
				return o;
			}
			half4 frag(v2f i) : COLOR 
			{ 
				return i.color; 
			}
			ENDCG
		}
		Pass 
		{
			Name "SHADOW"
			Tags {"QUEUE"="AlphaTest+1" "RenderType"="Opaque" "LightMode" = "Always"}
			Fog {Mode off}

			Stencil 
			{
                Ref 16
                Comp NotEqual
                Pass Replace
            }
			Cull Back
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 

			float4 _WorldPos;
			float4 _ShadowPlane;
			float4 _ShadowProjDir;
			float4 _ShadowFadeParams;

			struct appdata 
			{
				fixed4 vertex : POSITION;
			};
			struct v2f 
			{
				fixed4 position : SV_POSITION;
				fixed3 worldPos : TEXCOORD0;
				fixed3 shadowProjPos : TEXCOORD1;
			};
			v2f vert (appdata v)
			{
				v2f o;
				float3 wPos = mul(_Object2World, v.vertex).xyz;
				o.shadowProjPos = wPos - _ShadowProjDir.xyz * ((dot(_ShadowPlane.xyz, wPos) - _ShadowPlane.w) / dot(_ShadowPlane.xyz, _ShadowProjDir.xyz));
				o.worldPos = _WorldPos.xyz;
				o.position = mul(UNITY_MATRIX_VP, fixed4(o.shadowProjPos, 1.0f));
				return o;
			}
			fixed4 frag (v2f i) : COLOR
			{
				float3 posToPlane = i.worldPos - i.shadowProjPos;
				float shadow = (pow ((1.0 - clamp (((sqrt( dot (posToPlane, posToPlane)) * _ShadowFadeParams.w) - _ShadowFadeParams.x), 0.0, 1.0)), _ShadowFadeParams.y) * _ShadowFadeParams.z);
				return fixed4(0,0,0,shadow);
			}
			ENDCG
		}
		Pass 
		{
			Name "RIMLIGHT"
			Tags {"RenderType"="Opaque" "LightMode" = "ForwardBase"}
			Fog {Mode off}
			Cull Back
			CGPROGRAM
			#pragma vertex vert_spec
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			sampler2D _MainTex;
			fixed4 _DiffuseColor;
			fixed _Specular;
			fixed _Exposure;
			fixed _LightProbeIntensity;

			fixed4 _RimColor;
    		float _RimPower;
    		float _RimBrightness;
			fixed4 frag (v2f_spec i) : COLOR
			{
				fixed4 Dt = tex2D(_MainTex, i.uv);

				Dt.rgb = ungamma20(Dt.rgb);

				fixed3 Diffuse = Dt.rgb * ungamma20(_DiffuseColor);
				
				fixed3 lightDir = -normalize(fixed3(-0.3, 0.2, 0.4));
				fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.wposition);
				fixed3 halfView = normalize(lightDir + viewDir);
				fixed cosTh = saturate(dot(i.wnormal, halfView));
				fixed cosTi = saturate(dot(i.wnormal, lightDir));
				
				float rim = pow(1.0 - saturate(dot(viewDir, i.wnormal)), _RimPower);
			
				fixed3 Ci = Diffuse * ((1 + _Specular * pow(cosTh, 16)) * cosTi + i.SH_shading * _LightProbeIntensity);
				Ci = gamma20(Ci * pow(2, _Exposure)) + _RimColor.rgb * _RimBrightness * _RimColor.a * rim;;

				fixed4 col = fixed4(Ci,Dt.a);
				return col;
			}
			ENDCG
		}  
	} 
	FallBack "Diffuse"
}
