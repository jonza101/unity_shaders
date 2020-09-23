Shader "Unlit/water"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_normal_map("Normal map", 2D) = "white" {}
		_dudv_map("DuDv map", 2D) = "white" {}
		_refl_map("refl_map", 2D) = "" {}

		_ambient("Ambient", Color) = (1, 1, 1, 1)
		_diffuse("Diffuse", Color) = (1, 1, 1, 1)
		_specular("Specular", Color) = (1, 1, 1, 1)
		_shininess("Shininess", float) = 1.0

		_transparency("Transparency", Range(0.0, 1.0)) = 1.0
		_distor_s("Distorion strength", float) = 0.02
		_max_depth("Max depth", float) = 1.0
		_max_dist("Max distance", float) = 1.0
		_refl("Reflection factor", Range(0.0, 1.0)) = 1.0

		_wave_len("Wave length", float) = 1.0
		_period("Period(in seconds)", float) = 1.0
		_wave_amplitude("Wave amplitude", float) = 1.0

		_min_wave_ripple("Min wave ripple", Range(-1.0, 0.0)) = 0.0
		_max_wave_ripple("Max wave ripple", Range(0.0, 1.0)) = 0.0
		_wave_ripple_speed("Wave ripple speed", float) = 1.0
		_wave_ripple_amplitude("Wave ripple amplitude", float) = 1.0

		_txt_move("Texture move factor", float) = 0.0
		_txt_move_dir("Texture move direction (UV)", Vector) = (1.0, 1.0, 0.0, 0.0)

		_distor_move("Distorion move factor", float) = 0.02
		_distor_move_dir("Distorion move direction (UV)", Vector) = (1.0, 1.0, 0.0, 0.0)

		_normal_move("Normal move factor", float) = 0.0
		_normal_move_dir("Normal move direction (UV)", Vector) = (1.0, 1.0, 0.0, 0.0)
		_normal_offset("Normal offset", Vector) = (1.0, 1.0, 1.0)
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		GrabPass {"_frame_data"}


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"


			sampler2D	_MainTex;
			sampler2D	_normal_map;
			sampler2D	_dudv_map;
			sampler2D	_refl_map;

			sampler2D	_CameraDepthTexture, _frame_data;
			float4		_MainTex_ST;

            struct vert_in
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
            };

            struct vert_out
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float3 world_v : TEXCOORD2;
				float4 normal : NORMAL;
				float4 screen_pos : TEXCOORD1;
            };

            
			float4		_ambient;
			float4		_diffuse;
			float4		_specular;
			float		_shininess;


			float		_transparency;
			float		_distor_s;
			float		_max_depth;
			float		_max_dist;
			float		_refl;

			float		_wave_len;
			float		_period;
			float		_wave_amplitude;

			float		_min_wave_ripple;
			float		_max_wave_ripple;
			float		_wave_ripple_speed;
			float		_wave_ripple_amplitude;

			float		_txt_move;
			float2		_txt_move_dir;

			float		_distor_move;
			float2		_distor_move_dir;

			float		_normal_move;
			float2		_normal_move_dir;
			float4		_normal_offset;


			static const float PI = 3.14159265f;


			float	Unity_RandomRange_float(float2 seed, float min, float max)
			{
				float randomno = frac(sin(dot(seed, float2(12.9898, 78.233))) * 43758.5453);
				return (lerp(min, max, randomno));
			}

			vert_out	vert(vert_in v)
            {
				vert_out o;

				float noise = Unity_RandomRange_float(v.vertex.xz, _min_wave_ripple, _max_wave_ripple);

				float dx = cos(_Time * _wave_ripple_speed * noise) * _wave_ripple_amplitude;
				float dy = sin(_Time * _wave_ripple_speed * noise) * _wave_ripple_amplitude;

				float k = 2.0 * PI / _wave_len;
				float w = 2.0 * PI / _period;
				float phase = k * v.vertex.x - w * _Time;

				float3 old = float3(v.vertex.xyz);

				v.vertex.x += cos(phase) * _wave_amplitude;
				v.vertex.y += sin(phase) * _wave_amplitude + dy;

				float3 n = (normalize(v.vertex - old + _normal_offset));
				n.y = abs(n.y);
				o.normal = float4(n.xyz, 1.0);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.world_v = o.vertex;
				
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screen_pos = ComputeScreenPos(o.vertex);

                return o;
            }

			fixed4		frag(vert_out v) : SV_Target
			{
				//	PROPERTIES CALC
				float2 uv = v.screen_pos.xy / v.screen_pos.w;
				float2 d_uv = (v.uv + _Time * _distor_move * _distor_move_dir) % 1.0;
				float2 distortion = (tex2D(_dudv_map, d_uv).xy * 2.0 - 1.0) * _distor_s;

				float2 b_uv = (v.screen_pos.xy + distortion) / v.screen_pos.w;
				fixed4 b_color = tex2D(_frame_data, b_uv);

				float2 r_uv = (uv.xy + distortion);
				fixed4 r_color = tex2D(_refl_map, b_uv);

				//	DEPTH CALC
				float b_led = LinearEyeDepth(tex2D(_CameraDepthTexture, b_uv));
				float led = LinearEyeDepth(tex2D(_CameraDepthTexture, uv));
				float background_depth = b_led;
				if (led > b_led)
				{
					background_depth = led;
					b_color = tex2D(_frame_data, uv);
				}

				float surface_depth = UNITY_Z_0_FAR_FROM_CLIPSPACE(v.screen_pos.z);
				float depth = background_depth - surface_depth;
				float depth_factor = (min(_max_depth, depth) / _max_depth);

				float dist_factor = min(_max_dist, distance(v.world_v, _WorldSpaceCameraPos)) / _max_dist;
				depth_factor = saturate(depth_factor + (depth_factor * dist_factor));

				//	NORMAL MAPPING
				float2 n_uv = (v.uv + _Time * _normal_move * _normal_move_dir) % 1.0;
				float3 normal_map_color = tex2D(_normal_map, n_uv);
				float3 normal = float3(normal_map_color.x * 2.0 - 1.0, -normal_map_color.z, normal_map_color.y * 2.0 - 1.0);
				normal = normalize(normal);

				float3 t = cross(v.normal, float3(0.0, 1.0, 0.0));
				float3 b;
				if (length(t) == 0.0)
					t = cross(v.normal, float3(0.0, 0.0, 1.0));
				t = normalize(t);
				b = normalize(cross(v.normal, t));
				float3x3 tbn = float3x3(t, b, float3(v.normal.xyz));
				v.normal = float4(float3(normalize(mul(tbn, normal))).xyz, 1.0);

				//	SHADING CALC
				float4 light_dir = _WorldSpaceLightPos0;
				float3 view_dir = normalize(_WorldSpaceCameraPos - v.world_v);

				float diff = max(0.0, dot(v.normal, light_dir));
				float3 halfway_dir = normalize(light_dir + view_dir);
				float spec = pow(max(0.0, dot(halfway_dir, v.normal)), _shininess);

				float2 t_uv = (v.uv + _Time * _txt_move * _txt_move_dir) % 1.0;

				float3 ambient = _ambient;
				float3 diffuse = _diffuse * tex2D(_MainTex, t_uv) * diff;
				float3 specular = _specular * spec;

				float3 l_color = (ambient + diffuse + specular) * _LightColor0;

				//	FRESNEL
				float fresnel =	1.0 - max(0.0, dot(view_dir, v.normal));
				//return float4(fresnel, fresnel, fresnel, 1.0);

				float4 color = float4(lerp(b_color, l_color, depth_factor).xyz, 1.0);
				color = float4(lerp(color, r_color, fresnel * _refl).xyz, _transparency * 1);

				return color;
            }
            ENDCG
        }
    }
}
