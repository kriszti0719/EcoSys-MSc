using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaterSpawner))]
public class WaterSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WaterSpawner spa = (WaterSpawner)target;
        if (DrawDefaultInspector())
        {
            if (spa.autoUpdate)
            {
                spa.Generate();
            }
        }
        //If that button is pressed:
        if (GUILayout.Button("Generate"))
        {
            spa.Generate();
        }
        else if (GUILayout.Button("Clear"))
        {
            spa.Clear();
        }
    }
}
