Shader "TestShader"
{
	Properties
	{
		_MainTexture("Texture", 2D) = "white" {}
		//changable color. parameters are (R,G,B,Alpha)
		_Color("Color", Color) = (1,1,1,1)
	}

	Subshader
	{
		//part of shader that gets run, so shadows or way it looks
		Pass
		{
			//all cg code, pretty different
			CGPROGRAM
			
			/*different kinds of functions in shaders.
			there's vertex functions: they change a mesh on your object, finds where it is on screen,
			then you tell it to change the pixel on your screen to whatever you want.
			then there's fragment shader: based on where the pixels determined above are, it changes each pixel to a different
			color. This is the basis of everything, you change the color of each pixel with code.*/
			#pragma vertex vertexFunc
			#pragma fragment fragmentFunc

			//code to import a unity manager that is handled in the GPU
			#include "UnityCG.cginc"

			struct appdata
			{
				//4 floats to hold the 4 positions that make the vertex
				float4 vertex : POSITION;
				//hold the coordinates to tell this how to put the texture on our object
				float2 uv : TEXCOORD0;
			};
			
			//vertex to fragment. puts the above together
			struct v2f
			{
				float4 position : SV_POSITION;
				float2 uv : TEXTCOORD0;
			};

			//take in the color we define above
			fixed4 _Color;
			sampler2D _MainTexture;

			v2f vertexFunc(appdata IN)
			{
				v2f OUT;

				//takes the object position and shows it in comparison to the camera. 
				OUT.position = UnityObjectToClipPos(IN.vertex);
				OUT.uv = IN.uv;

				return OUT;
			}

			//fixed4 is RGB, ALPHA
			fixed4 fragmentFunc(v2f IN) : SV_Target
			{
				fixed4 pixelColor = tex2D(_MainTexture, IN.uv);
				
				//modifies texture with color
				return pixelColor * _Color;
			}

			ENDCG
		}
	}
}