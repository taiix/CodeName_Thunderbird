using System.Diagnostics;
using UnityEngine;

public class Compute : MonoBehaviour
{
    public int vectorSize = 1000000;
    private float[] vectorA, vectorB, resultVector;

    // Start is called before the first frame update
    void Start()
    {
        vectorA = new float[vectorSize];   
        vectorB = new float[vectorSize];      
        resultVector = new float[vectorSize];

        FillVector(vectorA);
        FillVector(vectorB);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int i = 0; i < vectorSize; i++) { 
            resultVector[i] = vectorA[i] / vectorB[i];
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log("CPU Time: " + stopwatch.ElapsedMilliseconds + " ms");
    }

    void FillVector(float[] v) {
        for (int i = 0; i < vectorSize; i++) {
            v[i] = Random.Range(0, 1.0f);
        }
    }
}
