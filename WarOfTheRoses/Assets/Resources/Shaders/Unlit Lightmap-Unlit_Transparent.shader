Shader "Unlit/Unlit_Transparent"
{
    Properties
    {
       _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
       _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader
    { 
        Tags 
        { 
            "QUEUE"="Transparent" 
            "IGNOREPROJECTOR"="true" 
            "RenderType"="Transparent" 
        }
        Pass
        {
            Name "FORWARDBASE"
            Tags 
            { 
                "QUEUE"="Transparent" 
                "IGNOREPROJECTOR"="true" 
                "RenderType"="Transparent" 
            }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            half4 frag (v2f IN) : COLOR
            {
                fixed4 c_1;
                fixed4 tmpvar_2;
                tmpvar_2 = tex2D(_MainTex, IN.texcoord);
                c_1.xyz = (tmpvar_2.xyz * (_Color.xyz * 2.0));
                c_1.w = (tmpvar_2.w * _Color.w);
                return c_1;
            }
            ENDCG
        }
    }

    Fallback Off
}