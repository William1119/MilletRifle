Shader "Custom/Unlit_Dissolve" 
{
    Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
	}

	Category 
	{
		SubShader 
		{
			Pass 
			{
				Tags { "Queue"="Transparent" "RenderType"="Transparent" "LIGHTMODE"="Always" }
				Fog {Mode Off}
				Cull Back
				ZWrite On
				Lighting Off
				ColorMask RGB
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 
				#pragma target 3.0

				sampler2D _MainTex;
				sampler2D _DissolveTex;
				float4 _DissolveTex_ST;

				float _DissolvePower;
				float4 _DissolveEdge;
				fixed4 _DissolveColor0;
				fixed4 _DissolveColor1;
				fixed4 _DissolveColor2;
				struct appdata 
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f 
				{
					float4 pos : POSITION;
					float4 uv : TEXCOORD0;
				};
		
				v2f vert(appdata v) 
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv.xy = v.uv;
					o.uv.zw = TRANSFORM_TEX(v.uv, _DissolveTex);
					return o;
				}
				fixed4 frag(v2f i) : COLOR 
				{ 
					float4 texDis = tex2D(_DissolveTex, i.uv.zw);
					float dis = _DissolvePower - texDis.r;
					fixed4 ret;
					if(dis < 0)
					{
						if(dis > _DissolveEdge.z)
							ret.rgb = _DissolveColor0.rgb;
						if(dis <= _DissolveEdge.z && dis > _DissolveEdge.y)
							ret.rgb = _DissolveColor1.rgb;
						if(dis <= _DissolveEdge.y && dis > _DissolveEdge.x)
							ret.rgb = _DissolveColor2.rgb;
						if(dis <= _DissolveEdge.x)
							discard;
					}
					else
						ret = tex2D(_MainTex, i.uv.xy); 
					ret.a = 1;
					return ret;
				}
				ENDCG
			}
		}
	}
}