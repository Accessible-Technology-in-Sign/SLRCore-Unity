using UnityEngine;
using System;
using System.Collections.Generic;
//New camera.cs file. Need it to use Unity Web Camera Texture instead of media pipe.

public interface CameraI<T, U> {
    //public void AddCall
    public void AddCallback(string name, Action<U> callback);
    public void RemoveCallback(string name);

    void poll();
    void pause();

}

public class CameraIProperties {

    public bool = enableISO = false;

    public int preferredWidth = 1280;

    //is this specific to MediaPipe?
    public ImageSource.ResolutionStruct[] prefereredResolutions = {
        new(512, 512, 0),
        new(640, 480, 0),
        new(1280, 720, 0),
    };
    //TODO: implement correct version of ImageSource resolution once I have clarification

}

public class WebCameraI : CameraI<string, string> {
    private WebCamTexture webCamTexture;
    
    public WebCameraI(CameraIProperties properties) {
        webCamTexture = new WebCamTexture(properties.preferredWidth, properties.prefereredResolutions);
    }

    protected Dictionary<string, Action<string>> callbacks = new();

    public void AddCallback(string name, Action<string> callback) {
        callbacks.Add(name, callback);
    }

    public void RemoveCallback(string name) {
        if (callbacks.ContainsKey(name)) {
            callbacks.Remove(name);
        }
    }

    public void poll() {
        webCamTexture.Play();
        if (!webCamTexture.isPlaying) {
            throw new Exception("Cannot get the camera to play");
        }
    }

    public void pause() {
        webCamTexture.Pause();
    }
}
