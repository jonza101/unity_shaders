Shader "Unlit/hologram"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_tint_color ("Tint color", Color) = (1, 1, 1, 1)
		_transparency ("Transparency", Range(0.0, 1.0)) = 1.0
		_cutout_thresh ("Cutout threshold", Range(0.0, 1.0)) = 1.0
		_distance ("Distance", float) = 1.0
		_amplitude ("Amplitude", float) = 1.0
		_speed ("Speed", float) = 1.0
		_amount ("Amount", Range(0.0, 1.0)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D	_MainTex;
            float4		_MainTex_ST;
			float4		_tint_color;
			float		_transparency;
			float		_cutout_thresh;

			float		_distance;
			float		_amplitude;
			float		_speed;
			float		_amount;

            v2f vert (appdata v)
            {
                v2f o;

				v.vertex.x += sin(_Time.y * _speed + v.vertex.y * _amplitude) * _distance * _amount;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _tint_color;
				col.a = _transparency;
				clip(col.r - _cutout_thresh);

                return col;
            }
            ENDCG
        }
    }
}
