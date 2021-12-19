Shader "Planet/Star/Flares"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
	    _Pixels("Pixels", range(10,300)) = 200.0
    	_GradientTex("Gradient", 2D) = "white" {}	    
	    _Speed("Speed", range(-1.0, 1.0)) = 0.2
	    _Rotation("Rotation", range(0.0, 6.28)) = 0.0
    	    	
    	_StormWidth("Storm Width", range(0.0, 0.5)) = 0.3
    	_StormDitherWidth("Storm Dither Width", range(0.0, 0.5)) = 0.07
    	_CircleAmount("Circle Amount", range(2, 30)) = 2
    	_CircleSize("Circle Size", range(0.0, 1.0)) = 1.0
	    
    	_Size("Size", float) = 50.0
	    _Scale("Scale", float) = 1.0
	    _Octaves("Octaves", range(0,20)) = 4
	    _Seed("Seed", range(1, 100)) = 1
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
            sampler2D _GradientTex;       
            float _Rotation;
			float _Speed;
            float _Timestamp;

            float _Scale;
            float _Size;
            int _Octaves;
            int _Seed;

            float _CircleAmount;
            float _CircleSize;
            float _StormWidth;
            float _StormDitherWidth;

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

			float2 rotate(float2 coord, float angle){
				coord -= 0.5;
				//coord *= mat2(float2(cos(angle),-sin(angle)),float2(sin(angle),cos(angle)));            	
            	coord = mul(coord,float2x2(float2(cos(angle),-sin(angle)),float2(sin(angle),cos(angle))));
				return coord + 0.5;
			}
			
			float circle(float2 uv) {
				float invert = 1.0 / _CircleAmount;
				
				if (mod(uv.y, invert*2.0) < invert) {
					uv.x += invert*0.5;
				}
				float2 rand_co = floor(uv*_CircleAmount)/_CircleAmount;
				uv = mod(uv, invert)*_CircleAmount;
				
				float r = rand(rand_co);
				r = clamp(r, invert, 1.0 - invert);
				float circle = distance(uv, float2(r,r));
				return smoothstep(circle, circle+0.5, invert * _CircleSize * rand(rand_co*1.5));
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
				float _Scale = 0.5;

				for(int i = 0; i < _Octaves ; i++){
					value += noise(coord) * _Scale;
					coord *= 2.0;
					_Scale *= 0.5;
				}
				return value;
			}

			bool dither(float2 uv1, float2 uv2) {
				return mod(uv1.x+uv2.y,2.0/_Pixels) <= 1.0 / _Pixels;
			}

			float2 spherify(float2 uv) {
				float2 centered= uv *2.0-1.0;
				float z = sqrt(1.0 - dot(centered.xy, centered.xy));
				float2 sphere = centered/(z + 1.0);
				return sphere * 0.5+0.5;
			}
			fixed4 frag(v2f i) : COLOR {
				float2 uv = i.uv;
                
				if(_Pixels > 0 ){
					uv = floor(i.uv*_Pixels)/_Pixels;				
				}

				// use dither val later to mix between colors
				bool dith = dither(i.uv, uv);
					
				uv = rotate(uv, _Rotation);
				
				// counter _Rotation against _Rotation caused by the way uv's are made later
				
				// angle from centered uv's
				float angle = atan2(uv.x - 0.5, uv.y - 0.5) * 0.4;
				// distance from center
				float d = distance(uv, float2(0.5,0.5));
				
				// we make uv circular here to have eternally outward moving stuff
				float2 circleUV = float2(d, angle);
				
				// two types of noise values
				float n = fbm(circleUV*_Size -_Timestamp * _Speed);
				float nc = circle(circleUV*_Scale -_Timestamp * _Speed + n);
				
				nc *= 1.5;
				float n2 = fbm(circleUV*_Size -_Timestamp + float2(100, 100));
				nc -= n2 * 0.1;
				
				// our alpha, default 0
				float a = 0.0;
				if (1.0 - d > nc) {
					// now we generate very thin strips of positive alpha if our noise has certain values and is close enough to center
					if (nc > _StormWidth - _StormDitherWidth + d && dith) {
						a = 1.0;
					} else if (nc > _StormWidth + d) { // could use an or statement instead, but this looks more clear to me
						a = 1.0;
					}
				}
				
				// use our two noise values to assign colors
				float interpolate = floor(n2 + nc);
				float3 c = tex2D(_GradientTex, float2(interpolate, 0.0)).rgb;	
				
				// final step to not have everything appear from the center
				a *= step(n2 * 0.25, d);
				return fixed4(c, a);
				}
            
            ENDCG
        }
    }
}
