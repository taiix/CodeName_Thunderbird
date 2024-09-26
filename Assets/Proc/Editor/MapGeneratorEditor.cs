using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;

        if (DrawDefaultInspector())
        {
            //map.GenerateRegions();  //Delete
            map.CalculatePerlin();

        }

        if (GUILayout.Button("Generate"))
        {
            //map.GenerateRegions();  //Delete
            map.CalculatePerlin();
        }
    }
}
