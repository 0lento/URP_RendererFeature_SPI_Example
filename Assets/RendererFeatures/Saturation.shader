Shader "Hidden/Saturation"
{
	HLSLINCLUDE
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	struct Attributes
	{
		float4 positionOS  : POSITION;
		float2 uv          : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS  : SV_POSITION;
		float4 uv          : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

		output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
		output.uv.xy = input.uv;
		float4 projPos = output.positionCS * 0.5;
		projPos.xy = projPos.xy + projPos.w;
		output.uv.zw = projPos.xy;

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
	}
}