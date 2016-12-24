 Shader "Custom/GaussianBlur"
 {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
		//_Width ("Texture Width", Int) = 1024
		//_Height ("Texture Height", Int) = 512
		//_Step ("Sample Step", Range(0, 5)) = 1.8
    }
 
 	CGINCLUDE

 	#include "UnityCG.cginc"
    sampler2D _MainTex;
	float4 _MainTex_ST; 
	//half _Width;
	//half _Height;
	//half _Step;
	
	uniform float2 sampleDir;

	static const float sampleOffsets[8] = {0, 1, 2, 3, 4, 5, 0, 0};
	static const float sampleWeights[8] = {0.109378915439644, 0.107213067801671, 0.100969464792377, 0.0913609498232073, 0.0794253941226624, 0.0663416657402598, 0, 0};

	// 10, 1
	//static const float sampleOffsets[8] = {0, 1.49625015258789, 3.49125099182129, 5.48625326156616, 7.48125886917114, 9.47626781463623, 0, 0};
	//static const float sampleWeights[8] = {0.0564687612052823, 0.111537726900501, 0.106111230043284, 0.0970001809665196, 0.0852030922382308, 0.071913389248823, 0, 0};

	// 5, 2
	//static const float sampleOffsets[8] = {0, 1.48500454425812, 3.4650571346283, 5.44522094726563, 7.42555713653564, 9.40612697601318, 0, 0};
	//static const float sampleWeights[8] = {0.0827184653977396, 0.157439297597506, 0.129158204077168, 0.0904346850300042, 0.0540439769605456, 0.0275646036359067, 0, 0};

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
		float2 tex0 : TEXCOORD0;
		float4 tex1 : TEXCOORD1;
		float4 tex2 : TEXCOORD2;
		float4 tex3 : TEXCOORD3;
		float4 tex4 : TEXCOORD4;
		float4 tex5 : TEXCOORD5;
	};

	v2f vertBlur(appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.color = v.color;
		
		//sampleDir = float2(1/_Width, 1/_Height) * _Step;
		v.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex); 
		o.tex0 = v.texcoord + sampleDir * sampleOffsets[0];
		
		o.tex1.xy = v.texcoord + sampleDir * sampleOffsets[1];
		o.tex1.zw = v.texcoord - sampleDir * sampleOffsets[1];

		o.tex2.xy = v.texcoord + sampleDir * sampleOffsets[2];
		o.tex2.zw = v.texcoord - sampleDir * sampleOffsets[2];

		o.tex3.xy = v.texcoord + sampleDir * sampleOffsets[3];
		o.tex3.zw = v.texcoord - sampleDir * sampleOffsets[3];

		o.tex4.xy = v.texcoord + sampleDir * sampleOffsets[4];
		o.tex4.zw = v.texcoord - sampleDir * sampleOffsets[4];

		o.tex5.xy = v.texcoord + sampleDir * sampleOffsets[5];
		o.tex5.zw = v.texcoord - sampleDir * sampleOffsets[5];

		return o;
	}

	half4 fragBlur( v2f In) : COLOR
	{
		float4 color = 0;
		color += tex2D(_MainTex, In.tex0) * sampleWeights[0];

		color += tex2D(_MainTex, In.tex1.xy) * sampleWeights[1];
		color += tex2D(_MainTex, In.tex1.zw) * sampleWeights[1];

		color += tex2D(_MainTex, In.tex2.xy) * sampleWeights[2];
		color += tex2D(_MainTex, In.tex2.zw) * sampleWeights[2];

		color += tex2D(_MainTex, In.tex3.xy) * sampleWeights[3];
		color += tex2D(_MainTex, In.tex3.zw) * sampleWeights[3];

		color += tex2D(_MainTex, In.tex4.xy) * sampleWeights[4];
		color += tex2D(_MainTex, In.tex4.zw) * sampleWeights[4];

		color += tex2D(_MainTex, In.tex5.xy) * sampleWeights[5];
		color += tex2D(_MainTex, In.tex5.zw) * sampleWeights[5];

		color *= In.color;
		color.a = 1.0f;
		return color;
	}	
	ENDCG

    SubShader
    {
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
	 	LOD 100 ZTest Off Cull Off ZWrite Off Blend Off Fog { Mode off }  
		
		//down sample pass
		Pass
		{	
			SetTexture [_MainTex] {}
		}

		//blur pass
		Pass
		{ 
			CGPROGRAM
			
			#pragma vertex vertBlur
			#pragma fragment fragBlur
			#pragma fragmentoption ARB_precision_hint_fastest 
			
			ENDCG
		}	
      
     }
 
     //Fallback "VertexLit"
 }