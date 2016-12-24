Shader "Mobile/Diffuse Rimlight" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_RimColor ("Rim Light Color", Color) = (1,1,1,1.0)
    	_RimPower ("Rim Light Range", Range(0.5, 10.0)) = 4.0
    	_RimBrightness ("Rim Light Brightness", Range(0.1, 10.0)) = 2.0
	}
	
	SubShader {
		UsePass "Character/NGCharShader_All/OUTLINE"
		
		Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd nodirlightmap nolightmap exclude_path:prepass
		#pragma target 2.0
		
		fixed4 _RimColor;
    	float _RimPower;
    	float _RimBrightness;

		sampler2D _MainTex;
		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = 1.0f;

			half rim = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower);
			o.Emission = _RimColor.rgb * _RimBrightness * _RimColor.a * rim;
		}
		ENDCG
    }

	Fallback "Mobile/Diffuse"
}
