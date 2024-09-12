using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TensorFlowLite;

public abstract class TfLiteModelManager<T>
{
    protected Interpreter interpreter;
    protected int numThreads = 1;

	protected int maxFrames;
	protected int inputSize;
	protected int outputSize;

	protected TextAsset model;

	protected T[] mapping;

    protected Interpreter GetInterpreter() {
        if (interpreter == null) {
			interpreter = new Interpreter(model.bytes, new InterpreterOptions()
			{
				threads = numThreads,
			});
            
			maxFrames  = interpreter. GetInputTensorInfo(0).shape[1];
			inputSize  = interpreter. GetInputTensorInfo(0).shape[2];
			outputSize = interpreter.GetOutputTensorInfo(0).shape[1];
		}
		return interpreter;
    }
    // since data can be dynamic - have multiple tensors etc - we don't have a Run Model function
    // as that would require different signatures.
}