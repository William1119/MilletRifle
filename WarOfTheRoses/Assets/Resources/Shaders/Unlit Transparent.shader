Shader "Custom/Unlit Transparent" 
{
    Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Alpha ("Opacity", Range(0.001, 1)) = 0.5
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
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest 

				sampler2D _MainTex;
				fixed _Alpha;
				struct appdata 
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f 
				{
					float4 pos : POSITION;
					float2 uv : TEXCOORD0;
				};
		
				v2f vert(appdata v) 
				{
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					return o;
				}
				fixed4 frag(v2f i) : COLOR 
				{ 
					return fixed4(tex2D(_MainTex, i.uv).rgb, _Alpha); 
				}
				ENDCG
			}
		}
	}
}