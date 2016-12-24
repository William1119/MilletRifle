  Shader "Character/Alpha Shader" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader {
      Tags { "RenderType" = "Transparent" "Queue"="Transparent" "IgnoreProjector"="True"}
      
    // extra pass that renders to depth buffer only
     Pass {
        ZWrite On
        ColorMask 0
       }
          
      CGPROGRAM
      #pragma surface surf Lambert alpha nolightmap nodirlightmap  novertexlights
      struct Input {
          float2 uv_MainTex;
          float3 viewDir;
      };
      sampler2D _MainTex;
      float _Alpha;

      void surf (Input IN, inout SurfaceOutput o) {
      	  half4 basecol = tex2D (_MainTex, IN.uv_MainTex);
          o.Albedo = basecol;
          o.Alpha = _Alpha;
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }