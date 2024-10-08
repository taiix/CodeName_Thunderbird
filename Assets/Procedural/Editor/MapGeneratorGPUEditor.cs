using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGeneratorGPU))]
public class MapGeneratorGPUEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGeneratorGPU map = target as MapGeneratorGPU;

        if (DrawDefaultInspector())
        {
            //map.GenerateRegions();  //Delete
            map.Calculate();

        }

        if (GUILayout.Button("Generate"))
        {
            //map.GenerateRegions();  //Delete
            map.Calculate();
        }
    }
}
