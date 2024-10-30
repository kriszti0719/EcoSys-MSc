using UnityEngine;
using System.Collections;
using UnityEditor;



[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Spawner spa = (Spawner)target;

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
