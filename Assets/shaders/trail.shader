Shader "Unlit/trail"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

		_ambient("Ambient", Color) = (1, 1, 1, 1)
		_diffuse("Diffuse", Color) = (1, 1, 1, 1)
		_specular("Specular", Color) = (1, 1, 1, 1)
		_shininess("Shininess", float) = 1.0

		_old_pos("old_pos", Vector) = (1, 1, 1, 1)
		_curr_pos("curr_pos", Vector) = (1, 1, 1, 1)
		_plane("plane", Vector) = (1, 1, 1, 1)
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

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

				float4 w_vertex : POSITION1;
				float4 w_default_vertex : POSITION2;

				float4 world_v : POSITION3;
				float4 normal : TEXCOORD1;
            };

            sampler2D	_MainTex;
            float4		_MainTex_ST;

			float4		_ambient;
			float4		_diffuse;
			float4		_specular;
			float		_shininess;

			float4		_old_pos;
			float4		_curr_pos;
			float4		_plane;

			vert_out	vert(vert_in v)
            {
				vert_out o;

				float4 world_v = mul(unity_ObjectToWorld, v.vertex);
				float dist = dot(world_v, _plane.xyz) + _plane.w;

				float4 new_w_v = world_v;
				if (dist < 0.0)
					new_w_v.xyz += (-_plane.xyz * distance(_old_pos.xyz, _curr_pos.xyz)).xyz;

				o.w_vertex = new_w_v;
				o.w_default_vertex = world_v;

				//o.default_vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = UnityObjectToClipPos(mul(unity_WorldToObject, new_w_v));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.world_v = world_v;
				o.normal = v.normal;

                return o;
            }

			fixed4		frag(vert_out v) : SV_Target
			{
				//	SHADING CALC
				float4 light_dir = _WorldSpaceLightPos0;
				float3 view_dir = normalize(_WorldSpaceCameraPos - v.world_v);

				float diff = max(0.0, dot(v.normal, light_dir));
				float3 halfway_dir = normalize(light_dir + view_dir);
				float spec = pow(max(0.0, dot(halfway_dir, v.normal)), _shininess);

				float3 ambient = _ambient;
				float3 diffuse = _diffuse * tex2D(_MainTex, v.uv) * diff;
				float3 specular = _specular * spec;

				float3 l_color = (ambient + diffuse + specular) * _LightColor0;
				float4 col = float4(l_color.xyz, 1.0);

				/*float t_factor = 1.0 - distance(_curr_pos.xyz, v.w_default_vertex.xyz) / distance(_old_pos.xyz, v.w_vertex.xyz);
				float3 cam_dir = mul((float3x3)unity_CameraToWorld, float3(0.0, 0.0, 1.0));
				float t_dot = abs(dot(_plane.xyz, cam_dir));*/

				//col.a = saturate(t_factor + t_dot);

                return col;
            }
            ENDCG
        }
    }
}
