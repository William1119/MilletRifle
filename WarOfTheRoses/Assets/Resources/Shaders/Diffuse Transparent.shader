Shader "Mobile/Diffuse Transparent" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" { }
		_Alpha ("Opacity", Range(0.001, 1)) = 0.5
	}
	
	SubShader {
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd nodirlightmap nolightmap exclude_path:prepass
		#pragma target 2.0
		
		sampler2D _MainTex;
		fixed _Alpha;
		struct Input {
			float2 uv_MainTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = _Alpha;
		}
		ENDCG
    }

	Fallback "Mobile/Diffuse"
}
