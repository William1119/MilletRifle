Shader "Custom/Unlit Outline" 
{
    Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005
	}

	Category 
	{
		SubShader 
		{
        	UsePass "Character/NGCharShader_All/OUTLINE"
			Pass {
				Tags { "RenderType"="Opaque" "LightMode" = "Vertex" }
				Lighting Off
        		Cull back
				SetTexture [_MainTex] { combine texture } 
			}
			
		}
	}
}