using Assets.Scripts.Animals.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimalSpawner))]
public class AimalSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AnimalSpawner spa = (AnimalSpawner)target;

        if (DrawDefaultInspector())
        {
            if (spa.autoUpdate)
            {
                spa.Generate();
            }
        }
        //If that button is pressed:   3568
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
