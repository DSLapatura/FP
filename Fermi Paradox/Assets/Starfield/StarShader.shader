Shader "Unlit/StarShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Brightness("Brightness", Range(0,50)) = 1
		_AnotherBrightness("Brightness2",Float) = 1
	}
	SubShader
	{
		Tags{ "Quene" = "Transparent" "RenderType" = "Transparent" }
		LOD 100
		Blend One One
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float atten : PSIZE0;
			};

			sampler2D _MainTex;
			float4 _Color;
			float4 _MainTex_ST;
			float _Brightness;
			float _AnotherBrightness;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float distance = length(WorldSpaceViewDir(v.vertex));
				o.atten = 1 / distance / distance;
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = max(float4 (0,0,0,0), tex2D(_MainTex, i.uv) * _Color * i.atten * _Brightness * _AnotherBrightness);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
