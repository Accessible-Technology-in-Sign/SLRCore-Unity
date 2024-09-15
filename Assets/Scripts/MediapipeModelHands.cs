using System;
using System.Collections.Generic;
using Mediapipe;
using Mediapipe.Tasks.Vision.HandLandmarker;
using RunningMode = Mediapipe.Tasks.Vision.Core.RunningMode;

public class ImageMPResultWrapper<T>
{
    public T Result { get; }
    public Image Image { get; }

    public ImageMPResultWrapper(T result, Image image)
    {
        Result = result;
        Image = image;
    }
}

public class MediapipeHandModelManager
{
    private readonly HandLandmarker graph;
    private readonly Dictionary<string, Action<ImageMPResultWrapper<HandLandmarkerResult>>> callbacks = new Dictionary<string, Action<ImageMPResultWrapper<HandLandmarkerResult>>>();

    private readonly Dictionary<long, Image> outputInputLookup = new Dictionary<long, Image>();
    private readonly RunningMode runningMode;

    private static class Config {
        public static readonly float HAND_DETECTION_CONFIDENCE = 0.5f;
        public static readonly float HAND_TRACKING_CONFIDENCE = 0.5f;
        public static readonly float HAND_PRESENCE_CONFIDENCE = 0.5f;
        public static int NUM_HANDS = 1; 
    }

    public MediapipeHandModelManager(byte[] modelAssetBuffer, Mediapipe.Tasks.Vision.Core.RunningMode runningMode)
    {
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
                    foreach (var cb in callbacks.Values){
                        cb(new ImageMPResultWrapper<HandLandmarkerResult>(
                            i,
                            outputInputLookup.TryGetValue(timestampMs, out var value) ? value : null // Empties.EMPTY_BITMAP
                        ));
                    }
                    outputInputLookup.Remove(timestampMs);
                }
            ));
    }

    public void Single(Image img, long timestamp)
    {
        switch (runningMode)
        {
            case Mediapipe.Tasks.Vision.Core.RunningMode.LIVE_STREAM:
                outputInputLookup[timestamp] = img;
                graph.DetectAsync(img, timestamp);
                break;
            case Mediapipe.Tasks.Vision.Core.RunningMode.IMAGE:
                var result = graph.Detect(img);
                foreach (var cb in callbacks.Values)
                {
                    cb(new ImageMPResultWrapper<HandLandmarkerResult>(result, img));
                }
                break;
            case Mediapipe.Tasks.Vision.Core.RunningMode.VIDEO:
                var videoResult = graph.DetectForVideo(img, timestamp);
                foreach (var cb in callbacks.Values)
                {
                    cb(new ImageMPResultWrapper<HandLandmarkerResult>(videoResult, img));
                }
                break;
        }
    }

    public void AddCallback(string name, Action<ImageMPResultWrapper<HandLandmarkerResult>> callback)
    {
        callbacks[name] = callback;
    }

    public void RemoveCallback(string name)
    {
        callbacks.Remove(name);
    }
}


