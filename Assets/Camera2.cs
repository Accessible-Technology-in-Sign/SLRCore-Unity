using System;
using System.Collections.Generic;
using UnityEngine;

public interface CameraI<T, U>
{
    public void AddCallback(string name, Action<U> callback);
    public void RemoveCallback(string name);

    public void poll();
    public void pause();
}

public class CameraIProperties
{
    //camera selector
    public bool enableISO = false;

    public int preferredWidth = 1280;
    public ImageSource.ResolutionStruct[] prefereredResolutions = {
        new(512, 512, 0),
        new(640, 480, 0),
        new(1280, 720, 0),
    };
}

public class WebCameraI : CameraI<string, string>
{
    private WebCamTexture webCamTexture = new WebCamTexture();
    Renderer renderer = GetComponent<Renderer>();
    renderer.material.mainTexture = webCamTexture;

    protected Dictionary<string, Action<string>> callbacks = new();

    public void AddCallback(string name, Action<string> callback)
    {
        callbacks.Add(name, callback);
    }

    public void RemoveCallback(string name)
    {
        if (callbacks.ContainsKey(name)) callbacks.Remove(name);
    }

    public void poll()
    {
        webCamTexture.Play();
        if (!webCamTexture.isPlaying)
        {
            throw new Exception("The camera is not playing.")
        }
    }

    public void pause()
    {
        webCamTexture.pause();
    }
}
