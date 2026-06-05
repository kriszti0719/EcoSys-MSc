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

        GUILayout.Space(5);

        if (GUILayout.Button("Generate"))
        {
            spa.Generate();
        }
        
        if (GUILayout.Button("Clear"))
        {
            spa.Clear();
        }

        GUILayout.Space(15);
        EditorGUILayout.LabelField("CSV InfluxDB Importer", EditorStyles.boldLabel);

        GUI.backgroundColor = new Color(0.2f, 0.6f, 1f); 
        
        if (GUILayout.Button("Upload CSV to InfluxDB", GUILayout.Height(30)))
        {
            InfluxLogger.Init(
                _url: "http://localhost:8086",
                _token: "KeWK_betKl_J8Aqgnd6Nh-D2UrUabSBsfjPR-pu_C8QA9UX6y6V3z_lZNMm3jIDCXSeE4aWW_KBGIIe4GTcyUA==",
                _org: "EcoSys",
                _bucket: "ecosys_csv",
                runner: null
            );
            spa.UploadCsvToInflux();
        }
        
        GUI.backgroundColor = Color.white;
    }
}