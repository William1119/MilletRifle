Shader "Custom/Unlit/UnlitTint" 
{
    Properties 
	{
		_MultColor ("Color Tint Mult", Color) = (1,1,1,1)	
		_AddColor("Color Tint Add", Color) = (0,0,0,0)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white"
	}

	Category 
	{
		Lighting Off
		ZWrite On
        Cull back
		Blend SrcAlpha OneMinusSrcAlpha
		Tags {Queue=Transparent}

		SubShader 
		{

			Pass 
			{
				SetTexture [_MainTex] 
				{
					ConstantColor [_MultColor]
					Combine Texture * constant
				}
				SetTexture [_MainTex] 
				{
					ConstantColor [_AddColor]
					Combine previous + constant 
				}
			}
		}
	}
}