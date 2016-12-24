Shader "Character/NGCharShader_Base" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DiffuseColor ("Diffuse Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Specular ("Specular", float) = 16
		_Exposure ("Global Exposure", float) = 1.0	
		_LightProbeIntensity ("Light Probe Intensity", float) = 4.0
		[Toggle] _SelfIllumine("Self Illumination On/Off", float) = 0
		_IlluminColor ("Self Illumination Color(RGB)", Color) = (1,1,1,1)
        _IlluminPower ("Illumination Power", Range(0,4)) = 1
        _IlluminTex ("Illumination Channel (A)", 2D) = "black" {}
		_IlluminSpeed ("Illumination flash speed", Range(10, 0.01)) = 1
	}
	SubShader 
	{
		UsePass "Character/NGCharShader_All/BASE"
		UsePass "Character/NGCharShader_All/SHADOW"
	} 
	FallBack "Diffuse"
}
