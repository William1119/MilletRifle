Shader "Character/NGCharShader_Outline" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DiffuseColor ("Diffuse Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_Specular ("Specular", float) = 10
		_Exposure ("Global Exposure", Range(-4.0, 8.0)) = 0.0	
		_LightProbeIntensity ("Light Probe Intensity", float) = 1.0

		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_Outline ("Outline width", Range (.002, 0.01)) = .005

		[Toggle] _SelfIllumine("Self Illumination On/Off", float) = 0
		_IlluminColor ("Self Illumination Color(RGB)", Color) = (1,1,1,1)
        _IlluminPower ("Illumination Power", Range(0,4)) = 1
        _IlluminTex ("Illumination Channel (A)", 2D) = "black" {}
		_IlluminSpeed ("Illumination flash speed", Range(10, 0.01)) = 1
	}
	SubShader 
	{
		UsePass "Character/NGCharShader_All/BASE"
		UsePass "Character/NGCharShader_All/OUTLINE"
		UsePass "Character/NGCharShader_All/SHADOW"
	} 
	FallBack "Diffuse"
}
