Shader "Custom/shader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Amplitude("Extrusion Amplitude", float) = 0.1
		_noise_offset("Noise offset", float) = 0.1
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM

		#pragma surface surf Standard fullforwardshadows vertex:vert

		#pragma target 3.0

		struct Input
	{
		float4 color : COLOR;
	};

	fixed4 _Color;
	float _Amplitude;
	float _noise_offset;

	float	Unity_RandomRange_float(float2 Seed, float Min, float Max)
	{
		float randomno =  frac(sin(dot(Seed, float2(12.9898, 78.233))) * 43758.5453);
		return (lerp(Min, Max, randomno));
	}

	void	vert(inout appdata_full v)
	{
		v.vertex.xyz += v.normal * _Amplitude * (1.0 - _SinTime.z);
		v.vertex.xyz += v.normal * Unity_RandomRange_float(v.vertex.xz, -1.0, 1.0) * _noise_offset;
	}
	void	surf(Input IN, inout SurfaceOutputStandard o)
	{
		fixed4 c = _Color;
		o.Albedo = c.rgb;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
