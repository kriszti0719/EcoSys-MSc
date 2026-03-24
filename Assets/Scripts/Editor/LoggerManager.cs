using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AnimalSpawner))]
class LoggerManager : Editor
{
    public override void OnInspectorGUI()
    {
        AnimalSpawner ls = (AnimalSpawner)target;

        if (DrawDefaultInspector())
        {
            if (ls.autoUpdate)
            {
                ls.Generate();
            }
        }
        //If that button is pressed:
        if (GUILayout.Button("Generate"))
        {
            ls.Generate();
        }
        else if (GUILayout.Button("Clear"))
        {
            ls.Clear();
        }
    }
}