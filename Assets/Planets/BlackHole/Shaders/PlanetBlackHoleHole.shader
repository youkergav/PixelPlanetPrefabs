Shader "Planet/BlackHole/Hole"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Pixels("Pixels", range(10,100)) = 100.0
		_GradientTex("Gradient", 2D) = "white" {}
	    _Color("Color", Color) = (59,32,39,1)

		_Radius("Radius", range(0, 1)) = 0.5
	    _LightWidth("Light Width", range(0, 0.5)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "Queue"="Transparent"}
        LOD 100

        Pass
        {
			CULL Off
			ZWrite Off
         	Blend SrcAlpha OneMinusSrcAlpha
        	
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Pixels;
			sampler2D _GradientTex;
			fixed4 _Color;
			float _Radius;
			float _LightWidth;
            
			struct Input
	        {
	            float2 uv_MainTex;
	        };

            v2f vert (appdata v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			fixed4 frag(v2f i) : COLOR {
				float2 uv = floor(i.uv * _Pixels) / _Pixels; // Pixelize the UV.
				float centerDistance = distance(uv, float2(0.5,0.5)); // Cut out a circle

                // Create the outline color.
				float3 color = _Color.rgb;
				if(centerDistance > _Radius - _LightWidth) {
					float col_val = ceil(centerDistance - (_Radius - (_LightWidth * 0.5))) * (1.0 / (_LightWidth * 0.5));
					color = tex2D(_GradientTex, float2(col_val, 0.0)).rgb;
				}

            	return fixed4(color, step(centerDistance, _Radius));
			}
            
            ENDCG
        }
    }
}
