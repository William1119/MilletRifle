Shader "Unlit/Alpha Blend ZTestOff"
{
    Properties
    {
       _Color ("Main Color", Color) = (1,1,1,1)
       _Brightness ("Brightness", Float) = 1
       _MainTex ("Base (RGB)", 2D) = "white" {}
       _MaskTex ("Mask Texture (R)", 2D) = "white" {}
       _UVParam ("UV Param", Vector) = (0,0,1,1)
       _UVTile ("UV Tile", Vector) = (0,0,1,1)
       _Rotate ("UV Rotate", Range(0,360)) = 0
       _MaskUVParam ("Mask UV Param", Vector) = (0,0,1,1)
       _MaskRotate ("Mask UV Rotate", Range(0,360)) = 0
       [Toggle]  _Flow ("_Flow", Float) = 0
       _FlowUVParam ("_Flow UV Param", Vector) = (0,0,0,1)
       _FlowTex ("Flow Texture (RG)", 2D) = "black" {}
       _FlowRotate ("FlowRotate", Range(0,360)) = 0
      [KeywordEnum(Off,On)]  _SeparateAlpha ("_Separate Alpha", Float) = 0
       _AlphaTex ("Alpha Texture", 2D) = "white" {}
       _AlphaCtrl ("Alpha control", Float) = 1
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
            Name "EFFECT"
            Tags 
            { 
                "QUEUE"="Transparent" 
                "IGNOREPROJECTOR"="true" 
                "RenderType"="Transparent" 
            }
			ZTest False
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile MASK_OFF MASK_ON
			#pragma multi_compile _FLOW_OFF _FLOW_ON
			#pragma multi_compile _SEPARATEALPHA_OFF _SEPARATEALPHA_ON
			#pragma multi_compile _SCALE_OFF _SCALE_ON 
            struct appdata_t
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : POSITION;
                half4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;

            float4 _UVParam;
            float4 _UVTile;
            float _Rotate;
            fixed _AlphaCtrl;
            float4 _MainTex_ST;
            float _Brightness;
            fixed4 _Color;

            #ifdef _SCALE_ON
            float4 _Camera2World;
            float4 _ScalePosInWorld;
            half _ScaleSize;
            #endif

            #ifdef MASK_ON
            float4 _MaskUVParam;
            float _MaskRotate;
            #endif

            #ifdef _FLOW_ON
            float _FlowRotate;
            float4 _FlowUVParam;
            sampler2D _FlowTex;
            #endif

            #ifdef _SEPARATEALPHA_ON
            sampler2D _AlphaTex;
            #endif
            
            v2f vert (appdata_t v)
            {
                half4 tmpvar_1;
                tmpvar_1.xyz = v.color.xyz;
                float4 tmpvar_2;
                float2 tmpvar_3;

                #ifdef _SCALE_ON    
                float4 pos_2;
                float4 tmpvar_5;
                tmpvar_5 = (_Camera2World * mul(UNITY_MATRIX_MV, v.vertex));
                pos_2.w = tmpvar_5.w;
                pos_2.xyz = (tmpvar_5.xyz - _ScalePosInWorld.xyz);
                pos_2.xyz = (pos_2.xyz * _ScaleSize);
                pos_2.xyz = (pos_2.xyz + _ScalePosInWorld.xyz);
                #endif

                float uvRotate_4;
                uvRotate_4 = (_Rotate / 57.2958);
                float2 outUV_5;
                float tmpvar_6;
                tmpvar_6 = sin(uvRotate_4);
                float tmpvar_7;
                tmpvar_7 = cos(uvRotate_4);
                float2 tmpvar_8;
                tmpvar_8 = (((v.texcoord.xy * _MainTex_ST.xy) + _MainTex_ST.zw) - float2(0.5, 0.5));
                float2 tmpvar_9;
                tmpvar_9.x = ((tmpvar_8.x * tmpvar_7) - (tmpvar_8.y * tmpvar_6));
                tmpvar_9.y = ((tmpvar_8.x * tmpvar_6) + (tmpvar_8.y * tmpvar_7));
                outUV_5.x = (tmpvar_9.x * _UVParam.z);
                outUV_5.y = (tmpvar_9.y * _UVParam.w);
                float2 tmpvar_10;
                tmpvar_10 = ((outUV_5 + _UVParam.xy) + float2(0.5, 0.5));
                outUV_5.x = ((tmpvar_10.x * _UVTile.z) + _UVTile.x);
                outUV_5.y = ((tmpvar_10.y * _UVTile.w) + _UVTile.y);
                tmpvar_2.xy = outUV_5;
                tmpvar_2.zw = v.texcoord.xy;

                #ifdef MASK_ON
                float uvRotate_11;
                uvRotate_11 = (_MaskRotate / 57.2958);
                float2 outUV_12;
                float tmpvar_13;
                tmpvar_13 = sin(uvRotate_11);
                float tmpvar_14;
                tmpvar_14 = cos(uvRotate_11);
                float2 tmpvar_15;
                tmpvar_15 = (v.texcoord.xy - float2(0.5, 0.5));
                float2 tmpvar_16;
                tmpvar_16.x = ((tmpvar_15.x * tmpvar_14) - (tmpvar_15.y * tmpvar_13));
                tmpvar_16.y = ((tmpvar_15.x * tmpvar_13) + (tmpvar_15.y * tmpvar_14));
                outUV_12.x = (tmpvar_16.x * _MaskUVParam.z);
                outUV_12.y = (tmpvar_16.y * _MaskUVParam.w);
                outUV_12 = ((outUV_12 + _MaskUVParam.xy) + float2(0.5, 0.5));
                tmpvar_2.zw = outUV_12;
                #endif

                #ifdef _FLOW_ON
                float uvRotate_10;
                uvRotate_10 = (_FlowRotate / 57.2958);
                float tmpvar_11;
                tmpvar_11 = sin(uvRotate_10);
                float tmpvar_12;
                tmpvar_12 = cos(uvRotate_10);
                float2 tmpvar_17;
                tmpvar_17 = (v.texcoord.xy - float2(0.5, 0.5));
                float2 tmpvar_18;
                tmpvar_18.x = ((tmpvar_17.x * tmpvar_12) - (tmpvar_17.y * tmpvar_11));
                tmpvar_18.y = ((tmpvar_17.x * tmpvar_11) + (tmpvar_17.y * tmpvar_12));
                tmpvar_3 = ((tmpvar_18 + float2(0.5, 0.5)) + _FlowUVParam.xy);
                #endif

                tmpvar_1.w = (v.color.w * _AlphaCtrl);

                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                #ifdef _SCALE_ON
                o.vertex = mul(UNITY_MATRIX_VP, pos_2);
                #endif
                o.color = tmpvar_1;
                o.texcoord = tmpvar_2;
                o.texcoord1 = tmpvar_3;
                return o;
            }

            half4 frag (v2f IN) : COLOR
            {
                fixed4 color_1;
                fixed4 tmpvar_2;
                tmpvar_2 = tex2D(_MainTex, IN.texcoord.xy);
                color_1 = tmpvar_2;
                #ifdef _FLOW_ON
                half4 flow_2;
                fixed4 tmpvar_3;
                tmpvar_3 = tex2D(_FlowTex, IN.texcoord1.xy);
                flow_2 = tmpvar_3;
                fixed4 tmpvar_4;
                tmpvar_4 = (flow_2 + _FlowUVParam.w);
                flow_2 = tmpvar_4;
                float2 P_5;
                P_5 = (IN.texcoord.xy + (flow_2.xy * _FlowUVParam.z));
                fixed4 tmpvar_6;
                tmpvar_6 = tex2D(_MainTex, P_5);
                color_1.xyz = tmpvar_6.xyz;
                #endif

                #ifdef _SEPARATEALPHA_ON
                color_1.w = tex2D(_AlphaTex, IN.texcoord.xy).x;
                tmpvar_2.w = color_1.w;
                #endif
                
                #ifdef MASK_ON
                color_1.w = (tmpvar_2.w * tex2D(_MaskTex, IN.texcoord.zw).x);
                #endif
                
                color_1 = (color_1 * IN.color);
                
                color_1 = (color_1 * _Color);
                color_1.xyz = (color_1.xyz * _Brightness);
                return color_1;
            }
            ENDCG
        }
    }

    Fallback Off
}