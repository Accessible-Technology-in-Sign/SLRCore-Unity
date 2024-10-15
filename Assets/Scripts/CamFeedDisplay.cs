using UnityEngine;
using UnityEngine.UI;

public class CameraFeedDisplay : MonoBehaviour {
     
    public RawImage rawImage;
    public WebCamTexture webCameraTexture;

    void Start() {

        webCameraTexture = new WebCamTexture();

        rawImage.texture = webCameraTexture;

        webCameraTexture.Play();


    }

    void OnDisable()
    {
        if (webCameraTexture != null && webCameraTexture.isPlaying)
        {
            webCameraTexture.Stop();
        }
    }

}
