using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamSetup : MonoBehaviour
{
    WebCamTexture webCamTexture;
    // Start is called before the first frame update
    void Start()
    {

        WebCamDevice my_device = new WebCamDevice();
        WebCamDevice[] devices = WebCamTexture.devices;

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log(devices[i].name);
            my_device = devices[0];
        }

        webCamTexture = new WebCamTexture(my_device.name);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = webCamTexture;
        webCamTexture.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
