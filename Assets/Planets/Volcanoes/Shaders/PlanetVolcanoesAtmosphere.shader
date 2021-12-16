Shader "Planet/Volcanoes/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_InnerRadius("Inner Radius", range(0, .5)) = .4
		_Power("Power", range(0, 10)) = 2
		_LightOrigin("Light Origin", Vector) = (0.39, 0.39, 0, 0)
		_LightZone("Light Zone", range(0, 4.084067)) = 0
		_Falloff("Falloff", range(0, 10)) = 2
		_Pixels("Pixels", range(10,100)) = 0.0
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
			fixed4 _Color;
			float _Pixels;
			float _InnerRadius;
			float2 _LightOrigin;
			float _LightZone;
			float _Falloff;
			float _Power;
            
			struct Input
	        {
	            float2 uv_MainTex;
	        };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float2 pixelize(float2 uv) {
				return floor(uv * _Pixels) / _Pixels;
			}

			float getRadians(float2 coord) {
				float deltaX = coord.x - .5;
				float deltaY = coord.y - .5;

				float radius = distance(coord, float2(.5, .5));
				float radians = acos(deltaX / radius);

				if(deltaY < 0) {
					return (3.141592 * 2) - radians;
				}

				return radians;
			}

			fixed4 frag(v2f i) : COLOR {
				const float PI = 3.141592;

				float2 uv = pixelize(i.uv);
				float alpha = distance(uv, float2(.5, .5));

				// Cut out a circle.
				if(step(alpha, .5) == 0) {
					return fixed4(_Color.rgb, 0);
				}

				// Remove the inner circle.
				if(alpha < _InnerRadius) {
					return fixed4(_Color.rgb, 0);
				}

				// Fade between inner circle and outer circle.
				alpha = ((alpha - _InnerRadius) / (.5 - _InnerRadius));
				alpha = pow(alpha, _Power);

				// Handle default light position.
				if(_LightOrigin.x == .5 && _LightOrigin.y == .5) {
					return fixed4(_Color.rgb, alpha);
				}

				// Set light direction.
				float transitionZone = (PI / 2);
				float darkZone = PI * 2 - (_LightZone + transitionZone);
				float lightRadians = getRadians(_LightOrigin);
				float uvRadians = getRadians(i.uv);
				float arcDistance = distance(lightRadians, uvRadians);

				if(arcDistance > PI) {
					arcDistance = (2 * PI) - arcDistance;
				}

				if(arcDistance < _LightZone / 2) {
					return fixed4(_Color.rgb, alpha);
				}

				if(arcDistance > (_LightZone + transitionZone) / 2) {
					return fixed4(_Color.rgb, 0);
				}

				// Fade the outside edges.
				float fade = (((_LightZone + transitionZone) / 2) - arcDistance) / (transitionZone / 2);
				fade = pow(fade, _Falloff);

				return fixed4(_Color.rgb, alpha * fade);
			}

            ENDCG
        }
    }
}