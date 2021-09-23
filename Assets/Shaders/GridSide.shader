

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Voxels/GridSide" {
	Properties{
		_GridThickness("Grid Thickness", Float) = 0.1
		_GridSpacing("Grid Spacing", Float) = 0.1
		_GridColour("Grid Colour", Color) = (1.0, 1.0, 1.0, 1.0)
		_BaseColour("Base Colour", Color) = (0.0, 0.0, 0.0, 0.0)
	}

		SubShader{
			Tags{ "Queue" = "Transparent" }

			Pass{
				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM

		// Define the vertex and fragment shader functions
		#pragma vertex vert
		#pragma fragment frag

		// Access Shaderlab properties
		uniform float _GridThickness;
		uniform float _GridSpacing;
		uniform float4 _GridColour;
		uniform float4 _BaseColour;

		// Input into the vertex shader
		struct vertexInput {
			float4 vertex : POSITION;
		};

		// Output from vertex shader into fragment shader
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 worldPos : TEXCOORD0;
		};

		// VERTEX SHADER
		vertexOutput vert(vertexInput input) {
			vertexOutput output;
			output.pos = UnityObjectToClipPos(input.vertex);
			// Calculate the world position coordinates to pass to the fragment shader
			output.worldPos = mul(unity_ObjectToWorld, input.vertex);

			output.worldPos.x += _GridThickness / 2;
			output.worldPos.z += _GridThickness / 2;

			return output;
		}

		// FRAGMENT SHADER
		float4 frag(vertexOutput input) : COLOR{
			if (frac((input.worldPos.x + 0.5) / _GridSpacing) < _GridThickness || frac((input.worldPos.z + 0.5f) / _GridSpacing) < _GridThickness) {
				return _GridColour;
			}
			else {
				return _BaseColour;
			}
		}
		ENDCG
	}
	}
}