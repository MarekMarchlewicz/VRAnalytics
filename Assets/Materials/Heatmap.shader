Shader "Heatmap" 
{
	Properties
	{
		_MovieTex("Texture", 2D) = "white" {}
		_HeatmapTex("Texture", 2D) = "white" {}
		_Transparency("Transparency", Range(0,1)) =0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha

		Cull Front

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
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};
			
			sampler2D _MovieTex;
			float4 _MovieTex_ST;
			sampler2D _HeatmapTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.uv = TRANSFORM_TEX(v.uv, _MovieTex);
				return o;
			}

			uniform int _PointsLength = 0;
			uniform float3 _Points[100];
			uniform float _Radius;

			half _Transparency;

			half4 frag(v2f o) : SV_Target
			{
				half h = 0;

				for (int i = 0; i < _PointsLength; i++)
				{
					float3 worldPosition = o.worldPos;
					worldPosition = normalize(worldPosition);
					float3 pointPos = _Points[i].xyz;
					pointPos = normalize(pointPos);

					half di = acos(dot(worldPosition, pointPos)) / _Radius;
					// distance(output.worldPos, _Points[i].xyz);

					di = di * di;
					half hi = 1 - (di / _Radius);

					hi = saturate(hi);
					hi = hi * hi;

					h += hi;
				}

				h /= _PointsLength;

				half4 movie = tex2D(_MovieTex, fixed2(1 - o.uv.x, o.uv.y));
				half4 heat = tex2D(_HeatmapTex, fixed2(h, 0.5));

				half4 final = lerp(movie, heat, heat.a);
				return final;
			}

			ENDCG
		}
	}

	Fallback "Diffuse"
}
