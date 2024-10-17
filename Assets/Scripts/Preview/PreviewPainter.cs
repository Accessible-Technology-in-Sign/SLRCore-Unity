using System;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Image = Mediapipe.Image;

namespace Preview {
    public enum PainterMode {
        IMAGE_ONLY,
        SKELETON_ONLY,
        IMAGE_AND_SKELETON
    }
    public class UnityMPHandPreviewPainter: MonoBehaviour {
        
        public PainterMode painterMode = PainterMode.IMAGE_AND_SKELETON;
        private HandLandmarkerResultAnnotationController _handLandmarkerResultAnnotationController;
        private RawImage screen;
        

        private HandLandmarkerResult result;
        private Texture2D image;
        private bool visible = true;

        private void Awake() {
            _handLandmarkerResultAnnotationController = gameObject.GetComponentInChildren(typeof(HandLandmarkerResultAnnotationController)) as HandLandmarkerResultAnnotationController;
            screen = GetComponent<RawImage>();
        }

        public void Hide() {
            this.visible = false;
        }

        public void Show() {
            this.visible = true;
        }
        
        
        public void UpdateLandmarks(HandLandmarkerResult result) {
            this.result = result;
        }
        
        public void UpdateImage(Texture2D image) {
            Debug.Log("Updating Image");
            this.image = image;
        }
        
        
        
        void Update() {
            // if (!visible) {
            //     if (enabled) {
            //         _handLandmarkerResultAnnotationController.DrawNow(default);
            //         enabled = false;
            //         screen.texture = null;
            //         screen.enabled = false;
            //     }
            // }
            // else {
            //     if (!enabled) {
            //         enabled = true;
            //         screen.enabled = true;
            //     }
            if (image != null) {
                // Debug.Log(image.width + ", " + image.height);
                // Debug.Log("Results: " + result.handLandmarks.Count);
                screen.rectTransform.sizeDelta = new Vector2(image.width, image.height);
                screen.texture = image;
                _handLandmarkerResultAnnotationController.DrawNow(result);
            }
            // }
        }
        
        
    }
}