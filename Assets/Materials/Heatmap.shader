Shader "Heatmap" 
{
	Properties
	{
		_HeatmapTex("Texture", 2D) = "white" {}
		_Transparency("Transparency", Range(0,1)) =0.5
	}

	SubShader
	{
		Tags{ "Queue" = "Transparent" }
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
			};
		
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
			};
			
			sampler2D _HeatmapTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				return o;
			}

			uniform int _PointsLength = 0;
			uniform float3 _Points[100];
			uniform float _Radius;

			half _Transparency;

			half GetFalloff(float3 worldPos)
			{
				half h = 0;

				for (int i = 0; i < _PointsLength; i++)
				{
					float3 worldPosition = worldPos;
					float3 pointPos = _Points[i].xyz;

					half di = distance(worldPos, pointPos) / _Radius;

					di = di * di;

					half hi = 1 - (di / _Radius);

					hi = saturate(hi);
					hi = hi * hi;

					h += hi;
				}

				h /= _PointsLength;

				return h;
			}

			half4 frag(v2f o) : SV_Target
			{
				half h = GetFalloff(o.worldPos);

				half4 heat = tex2D(_HeatmapTex, fixed2(h, 0.5));

				heat.a = _Transparency;

				return heat;
			}

			ENDCG
		}
	}

	Fallback "Diffuse"
}
