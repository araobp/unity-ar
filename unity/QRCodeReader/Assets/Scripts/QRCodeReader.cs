using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using ZXing;

public class QRCodeReader : MonoBehaviour
{
    [SerializeField]
    ARCameraBackground m_ARCameraBackground;

    [SerializeField]
    Text m_TextResult;

    BarcodeReader m_BarcodeReader;

    // Start is called before the first frame update
    void Start()
    {
        m_BarcodeReader = new BarcodeReader();
    }

    public void DecodeQRCode()
    {
        m_TextResult.text = "...";

        StartCoroutine("RecognizeQRCode");
    }

    // https://siddeshb88.medium.com/how-to-augment-in-real-world-using-qrcode-e1a344416288
    IEnumerator RecognizeQRCode()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);

        for (int i = 0; i < 100; i++)  // Time out
        {

            UpdateCameraTexture(texture);

            byte[] barcodeBitmap = texture.GetRawTextureData();

            LuminanceSource source = new RGBLuminanceSource(barcodeBitmap, texture.width, texture.height);
            var result = m_BarcodeReader.Decode(source);

            if (result != null && result.Text != "")
            {
                string QRContents = result.Text;

                if (QRContents.StartsWith("http"))
                {
                    Application.OpenURL(QRContents);
                }
                else
                {
                    m_TextResult.text = result.Text;
                }
                break;
            }
            yield return new WaitForSeconds(0.05F);
        }

        Destroy(texture);
    }

    void UpdateCameraTexture(Texture2D texture)
    {
        RenderTexture renderTexture = new RenderTexture(texture.width, texture.height, 24, RenderTextureFormat.ARGB32);

        // Copy the camera background to a RenderTexture
        Graphics.Blit(null, renderTexture, m_ARCameraBackground.material);

        // Copy the RenderTexture from GPU to CPU
        var activeRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        RenderTexture.active = activeRenderTexture;

        Destroy(renderTexture);
    }

}