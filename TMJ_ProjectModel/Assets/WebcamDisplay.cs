using UnityEngine;

public class WebcamDisplay : MonoBehaviour
{
    public Renderer renderer;
    private WebCamTexture webcamTexture;

    void Start()
    {
        webcamTexture = new WebCamTexture();
        renderer.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    void OnDestroy()
    {
        webcamTexture.Stop();
    }
}