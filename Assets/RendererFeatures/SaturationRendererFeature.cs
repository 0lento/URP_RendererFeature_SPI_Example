using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

class SaturationRendererFeature : ScriptableRendererFeature
{
	public float m_Intensity = 1.0f;
	private Material m_Material;
	private SaturationPass m_RenderPass = null;

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		if (m_RenderPass == null)
			return;

		m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
		m_RenderPass.SetTarget(renderer.cameraColorTarget, m_Intensity);
		renderer.EnqueuePass(m_RenderPass);
	}

	public override void Create()
	{
		Shader shader = Shader.Find("Hidden/Saturation");
		if (shader == null)
			return;
		m_Material = new Material(shader);
		m_RenderPass = new SaturationPass(m_Material);
	}

	protected override void Dispose(bool disposing)
	{
		CoreUtils.Destroy(m_Material);
	}
}