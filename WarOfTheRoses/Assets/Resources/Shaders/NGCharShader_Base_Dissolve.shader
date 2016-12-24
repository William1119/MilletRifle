Shader "Character/NGCharShader_Base_Dissolve" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DiffuseColor ("Diffuse Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Specular ("Specular", float) = 16
		_Exposure ("Global Exposure", float) = 1.0	
		_LightProbeIntensity ("Light Probe Intensity", float) = 4.0

		[Toggle] _SelfIllumine("Self Illumination On/Off", float) = 0
		_IlluminColor ("Self Illumination Color(RGB)", Color) = (1,1,1,1)
        _IlluminPower ("Illumination Power", Range(0,4)) = 1
        _IlluminTex ("Illumination Channel (A)", 2D) = "black" {}
		_IlluminSpeed ("Illumination flash speed", Range(10, 0.01)) = 1
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" "LIGHTMODE"="ForwardBase" }
		Pass 
		{
			Name "BASE"
			Tags { "RenderType"="Opaque" "LIGHTMODE"="ForwardBase" }
			Fog {Mode off}
			Cull Back
			LOD 200

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert_spec
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma multi_compile _SELFILLUMINE_OFF _SELFILLUMINE_ON
			#pragma target 3.0
				
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

			sampler2D _MainTex;
			fixed4 _DiffuseColor;
			fixed _Specular;
			fixed _Exposure;
			fixed _LightProbeIntensity;

			sampler2D _DissolveTex;
			float4 _DissolveTex_ST;
			float _DissolvePower;
			float4 _DissolveEdge;
			fixed4 _DissolveColor0;
			fixed4 _DissolveColor1;
			fixed4 _DissolveColor2;
			fixed4 frag (v2f_spec i) : COLOR
			{				
				float4 texDis = tex2D(_DissolveTex, TRANSFORM_TEX(i.uv, _DissolveTex));
				float dis = _DissolvePower - texDis.r;
				
				fixed4 ret;
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
				ret.rgb = Ci.rgb;
				ret.a = Dt.a;
				if(dis < 0)
				{
					ret.a = 1.0;
					if(dis > _DissolveEdge.z)
						ret.rgb = _DissolveColor0.rgb;
					if(dis <= _DissolveEdge.z && dis > _DissolveEdge.y)
						ret.rgb = _DissolveColor1.rgb;
					if(dis <= _DissolveEdge.y && dis > _DissolveEdge.x)
						ret.rgb = _DissolveColor2.rgb;
					if(dis <= _DissolveEdge.x)
						discard;
				}
				return ret;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
