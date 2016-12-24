Shader "Custom/SelfIllumin/Diffuse" 
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _IlluminColor ("Self Illumination Color(RGB)", Color) = (1,1,1,1)
        _IlluminPower ("Illumination Power", Range(0,4)) = 1
        _IlluminTex ("Illumination Channel (A)", 2D) = "white" {}
    }
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
    
        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        sampler2D _IlluminTex;
        fixed4 _Color;
        fixed4 _IlluminColor;
        float _IlluminPower;
        struct Input {
            float2 uv_MainTex;
            float2 uv_IlluminTex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 c = tex * _Color;
            o.Albedo = c.rgb;
            o.Emission = c.rgb * tex2D(_IlluminTex, IN.uv_IlluminTex).r * _IlluminColor.rgb * _IlluminPower;
            o.Alpha = c.a;
        }
        ENDCG
    } 
    FallBack "Self-Illumin/VertexLit"
}
