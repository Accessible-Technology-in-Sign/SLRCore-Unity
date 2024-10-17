using System;
using System.Collections.Generic;
using System.IO;
using Mediapipe;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using UnityEngine;
using RunningMode = Mediapipe.Tasks.Vision.Core.RunningMode;

namespace Model {
    public class ImageMPResultWrapper<T> {
        public T Result { get; }
        public Texture2D Image { get; }

        public ImageMPResultWrapper(T result, Texture2D image) {
            Result = result;
            Image = image;
        }
    }

    public class MediapipeHandModelManager {
        private readonly HandLandmarker graph;

        private readonly Dictionary<string, Action<ImageMPResultWrapper<HandLandmarkerResult>>> callbacks =
            new Dictionary<string, Action<ImageMPResultWrapper<HandLandmarkerResult>>>();

        private readonly Dictionary<long, Texture2D> outputInputLookup = new();
        private readonly RunningMode runningMode;

        private static class Config {
            public static readonly float HAND_DETECTION_CONFIDENCE = 0.5f;
            public static readonly float HAND_TRACKING_CONFIDENCE = 0.5f;
            public static readonly float HAND_PRESENCE_CONFIDENCE = 0.5f;
            public static int NUM_HANDS = 1;
        }

        public MediapipeHandModelManager(byte[] modelAssetBuffer, Mediapipe.Tasks.Vision.Core.RunningMode runningMode) {
            this.runningMode = runningMode;
            if (runningMode != Mediapipe.Tasks.Vision.Core.RunningMode.LIVE_STREAM)
                graph = HandLandmarker.CreateFromOptions(new HandLandmarkerOptions(
                    new Mediapipe.Tasks.Core.BaseOptions(
                        modelAssetBuffer: modelAssetBuffer
                    ),
                    minHandDetectionConfidence: Config.HAND_DETECTION_CONFIDENCE,
                    minTrackingConfidence: Config.HAND_TRACKING_CONFIDENCE,
                    minHandPresenceConfidence: Config.HAND_PRESENCE_CONFIDENCE,
                    numHands: Config.NUM_HANDS,
                    runningMode: runningMode
                ));
            else
                graph = HandLandmarker.CreateFromOptions(new HandLandmarkerOptions(
                    new Mediapipe.Tasks.Core.BaseOptions(
                        modelAssetBuffer: modelAssetBuffer
                    ),
                    minHandDetectionConfidence: Config.HAND_DETECTION_CONFIDENCE,
                    minTrackingConfidence: Config.HAND_TRACKING_CONFIDENCE,
                    minHandPresenceConfidence: Config.HAND_PRESENCE_CONFIDENCE,
                    numHands: Config.NUM_HANDS,
                    runningMode: runningMode,
                    resultCallback: (i, _, timestampMs) => {
                        foreach (var cb in callbacks.Values) {
                            cb(new ImageMPResultWrapper<HandLandmarkerResult>(
                                i,
                                outputInputLookup.TryGetValue(timestampMs, out var value)
                                    ? value
                                    : null // Empties.EMPTY_BITMAP
                                //TODO: maybe pass in transformed image from MP graph?
                            ));
                        }

                        outputInputLookup.Remove(timestampMs);
                    }
                ));
        }


        private int FileCounter;
        public void Single(Texture2D image, long timestamp) {
            Image img = new Image(image.format.ToImageFormat(), image);
            // Image img = new Image(ImageFormat.Types.Format.Srgba, image.width, image.height, ImageFormat.Types.Format.Srgba.NumberOfChannels() * image.width, image.GetRawTextureData<byte>());
            // Image img = new Image(image.GetNativeTexturePtr(), false);
            // Color32[] pixels = new Color32[image.width * image.height];
            // img.TryReadPixelData(pixels);
            // var texture = new Texture2D(image.width, image.height);
            // texture.SetPixels32(pixels);
            // texture.Apply();
            //
            // File.WriteAllBytes(Application.dataPath + "/IMGDUMP/" + FileCounter + ".png", texture.EncodeToPNG());
            // FileCounter++;
            Debug.Log("Image MP: " + img.Width() + "x"  + img.Height() + ", " + img.ImageFormat());
            switch (runningMode) {
                case Mediapipe.Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                    outputInputLookup[timestamp] = image;
                    graph.DetectAsync(img, timestamp);
                    break;
                case Mediapipe.Tasks.Vision.Core.RunningMode.IMAGE:
                    var result = graph.Detect(img);
                    foreach (var cb in callbacks.Values) {
                        cb(new ImageMPResultWrapper<HandLandmarkerResult>(result, image));
                    }

                    break;
                case Mediapipe.Tasks.Vision.Core.RunningMode.VIDEO:
                    var videoResult = graph.DetectForVideo(img, timestamp);
                    foreach (var cb in callbacks.Values) {
                        cb(new ImageMPResultWrapper<HandLandmarkerResult>(videoResult, image));
                    }

                    break;
            }
        }

        public void AddCallback(string name, Action<ImageMPResultWrapper<HandLandmarkerResult>> callback) {
            callbacks[name] = callback;
        }
        
        public void RemoveCallback(string name) {
            callbacks.Remove(name);
        }
    }
}


