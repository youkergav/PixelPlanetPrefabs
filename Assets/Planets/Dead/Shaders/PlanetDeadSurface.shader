Shader "Planet/Dead/Surface"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    	
	    _Pixels("Pixels", range(10,100)) = 0.0
	    _Rotation("Rotation", range(0.0, 6.28)) = 0.0
    	_LightOrigin("Light Origin", Vector) = (0.39,0.39,0.39,0.39)
	    _Speed("Speed", range(-1.0, 1.0)) = 0.2
	    _Dithering("Dithering", range(0.0, 10.0)) = 2.0
    	
	    _LightBorder1("Light Border 1", range(0.0, 1.0)) = 0.52
	    _LightBorder2("Light Border 2", range(0.0, 1.0)) = 0.62
    	    	
	    _Color("Color 1", Color) = (1,1,1,1)
    	_Color2("Color 2", Color) = (1,1,1,1)
    	_Color3("Color 3", Color) = (1,1,1,1)
    	
	    _Size("Size",float) = 50.0
	    _Octaves("Octaves", range(0,20)) = 1.5
	    _Seed("Seed",range(1, 100)) = 1
	    _Timestamp("Timestamp",float) = 0.0
    	
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
            float _Dithering;
			float2 _LightOrigin;    	
			float _Speed;
            float _LightBorder1;
			float _LightBorder2;
            float _Size;
            int _Octaves;
            int _Seed;
			float _Timestamp;
    		fixed4 _Color;
            fixed4 _Color2;
            fixed4 _Color3;
            
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

			bool dither(float2 uv1, float2 uv2) {
				return mod(uv1.x+uv2.y,2.0/_Pixels) <= 1.0 / _Pixels;
			}

			float2 rotate(float2 coord, float angle){
				coord -= 0.5;
				//coord *= mat2(float2(cos(angle),-sin(angle)),float2(sin(angle),cos(angle)));            	
            	coord = mul(coord,float2x2(float2(cos(angle),-sin(angle)),float2(sin(angle),cos(angle))));
				return coord + 0.5;
			}

			fixed4 frag(v2f i) : COLOR {
				float2 uv = i.uv;
                
				if(_Pixels > 0 ){
					uv = floor(i.uv*_Pixels)/_Pixels;				
				}
	
				// check distance from center & distance to light
				float d_circle = distance(uv, float2(0.5,0.5));
				float d_light = distance(uv , float2(_LightOrigin));
				// cut out a circle
				float a = step(d_circle, 0.5);
				
				bool dith = dither(uv,uv);
				uv = rotate(uv, _Rotation);

				// get a noise value with light distance added
				// this creates a moving dynamic shape
				float fbm1 = fbm(uv);
				d_light += fbm(uv*_Size+fbm1+float2(_Timestamp*_Speed, 0.0))*0.3; // change the magic 0.3 here for different light strengths
				
				// _Size of edge in which colors should be dithered
				float dither_border = (1.0/_Pixels)*_Dithering;

				// now we can assign colors based on distance to light origin
				float3 col = _Color.rgb;
				if (d_light > _LightBorder1) {
					col = _Color2.rgb;
					if (d_light < _LightBorder1 + dither_border && dith) {
						col = _Color.rgb;
					}
				}
				if (d_light > _LightBorder2) {
					col = _Color3.rgb;
					if (d_light < _LightBorder2 + dither_border && dith) {
						col = _Color2.rgb;
					}
				}
				
				return fixed4(col, a);
				}
            
            ENDCG
        }
    }
}
