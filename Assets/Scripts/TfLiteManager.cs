using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using TensorFlowLite;
using System.IO;
using UnityEngine.Networking;

public class SLRTfLiteModel<T> : TfLiteModelManager<T>
{
    float[,,,] modelInputTensor;
    NativeArray<float> modelOutputTensor;

    public SLRTfLiteModel(TextAsset model, T[] mapping) {
        this.model = model;
        GetInterpreter();
        modelInputTensor = new float[1, maxFrames, inputSize, 1];
        modelOutputTensor = new NativeArray<float>(outputSize, Allocator.Persistent);
        interpreter.AllocateTensors();
        this.mapping = mapping;
    }

	private T RunModel(float[] data)
    {
		// if (allData.Count < maxFrames)
        // {
		// 	var middleData = allData[allData.Count / 2];
		// 	int middleDataIndex = allData.Count / 2;
		// 	int framesToAdd = maxFrames - allData.Count;
		// 	for (int i = 0; i < framesToAdd; i++) {
		// 		allData.Insert(middleDataIndex, middleData);
		// 	}
        // }

		// for(int frameNumber = 0; frameNumber < maxFrames; frameNumber++)
        // {
		// 	for(int mediapipevalue = 0; mediapipevalue < inputSize; mediapipevalue++)
        //     {
		// 		data[0, frameNumber, mediapipevalue, 0] = allData[frameNumber][mediapipevalue];
        //     }
        // }

		interpreter.SetInputTensorData(0, data);
		interpreter.Invoke();
		interpreter.GetOutputTensorData(0, modelOutputTensor.AsSpan());

        float[] outputs = modelOutputTensor.ToArray();

		//TODO: figure out filtering
		float max = 0f;
		int idx = 0;
		
		for (int i = 0; i < outputSize; i++)
		{
			if (outputs[i] > max)
			{
				max = outputs[i];
				idx = i;
			}
		}
        Debug.Log(mapping[idx]);
		return mapping[idx];
	}
}