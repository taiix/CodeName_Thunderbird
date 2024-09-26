using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseAirplaneInputs))]
public class AirplaneInputEditor : Editor
{

    private BaseAirplaneInputs inputs;

    private void OnEnable()
    {
        inputs = (BaseAirplaneInputs)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        string debugInfo = "";
        debugInfo += "Pitch = " + inputs.Pitch + '\n';
        debugInfo += "Roll = " + inputs.Roll + '\n';
        debugInfo += "Yaw = " + inputs.Yaw + '\n';
        debugInfo += "Throttle = " + inputs.Throttle + '\n';
        debugInfo += "Break = " + inputs.Break + '\n';
        debugInfo += "Flaps = " + inputs.Flaps;

        GUILayout.Space(20);
        EditorGUILayout.TextArea(debugInfo);

        Repaint();
    }
}
