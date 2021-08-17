using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class SaturationPass : ScriptableRenderPass
{
	private float m_Intensity;
	private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Saturation");
	private Material m_Material;
	private RenderTargetIdentifier m_CameraColorTarget;
	private RenderTargetHandle m_SaturationTexture;

	public SaturationPass(Material material)
	{
		m_Material = material;
		m_SaturationTexture.Init("_SaturationTexture");
		renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
	}

	public void SetTarget(RenderTargetIdentifier cameraColorTarget, float intensity)
	{
		m_CameraColorTarget = cameraColorTarget;
		m_Intensity = intensity;
	}

	public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
	{
		RenderTextureDescriptor saturationDescriptor = renderingData.cameraData.cameraTargetDescriptor;
		saturationDescriptor.depthBufferBits = 0;
		cmd.GetTemporaryRT(m_SaturationTexture.id, saturationDescriptor, FilterMode.Bilinear);
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		if (m_Material == null)
			return;

		CommandBuffer cmd = CommandBufferPool.Get();

		using (new ProfilingScope(cmd, m_ProfilingSampler))
		{
			m_Material.SetFloat("_Intensity", m_Intensity);

			cmd.Blit(m_CameraColorTarget, m_SaturationTexture.Identifier(), m_Material, 0);
			cmd.Blit(m_SaturationTexture.Identifier(), m_CameraColorTarget);
		}

		context.ExecuteCommandBuffer(cmd);
		CommandBufferPool.Release(cmd);
	}

	public override void OnCameraCleanup(CommandBuffer cmd)
	{
		cmd.ReleaseTemporaryRT(m_SaturationTexture.id);
	}
}