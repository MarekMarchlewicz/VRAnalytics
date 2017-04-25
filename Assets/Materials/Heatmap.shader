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

		Cull Front

		Pass
		{
			CGPROGRAM
			#pragma vertex vert             
			#pragma fragment frag
			
			struct vertInput 
			{
				float4 pos : POSITION;
			};
		
			struct vertOutput 
			{
				float4 pos : POSITION;
				fixed3 worldPos : TEXCOORD1;
			};

			vertOutput vert(vertInput input) 
			{
				vertOutput o;
				o.pos = UnityObjectToClipPos(input.pos);
				o.worldPos = mul(unity_ObjectToWorld, input.pos).xyz;
				return o;
			}

			uniform int _PointsLength = 0;
			uniform float3 _Points[100];
			uniform float _Radius;

			half _Transparency;

			sampler2D _HeatmapTex;

			half4 frag(vertOutput output) : COLOR
			{
				half h = 0;

				for (int i = 0; i < _PointsLength; i++)
				{
					half di = acos(dot(normalize(output.worldPos), normalize(_Points[i].xyz))) / _Radius;
					// distance(output.worldPos, _Points[i].xyz);

					di = di * di;
					half hi = 1 - (di / _Radius);

					hi = saturate(hi);
					hi = hi * hi;

					h += hi;
				}

				h /= _PointsLength;

				half4 color = tex2D(_HeatmapTex, fixed2(h, 0.5));
				color.a = _Transparency;
				return color;
			}

			ENDCG
		}
	}

	Fallback "Diffuse"
}
