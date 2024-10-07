using System.Diagnostics;
using UnityEngine;

public class ComputeGPU : MonoBehaviour
{
    public ComputeShader computeShader;

    public int vectorSize = 1000000;
    private ComputeBuffer vectorAbuffer, vectorBbuffer, vectorResultBuffer;
    private float[] resultVector;

    void Start()
    {
        resultVector = new float[vectorSize];

        vectorAbuffer = new ComputeBuffer(vectorSize, sizeof(float));
        vectorBbuffer = new ComputeBuffer(vectorSize, sizeof(float));
        vectorResultBuffer = new ComputeBuffer(vectorSize, sizeof(float));

        int kernelHandler = computeShader.FindKernel("CSMain");

        computeShader.SetBuffer(kernelHandler, "VectorA", vectorAbuffer);
        computeShader.SetBuffer(kernelHandler, "VectorB", vectorBbuffer);
        computeShader.SetBuffer(kernelHandler, "ResultVector", vectorResultBuffer);

        Stopwatch sw = new Stopwatch();
        sw.Start();

        computeShader.Dispatch(kernelHandler, 256, 1, 1);

        sw.Stop();
        UnityEngine.Debug.Log("GPU Time: " + sw.ElapsedMilliseconds + " ms");

        // Retrieve the result from the GPU
        vectorResultBuffer.GetData(resultVector);

        // Release buffers
        vectorAbuffer.Release();
        vectorBbuffer.Release();
        vectorResultBuffer.Release();
    }
}
