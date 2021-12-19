Shader "Planet/Icelands/Clouds"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Pixels("Pixels", range(10,100)) = 0.0
	    _Rotation("Rotation", range(0.0, 6.28)) = 0.0
	    _CloudCover("Cloud Cover", range(0.0, 1.0)) = 0.0
    	_LightOrigin("Light Origin", Vector) = (0.39,0.39,0.39,0.39)
	    _Speed("Speed", range(-1.0, 1.0)) = 0.2
	    _Stretch("Stretch", range(1.0,3.0)) = 2.0
	    _CloudCurve("Cloud Curve", range(1.0, 2.0)) = 1.3
	    _LightBorder1("Light Border 1", range(0.0, 1.0)) = 0.52
	    _LightBorder2("Light Border 2", range(0.0, 1.0)) = 0.62

	    _Color("Color 1", Color) = (1,1,1,1)
    	_Color3("Color 2", Color) = (0,0,0,0)
	    _Color2("Color 3", Color) = (0,0,0,0)
	    _Color4("Color 4", Color) = (0,0,0,0)

	    _Size("Size", float) = 50.0
	    _Octaves("Octaves", range(0,20)) = 5
	    _Seed("Seed", range(1, 10)) = 1
	    _Timestamp("Timestamp", float) = 0.0
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True" "Queue"="Transparent"}
        LOD 100

        Pass
        {
			CULL Off
			ZWrite Off // don't write to depth buffer 
         	Blend SrcAlpha OneMinusSrcAlpha // use alpha blending


        	
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Pixels;
            float _Rotation;
            float _CloudCover;
			float2 _LightOrigin;    	
			float _Speed;
            float _Stretch;
            float _CloudCurve;
            float _LightBorder1;
			float _LightBorder2;
			fixed4 _Color;
            fixed4 _Color3;
			fixed4 _Color2;
			fixed4 _Color4;
			float _Size;
            int _Octaves;
            int _Seed;
			float _Timestamp;
            
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

			float mod(float x, float y) {
				return x - y * floor(x / y);
			}

			float2 mod(float2 x, float2 y) {
				return x - y * floor(x / y);
			}

			float3 mod(float3 x, float3 y) {
				return x - y * floor(x / y);
			}

			float4 mod(float4 x, float4 y) {
				return x - y * floor(x / y);
			}

			float rand(float2 coord) {
				coord = mod(coord, float2(1.0,1.0)*round(_Size));
				return frac(sin(dot(coord.xy ,float2(12.9898,78.233))) * 15.5453 * _Seed);
			}

			float noise(float2 coord){
				float2 i = floor(coord);
				float2 f = frac(coord);
				
				float a = rand(i);
				float b = rand(i + float2(1.0, 0.0));
				float c = rand(i + float2(0.0, 1.0));
				float d = rand(i + float2(1.0, 1.0));

				float2 cubic = f * f * (3.0 - 2.0 * f);

				return lerp(a, b, cubic.x) + (c - a) * cubic.y * (1.0 - cubic.x) + (d - b) * cubic.x * cubic.y;
			}

			float fbm(float2 coord){
				float value = 0.0;
				float scale = 0.5;

				for(int i = 0; i < _Octaves ; i++){
					value += noise(coord) * scale;
					coord *= 2.0;
					scale *= 0.5;
				}
				return value;
			}


			// by Leukbaars from https://www.shadertoy.com/view/4tK3zR
			float circleNoise(float2 uv) {
			    float uv_y = floor(uv.y);
			    uv.x += uv_y*.31;
			    float2 f = frac(uv);
				float h = rand(float2(floor(uv.x),floor(uv_y)));
			    float m = (length(f-0.25-(h*0.5)));
			    float r = h*0.25;
			    return smoothstep(0.0, r, m*0.75);
			}

			float cloud_alpha(float2 uv) {
				float c_noise = 0.0;
				
				// more iterations for more turbulence
				for (int i = 0; i < 9; i++) {
					c_noise += circleNoise((uv * _Size * 0.3) + (float(i+1)+10.) + (float2(_Timestamp*_Speed, 0.0)));
				}
				float fbmval = fbm(uv*_Size+c_noise + float2(_Timestamp*_Speed, 0.0));
				
				return fbmval;//step(a_cutoff, fbm);
			}

			bool dither(float2 uv_pixel, float2 uv_real) {
				return mod(uv_pixel.x+uv_real.y,2.0/_Pixels) <= 1.0 / _Pixels;
			}

			float2 spherify(float2 uv) {
				float2 centered= uv *2.0-1.0;
				float z = sqrt(1.0 - dot(centered.xy, centered.xy));
				float2 sphere = centered/(z + 1.0);
				return sphere * 0.5+0.5;
			}

			float2 rotate(float2 coord, float angle){
				coord -= 0.5;
				//coord *= float2x2(float2(cos(angle),-sin(angle)),float2(sin(angle),cos(angle)));
            	coord = mul(coord,float2x2(float2(cos(angle),-sin(angle)),float2(sin(angle),cos(angle))));
				return coord + 0.5;
			}

			fixed4 frag(v2f i) : COLOR {
				float2 uv = i.uv;
                
				if(_Pixels > 0 ){
					uv = floor(i.uv*_Pixels)/_Pixels;				
				}
				
				// distance to light source
				float d_light = distance(uv , _LightOrigin);
				
				// cut out a circle
				float d_circle = distance(uv, float2(0.5,0.5));
				float a = step(d_circle, 0.5);
				
				float d_to_center = distance(uv, float2(0.5,0.5));
				
				uv = rotate(uv, _Rotation);
				
				// map to sphere
				uv = spherify(uv);
				// slightly make uv go down on the right, and up in the left
				uv.y += smoothstep(0.0, _CloudCurve, abs(uv.x-0.4));
				
				
				float c = cloud_alpha(uv*float2(1.0, _Stretch));
				
				// assign some colors based on cloud depth & distance from light
				float3 col = _Color.rgb;
				if (c < _CloudCover + 0.03) {
					col = _Color3.rgb;
				}
				if (d_light + c*0.2 > _LightBorder1) {
					col = _Color2.rgb;

				}
				if (d_light + c*0.2 > _LightBorder2) {
					col = _Color4.rgb;
				}
				
				c *= step(d_to_center, 0.5);
				//COLOR = float4(col, step(_CloudCover, c) * a);
            	return fixed4(col, step(_CloudCover, c) * a);
			}
            
            ENDCG
        }
    }
}
