Shader "Character/NGCharShader_RimLight" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DiffuseColor ("Diffuse Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Specular ("Specular", float) = 10
		_Exposure ("Global Exposure", Range(-4.0, 8.0)) = 0.0	
		_LightProbeIntensity ("Light Probe Intensity", float) = 1.0
		
		_RimColor ("Rim Light Color", Color) = (1,1,1,1.0)
    	_RimPower ("Rim Light Range", Range(0.5, 10.0)) = 4.0
    	_RimBrightness ("Rim Light Brightness", Range(0.1, 10.0)) = 2.0
	}
	SubShader 
	{
		UsePass "Character/NGCharShader_All/RIMLIGHT"
		UsePass "Character/NGCharShader_All/SHADOW"
	} 
	FallBack "Diffuse"
}
