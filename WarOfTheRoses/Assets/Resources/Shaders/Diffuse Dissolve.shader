Shader "Mobile/Diffuse_Dissolve" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" { }
	}
	
	SubShader {
		Tags { "RenderType" = "Opaque" }
		
		CGPROGRAM
		#pragma surface surf Lambert noforwardadd nodirlightmap nolightmap exclude_path:prepass
		#pragma target 2.0
		
		sampler2D _MainTex;
		sampler2D _DissolveTex;

		float _DissolvePower;
		float4 _DissolveEdge;
		fixed4 _DissolveColor0;
		fixed4 _DissolveColor1;
		fixed4 _DissolveColor2;

		struct Input {
			float2 uv_MainTex;
			float2 uv_DissolveTex;
		};
		
		void surf (Input IN, inout SurfaceOutput o) {
			float4 texDis = tex2D(_DissolveTex, IN.uv_DissolveTex);
			float dis = _DissolvePower - texDis.r;
			
			if(dis < 0)
			{
				if(dis > _DissolveEdge.z)
					o.Albedo = _DissolveColor0.rgb;
				if(dis <= _DissolveEdge.z && dis > _DissolveEdge.y)
					o.Albedo = _DissolveColor1.rgb;
				if(dis <= _DissolveEdge.y && dis > _DissolveEdge.x)
					o.Albedo = _DissolveColor2.rgb;
				if(dis <= _DissolveEdge.x)
					discard;
			}
			else 
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = 1.0f;
		}
		ENDCG
    }

	Fallback "Mobile/Diffuse"
}
