Shader "WarFog"
{
	Properties
	{
		_MainTex ("ScrenTexture", 2D) = "white" {}
		_WarFogTex("RenderTexture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _WarFogTex;
			float4 _WarFogTex_TexelSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_WarFogTex, i.uv);
				col = 0.2 * col + 0.1 * (tex2D(_WarFogTex, i.uv + _WarFogTex_TexelSize.xy) + tex2D(_WarFogTex, i.uv - _WarFogTex_TexelSize.xy) + tex2D(_WarFogTex, i.uv + float2 (_WarFogTex_TexelSize.x, -_WarFogTex_TexelSize.y)) + tex2D(_WarFogTex, i.uv + float2 (-_WarFogTex_TexelSize.x, _WarFogTex_TexelSize.y)));
				_WarFogTex_TexelSize = _WarFogTex_TexelSize * 2;
				col = col + 0.1 * (tex2D(_WarFogTex, i.uv + _WarFogTex_TexelSize.xy) + tex2D(_WarFogTex, i.uv - _WarFogTex_TexelSize.xy) + tex2D(_WarFogTex, i.uv + float2 (_WarFogTex_TexelSize.x, -_WarFogTex_TexelSize.y)) + tex2D(_WarFogTex, i.uv + float2 (-_WarFogTex_TexelSize.x, _WarFogTex_TexelSize.y)));
				col.rgb = col.rgb * tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
