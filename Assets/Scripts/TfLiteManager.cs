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

	public T RunModel(float[] data)
    {
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
        Debug.Log("model output: " + mapping[idx]);
		return mapping[idx];
	}
}