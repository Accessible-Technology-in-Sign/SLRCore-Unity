using System;
using System.Collections.Generic;
using Mediapipe;
using UnityEngine;

namespace Camera {
    
    public class StreamCamera: MonoBehaviour, ICamera {
        [NonSerialized] private Dictionary<string, Action<Texture2D>> callbacks = new();
        public bool enableISO = true;
        public CameraSelector cameraSelector = CameraSelector.FIRST_FRONT_CAMERA;
        [NonSerialized] private WebCamTexture webCamTexture;

        public bool polling = true;

        // public bool PollTesting {
        //     get => polling;
        //     set {
        //         if (value) Poll();
        //         else Pause();
        //     }
        // }
        //
        // private Resolution getHighestResolution(WebCamDevice device) {
        //     int idx = 0;
        //     int currMaxHeight = -1;
        //     for (var i = 0; i < device.availableResolutions.Length; i++) {
        //         if (device.availableResolutions[i].height > currMaxHeight) {
        //             currMaxHeight = device.availableResolutions[i].height;
        //             idx = i;
        //         }
        //     }
        //     return device.availableResolutions[idx];
        // } 
        

        public void UpdateProps() {
            if (WebCamTexture.devices.Length <= 0) throw new Exception("Camera not connected");
                camera:
                foreach (var device in WebCamTexture.devices) {
                    // var bestWidth = getHighestResolution(device).width;
                    // var bestHeight = getHighestResolution(device).height;
                    switch (cameraSelector) {
                        case CameraSelector.FIRST_FRONT_CAMERA:
                            if (device.isFrontFacing) {
                                webCamTexture = new WebCamTexture(device.name, 1280, 720, 30);
                                goto ISO;
                            }

                            break;
                        case CameraSelector.FIRST_BACK_CAMERA:
                            if (!device.isFrontFacing) {
                                webCamTexture = new WebCamTexture(device.name, 1280, 720, 30);
                                goto ISO;
                            }

                            break;
                }

                // TODO: use NatCam? Can disable ISO from there + better interface for frontcam etc.
                }
                
                webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name);
                ISO:
                return;
        }
        
        public void Poll() {
            UpdateProps();
            polling = true;
            webCamTexture.Play();
        }

        public void Pause() {
            polling = false;
            webCamTexture.Pause();
        }
        private void Update() {
            if (polling) {
                if(webCamTexture == null || !webCamTexture.isPlaying) {
                Poll();
                }
                foreach (var callback in callbacks) {
                    // var imageNativePointer = webCamTexture.GetNativeTexturePtr();
                    // if (imageNativePointer != IntPtr.Zero) {
                    //     Debug.Log("PICKED UP");
                    //     callback.Value(new Image(imageNativePointer, false));
                    // }
                    // else {
                        // Debug.Log("NOT PICKED UP");
                        var texture2d = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.RGBA32, false);
                        // texture2d.SetPixels32(webCamTexture.GetPixels32());
                        // texture2d.LoadRawTextureData(webCamTexture.GetNativeTexturePtr(), webCamTexture.width * webCamTexture.height * 4);
                        // texture2d.UpdateExternalTexture(webCamTexture.GetNativeTexturePtr());
                        
                        // I think this removes lag but it will not work properly with color spaces?
                        // Might not be reliable
                        Graphics.CopyTexture(webCamTexture, texture2d);
                        texture2d.Apply(false);
                        callback.Value(texture2d);
                    // }
                }
            }
            else {
                if (webCamTexture != null && webCamTexture.isPlaying)
                    Pause();
            }
        }

        public void AddCallback(string name, Action<Texture2D> callback) {
            if (callbacks.ContainsKey(name)) callbacks.Remove(name);
            callbacks.Add(name, callback);
        }

        public void RemoveCallback(string name) {
            callbacks.Remove(name);
        }
    }
}