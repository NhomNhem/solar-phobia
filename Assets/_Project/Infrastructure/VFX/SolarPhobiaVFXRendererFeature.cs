using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace SolarPhobia.Infrastructure.VFX
{
    /// <summary>
    /// URP Renderer Feature for Solar Phobia's custom post-processing effects.
    /// Uses the Unity 6 Render Graph API.
    /// </summary>
    public class SolarPhobiaVFXRendererFeature : ScriptableRendererFeature
    {
        [Header("Settings")]
        public RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingPostProcessing;

        [Header("Materials")]
        public Material heatWallMaterial;
        public Material sensoryDecayMaterial;

        private SolarPhobiaVFXPass _vfxPass;

        public override void Create()
        {
            _vfxPass = new SolarPhobiaVFXPass(heatWallMaterial, sensoryDecayMaterial)
            {
                renderPassEvent = renderEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Only render in Game view or Scene view if desired
            if (renderingData.cameraData.cameraType == CameraType.Game || renderingData.cameraData.cameraType == CameraType.SceneView)
            {
                renderer.EnqueuePass(_vfxPass);
            }
        }

        protected override void Dispose(bool disposing)
        {
            _vfxPass = null;
        }
    }

    public class SolarPhobiaVFXPass : ScriptableRenderPass
    {
        private Material _heatWallMat;
        private Material _sensoryDecayMat;

        public SolarPhobiaVFXPass(Material heatWall, Material sensoryDecay)
        {
            _heatWallMat = heatWall;
            _sensoryDecayMat = sensoryDecay;
        }

        private class PassData
        {
            public Material heatWallMat;
            public Material sensoryDecayMat;
            public TextureHandle activeColor;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            if (resourceData.isActiveTargetBackBuffer)
                return;

            TextureHandle activeColor = resourceData.activeColorTexture;
            if (!activeColor.IsValid())
                return;

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Solar Phobia VFX Pass", out var passData))
            {
                passData.heatWallMat = _heatWallMat;
                passData.sensoryDecayMat = _sensoryDecayMat;
                passData.activeColor = activeColor;

                // We read from and write to the active color texture
                builder.UseTexture(activeColor, AccessFlags.ReadWrite);
                builder.SetRenderAttachment(activeColor, 0, AccessFlags.Write);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    // 1. Apply Heat Wall Edge Burn
                    if (data.heatWallMat != null)
                    {
                        Blitter.BlitTexture(context.cmd, data.activeColor, new Vector4(1, 1, 0, 0), data.heatWallMat, 0);
                    }

                    // 2. Apply Sensory Decay
                    if (data.sensoryDecayMat != null)
                    {
                        Blitter.BlitTexture(context.cmd, data.activeColor, new Vector4(1, 1, 0, 0), data.sensoryDecayMat, 0);
                    }
                });
            }
        }
    }
}
