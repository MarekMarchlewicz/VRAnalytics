Shader "Unlit/MovieHeatmap"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_HeatmapTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags{ "RenderType" = "Transparent" }
		
		Blend SrcAlpha OneMinusSrcAlpha

		Cull front

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
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			sampler2D _HeatmapTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			uniform int _PointsLength = 0;
			uniform float3 _Points[100];
			uniform float _Radius;

			half GetFalloff(float3 worldPos)
			{
				half h = 0;

				for (int i = 0; i < _PointsLength; i++)
				{
					float3 worldPosition = worldPos;
					worldPosition = normalize(worldPosition);
					float3 pointPos = _Points[i].xyz;
					pointPos = normalize(pointPos);

					half di = acos(dot(worldPosition, pointPos));

					di = di * di;
					half hi = 1 - (di / _Radius);

					hi = saturate(hi);
					hi = hi * hi;

					h += hi;
				}

				h /= _PointsLength;

				return h;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half h = GetFalloff(i.worldPos);
				fixed4 movie = tex2D(_MainTex, fixed2(1 - i.uv.x, i.uv.y));
				fixed4 heat = tex2D(_HeatmapTex, fixed2(h, 0.5));
				return lerp(movie, heat, heat.a);
			}
			ENDCG
		}
	}
}
