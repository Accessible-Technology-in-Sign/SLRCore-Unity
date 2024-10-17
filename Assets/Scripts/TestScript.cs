using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe;
using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity;
using Model;
using TMPro;
using Image = Mediapipe.Image;
using RunningMode = Mediapipe.Tasks.Vision.Core.RunningMode;

public class SimpleSLR : MonoBehaviour
{
    

    [SerializeField] private Preview.UnityMPHandPreviewPainter screen;
    [SerializeField] private Camera.StreamCamera camera;

    private SLRTfLiteModel<string> recognizer;
    private Buffer<HandLandmarkerResult> buffer;
    public MediapipeHandModelManager posePredictor;

    [SerializeField] private TextAsset _modelFile;
    [SerializeField] private TextAsset _mappingFile;
    [SerializeField] private TextAsset _mediapipeGraph;
    [SerializeField] private bool _isGPU;
    [SerializeField] private bool _isInterpolating;
    [SerializeField] private TextMeshProUGUI output;
    [SerializeField] private Button trigger;
    private string lastTranslation = "";

    private class Config
    {
        public static readonly int NUM_INPUT_FRAMES = 60;
        public static readonly int NUM_INPUT_POINTS = 21;
    }

    void Update()
    {
        output.text = lastTranslation;
        trigger.onClick.AddListener(TriggerOnClick);

    }

    void TaskOnClick()
    {
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("You have clicked the button!");
    }

    void Awake()
    {
        buffer = new Buffer<HandLandmarkerResult>();
        string[] mapping = _mappingFile.text.Split("\n");

        for (int i = 0; i < mapping.Length; i++)
        {
            mapping[i] = mapping[i].Trim().ToLower();
        }


        recognizer = new SLRTfLiteModel<string>(_modelFile, new List<string>(mapping));
        posePredictor = new MediapipeHandModelManager(_mediapipeGraph.bytes, RunningMode.LIVE_STREAM);

        posePredictor.AddCallback("buffer", result =>
        {
            buffer.AddElement(result.Result);
            screen.UpdateLandmarks(result.Result);
            if (result.Image != null)
                screen.UpdateImage(result.Image);
            else
                Debug.Log("Got null image");
        });
        buffer.AddCallback("trigger", bufferedResults =>
        {
            List<float> inputArray = new List<float>();

            if (_isInterpolating && bufferedResults.Count < Config.NUM_INPUT_FRAMES && bufferedResults.Count > 0)
            {
                foreach (var landmark in bufferedResults)
                {
                    for (int j = 0; j < Config.NUM_INPUT_POINTS; j++)
                    {
                        if (landmark.handLandmarks.Count <= 0 || landmark.handLandmarks[0].landmarks.Count <= 0)
                            return; // Exit if no landmark is present

                        inputArray.Add(landmark.handLandmarks[0].landmarks[j].x);
                        inputArray.Add(landmark.handLandmarks[0].landmarks[j].y);
                    }
                }

                var midpoint = bufferedResults[bufferedResults.Count / 2];

                for (int i = 0; i < Config.NUM_INPUT_FRAMES - bufferedResults.Count; i++)
                {
                    for (int j = 0; j < Config.NUM_INPUT_POINTS; j++)
                    {
                        if (midpoint.handLandmarks.Count <= 0 || midpoint.handLandmarks[0].landmarks.Count <= 0)
                            return; // Exit if no landmark is present

                        inputArray.Add(midpoint.handLandmarks[0].landmarks[j].x);
                        inputArray.Add(midpoint.handLandmarks[0].landmarks[j].y);
                    }
                }
            }
            else if (bufferedResults.Count >= 60)
            {
                var lastFrames = bufferedResults.GetRange(bufferedResults.Count - Config.NUM_INPUT_FRAMES, Config.NUM_INPUT_FRAMES);
                foreach (var landmark in lastFrames)
                {
                    for (int j = 0; j < Config.NUM_INPUT_POINTS; j++)
                    {
                        if (landmark.handLandmarks == null || landmark.handLandmarks.Count <= 0 || landmark.handLandmarks[0].landmarks.Count <= 0)
                            return; // Exit if no landmark is present

                        inputArray.Add(landmark.handLandmarks[0].landmarks[j].x);
                        inputArray.Add(landmark.handLandmarks[0].landmarks[j].y);
                    }
                }
            }

            if (inputArray.Count > 0)
            {
                bool updated = true;

                Debug.Log("Fingerspell: Running");
                recognizer.RunModel(inputArray.ToArray());

            }
            buffer.Clear();
        });
        recognizer.AddCallback("default", (translation) => {
            lastTranslation = translation;
            Debug.Log(translation);
        });
        
        camera.AddCallback("default", (image => posePredictor.Single(image, (int) (Time.time * 1000))));

    }

    void TriggerOnClick()
    {

    }


    private static float[] convertLandmarkToFloatArray(NormalizedLandmarkList landmarks, ClassificationList handedness)
    {
        float[] currentFrame = new float[42];
        for (int i = 0; i < landmarks.Landmark.Count; i++)
        {
            if (handedness.Classification[0].Label.Contains("Left"))
            {
                currentFrame[i * 2] = flip(landmarks.Landmark[i].Y);
            }
            else
            {
                currentFrame[i * 2] = landmarks.Landmark[i].Y;
            }
            currentFrame[i * 2 + 1] = flip(landmarks.Landmark[i].X);
            //We flip x and y for iOS
#if UNITY_IOS
            currentFrame[i * 2] = flip(currentFrame[i * 2]);
            currentFrame[i * 2 + 1] = flip(currentFrame[i * 2 + 1]);
#endif
        }
        return currentFrame;
    }


    private static float flip(float original)
    {
        return 1.0f - original;
    }


}
