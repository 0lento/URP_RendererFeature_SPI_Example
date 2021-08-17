Shader "Hidden/Saturation"
{
	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	struct Attributes
	{
		float4 positionHCS : POSITION;
		float2 uv          : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS  : SV_POSITION;
		float2 uv          : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		// Note: The pass is setup with a mesh already in CS
		// Therefore, we can just output vertex position
		output.positionCS = float4(input.positionHCS.xyz, 1.0);

		#if UNITY_UV_STARTS_AT_TOP
		output.positionCS.y *= -1;
		#endif

		output.uv = input.uv;

		return output;
	}

	ENDHLSL
	
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			// 0 - Saturation Pass
			Name "Saturation"

			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				TEXTURE2D_X(_CameraColorTexture);
				SAMPLER(sampler_CameraColorTexture);
				float _Intensity;

				float4 frag(Varyings input) : SV_Target
				{
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
					float4 color = SAMPLE_TEXTURE2D_X(_CameraColorTexture, sampler_CameraColorTexture, input.uv.xy);
					float luminance = dot(color.rgb, float3(0.22, 0.707, 0.071));
					return lerp(luminance, color, _Intensity);
				}
			ENDHLSL
		}
		Pass
		{
			// 1 - Copy Pass
			Name "Copy"

			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				TEXTURE2D_X(_CopyTexture);
				SAMPLER(sampler_CopyTexture);

				float4 frag(Varyings input) : SV_Target
				{
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
					return SAMPLE_TEXTURE2D_X(_CopyTexture, sampler_CopyTexture, input.uv);
				}
			ENDHLSL
		}
	}
}