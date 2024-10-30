using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FoodSpawner))]
public class FoodSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FoodSpawner spa = (FoodSpawner)target;

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