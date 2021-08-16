# URP Renderer Feature Example

Renderer Feature example for Unity URP that works in VR + SPI.

Background: [cmd.Blit has been broken in URP VR + SPI for while](https://issuetracker.unity3d.com/issues/xr-sdk-urp-game-view-is-rendered-grey-for-left-eye-and-black-for-right-eye-when-using-single-pass-instanced-and-render-feature) so we have to use cmd.DrawMesh instead. This project implements simple saturation effect using mentioned DrawMesh API.

Project has been made with Unity 2020.3 LTS but is also compatible with newer Unity versions.

This project mimics what Unity did with URP SSAO but tries to put it in simpler example. Original SSAO files here:
* [URP v10 SSAO Renderer Feature](https://github.com/Unity-Technologies/Graphics/blob/v10.5.1/com.unity.render-pipelines.universal/Runtime/RendererFeatures/ScreenSpaceAmbientOcclusion.cs)
* [URP v10 SSAO Shader](https://github.com/Unity-Technologies/Graphics/blob/v10.5.1/com.unity.render-pipelines.universal/Shaders/Utils/ScreenSpaceAmbientOcclusion.shader)
