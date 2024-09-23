using System;
using System.Collections.Generic;
using Mediapipe.Unity;

public interface CameraI<T, U>
{
    public void AddCallback(string name, Action<U> callback);
    public void RemoveCallback(string name);

    public void poll();
    public void pause();
}

public class CameraIProperties
{
    public bool enableISO = false;
    // camera selector
    public int preferredWidth = 1280;
    public ImageSource.ResolutionStruct[] prefereredResolutions = {
        new(512, 512, 0),
        new(640, 480, 0),
        new(1280, 720, 0),
    };
}

public class WebCameraI : CameraI<string, string>
{
    private WebCamSource webcamSource;

    public WebCameraI(CameraIProperties properties)
    {
        webcamSource = new WebCamSource(properties.preferredWidth, properties.prefereredResolutions);
    }

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
        webcamSource.Play();
        if (!webcamSource.isPlaying)
        {
            throw new Exception("Cannot get camera to play");
        }
        // TODO check if required: WaitForEndOfFrame

    }

    public void pause()
    {
        webcamSource.Pause();
    }
}