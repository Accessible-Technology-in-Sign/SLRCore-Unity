using System;
using System.Collections.Generic;
using Mediapipe.Unity;
using UnityEngine;

using TextureFramePool = Mediapipe.Unity.Experimental.TextureFramePool;

namespace Camera {
    public class ImageSourceCamera : MonoBehaviour, ICamera {
        [SerializeField] private int _preferredDefaultWebCamWidth = 1280;
        [SerializeField] private ImageSource.ResolutionStruct[] _defaultAvailableWebCamResolutions = new ImageSource.ResolutionStruct[] {
            new ImageSource.ResolutionStruct(176, 144, 30),
            new ImageSource.ResolutionStruct(320, 240, 30),
            new ImageSource.ResolutionStruct(424, 240, 30),
            new ImageSource.ResolutionStruct(640, 360, 30),
            new ImageSource.ResolutionStruct(640, 480, 30),
            new ImageSource.ResolutionStruct(848, 480, 30),
            new ImageSource.ResolutionStruct(960, 540, 30),
            new ImageSource.ResolutionStruct(1280, 720, 30),
            new ImageSource.ResolutionStruct(1600, 896, 30),
            new ImageSource.ResolutionStruct(1920, 1080, 30),
        };
        
        WebCamSource webCamSource;
        public bool polling = true;
        
        private TextureFramePool _textureFramePool;


        void Awake() {
            webCamSource = new WebCamSource(
                _preferredDefaultWebCamWidth,
                _defaultAvailableWebCamResolutions
            );
        }

        public void Poll() {
            webCamSource.Play();
        }

        public void Pause() {
            webCamSource.Pause();
        }

        public void Update() {
            if (polling && !webCamSource.isPlaying) {
                Poll();
            }
            
            if (!polling && webCamSource.isPlaying) {
                Pause();
            }
            
            // if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
            // {
            //     return;
            // }
            // textureFrame.ReadTextureAsync(webCamSource.GetCurrentTexture(), false, false);
            foreach (var callback in callbacks) {
                var texture = new Texture2D(webCamSource.textureWidth, webCamSource.textureHeight);
                texture.UpdateExternalTexture(webCamSource.GetCurrentTexture().GetNativeTexturePtr());
                callback.Value(texture);
            }
            
        }
        
        [NonSerialized] private Dictionary<string, Action<Texture2D>> callbacks = new();
        
        public void AddCallback(string name, Action<Texture2D> callback) {
            if (callbacks.ContainsKey(name)) callbacks.Remove(name);
            callbacks.Add(name, callback);
        }

        public void RemoveCallback(string name) {
            callbacks.Remove(name);
        }
    }
}