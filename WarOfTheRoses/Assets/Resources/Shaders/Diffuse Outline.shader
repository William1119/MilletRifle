Shader "Mobile/Diffuse Outline" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
	}
	
	SubShader {
		UsePass "Character/NGCharShader_All/OUTLINE"
		
		Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd nodirlightmap nolightmap exclude_path:prepass
		#pragma target 2.0
		
		sampler2D _MainTex;
		struct Input {
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = 1.0f;
		}
		ENDCG
    }

	Fallback "Mobile/Diffuse"
}
