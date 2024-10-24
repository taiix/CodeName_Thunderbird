using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Clouds3D))]
public class CloudsEditor : Editor
{
    public override void OnInspectorGUI()
    {


        Clouds3D clouds = (Clouds3D)target;

        if (GUILayout.Button("Generate Texture"))
        {
            clouds.GenerateTexture();
        }

        if (DrawDefaultInspector())
        {
            clouds.GenerateTexture();
        }
    }
}
