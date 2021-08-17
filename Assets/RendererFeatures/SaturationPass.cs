using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class SaturationPass : ScriptableRenderPass
{
	private float m_Intensity;
	private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Saturation");
	private Material m_Material;
	private RenderTargetIdentifier m_CameraColorTarget;
	private RenderTargetIdentifier m_SaturationTextureTarget = new RenderTargetIdentifier(s_SaturationTextureID, 0, CubemapFace.Unknown, -1);
	private static readonly int s_SaturationTextureID = Shader.PropertyToID("_SaturationTexture");

	private enum ShaderPasses
	{
		Saturation = 0,
		Copy = 1,
	}

	public SaturationPass(Material material)
	{
		m_Material = material;
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
		cmd.GetTemporaryRT(s_SaturationTextureID, saturationDescriptor, FilterMode.Bilinear);
	}

	private void Render(CommandBuffer cmd, RenderTargetIdentifier target, Material material, ShaderPasses pass)
	{
		cmd.SetRenderTarget(target);
		cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, 0, (int)pass);
	}

	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		if (m_Material == null)
			return;

		CommandBuffer cmd = CommandBufferPool.Get();

		using (new ProfilingScope(cmd, m_ProfilingSampler))
		{
			m_Material.SetFloat("_Intensity", m_Intensity);

			Render(cmd, m_SaturationTextureTarget, m_Material, ShaderPasses.Saturation);

			cmd.SetGlobalTexture("_CopyTexture", m_SaturationTextureTarget);
			Render(cmd, m_CameraColorTarget, m_Material, ShaderPasses.Copy);
		}

		context.ExecuteCommandBuffer(cmd);
		CommandBufferPool.Release(cmd);
	}

	public override void OnCameraCleanup(CommandBuffer cmd)
	{
		cmd.ReleaseTemporaryRT(s_SaturationTextureID);
	}
}