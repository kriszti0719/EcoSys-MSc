using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ForestSpawner))]
public class ForestSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ForestSpawner spa = (ForestSpawner)target;

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

