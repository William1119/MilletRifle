Shader "Custom/Unlit" 
{
    Properties 
	{
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white"
	}

	Category 
	{
		SubShader 
		{
			Tags { "RenderType"="Opaque" }
			LOD 100
			Lighting Off
			ZWrite On
        	Cull back
			Pass {
				Tags { "LightMode" = "Vertex" }
				SetTexture [_MainTex] { combine texture } 
			}
		}
	}
}