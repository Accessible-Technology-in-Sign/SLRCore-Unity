using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using TensorFlowLite;
using System.IO;
using Common;
using UnityEngine.Networking;

namespace Model {
	public abstract class TfLiteModelManager<T> {
		protected Interpreter interpreter;
		protected int numThreads = 1;

		protected int maxFrames;
		protected int inputSize;
		protected int outputSize;

		protected TextAsset model;

		protected List<T> mapping;

		protected Interpreter GetInterpreter() {
			if (interpreter == null) {
				interpreter = new Interpreter(model.bytes, new InterpreterOptions()
				{
					threads = numThreads,
				});

				maxFrames = interpreter.GetInputTensorInfo(0).shape[1];
				inputSize = interpreter.GetInputTensorInfo(0).shape[2];
				outputSize = interpreter.GetOutputTensorInfo(0).shape[1];
			}

			return interpreter;
		}
		// since data can be dynamic - have multiple tensors etc - we don't have a Run Model function
		// as that would require different signatures.
	}
	public class SLRTfLiteModel<T> : TfLiteModelManager<T> {
		float[,,,] modelInputTensor;
		NativeArray<float> modelOutputTensor;
		
		private Dictionary<string, Action<T>> callbacks = new();
		public List<PredictionFilter<T>> outputFilters = new();

		public SLRTfLiteModel(TextAsset model, List<T> mapping) {
			this.model = model;
			GetInterpreter();
			modelInputTensor = new float[1, maxFrames, inputSize, 1];
			modelOutputTensor = new NativeArray<float>(outputSize, Allocator.Persistent);
			interpreter.AllocateTensors();
			this.mapping = mapping;
			outputFilters.Add(new PassThroughFilterSingle<T>());
		}

		public void RunModel(float[] data) {
			interpreter.SetInputTensorData(0, data);
			interpreter.Invoke();
			interpreter.GetOutputTensorData(0, modelOutputTensor.AsSpan());

			float[] outputs = modelOutputTensor.ToArray();

			FilterUnit<T> output = new FilterUnit<T>(mapping, outputs);

			foreach (var filter in outputFilters)
			{
				output = filter.Filter(output);
			}

			foreach (var callback in callbacks) {
				if (output.mapping.Count == 1) {
					callbacks[callback.Key].Invoke(output.mapping[0]);
				}
				else if (output.mapping.Count > 1) {
					callbacks[callback.Key].Invoke(output.mapping[output.probabilities.ArgMax()]);
				}
			}
		}
		
		public void AddCallback(string name, Action<T> callback) {
			if (callbacks.ContainsKey(name)) callbacks.Remove(name);
			callbacks.Add(name, callback);
		}

		public void RemoveCallback(string name) {
			callbacks.Remove(name);
		}
	}
}