using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

//Electronic Distance Measurement (EDM) with LiDAR
public class EDM : MonoBehaviour
{
    [SerializeField]
    ARCameraManager m_ARCameraManager;

    [SerializeField]
    AROcclusionManager m_OcclusionManager;

    [SerializeField]
    Text m_TextDistance;

    Texture2D m_CameraTexture;

    float m_Distance = 0F;

    void Start()
    {
        if (LiDARsupported())
        {
            m_ARCameraManager.frameReceived += OnCameraFrameEventReceived;
        }
    }

    private void Update()
    {
        m_TextDistance.text = $"{m_Distance.ToString("F2")}m";
    }

    // https://forum.unity.com/threads/check-if-armeshing-supported-on-device.909839/#post-6048380
    bool LiDARsupported()
    {
        var subsystems = new List<XRMeshSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        bool supported = false;
        if (subsystems.Count > 0)
        {
            supported = true;
        }
        return supported;
    }

    void OnCameraFrameEventReceived(ARCameraFrameEventArgs cameraFrameEventArgs)
    {
        UpdateDistance();
    }

    // https://answers.unity.com/questions/1271693/reading-pixel-data-from-materialmaintexture-return.html?sort=votes
    void UpdateDistance()
    {
        // https://github.com/Unity-Technologies/arfoundation-samples/blob/6296272a416925b56ce85470e0c7bef5c913ec0c/Assets/Scripts/CpuImageSample.cs
        // Attempt to get the latest environment depth image. If this method succeeds,
        // it acquires a native resource that must be disposed (see below).
        if (m_OcclusionManager && m_OcclusionManager.TryAcquireEnvironmentDepthCpuImage(out XRCpuImage image))
        {
            using (image)
            {
                m_Distance = PerformEDM(image);
            }
        }
    }

    // https://github.com/Unity-Technologies/arfoundation-samples/blob/6296272a416925b56ce85470e0c7bef5c913ec0c/Assets/Scripts/CpuImageSample.cs
    float PerformEDM(XRCpuImage cpuImage)
    {
        // If the texture hasn't yet been created, or if its dimensions have changed, (re)create the texture.
        // Note: Although texture dimensions do not normally change frame-to-frame, they can change in response to
        //    a change in the camera resolution (for camera images) or changes to the quality of the human depth
        //    and human stencil buffers.
        if (m_CameraTexture == null)
        {
            m_CameraTexture = new Texture2D(cpuImage.width, cpuImage.height, cpuImage.format.AsTextureFormat(), false);

        }

        // For display, we need to mirror about the vertical access.
        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, cpuImage.format.AsTextureFormat(), XRCpuImage.Transformation.MirrorY);

        // Get the Texture2D's underlying pixel buffer.
        var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();


        // Perform the conversion.
        try
        {
            cpuImage.Convert(conversionParams, rawTextureData);
        }
        finally
        {
            cpuImage.Dispose();
        }

        // "Apply" the new pixel data to the Texture2D.
        m_CameraTexture.Apply();

        Color pixel = m_CameraTexture.GetPixel(m_CameraTexture.width / 2, m_CameraTexture.height / 2);
        return pixel.r;
    }
}


