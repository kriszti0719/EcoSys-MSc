using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

public abstract class Spawner : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] protected float minHeight;
    [SerializeField] protected float maxHeight;
    [SerializeField] protected Vector2 xRange;
    [SerializeField] protected Vector2 zRange;

    [Header("Prefab Variation Settings")]
    [SerializeField] protected Vector2 rotationRange;
    [SerializeField] protected Vector3 minScale;
    [SerializeField] protected Vector3 maxScale;

    //Autoupdate whenever we change one of the values
    [Space]
    [SerializeField] public bool autoUpdate;

    [Header("Prefabs")]
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int amount;
    public abstract void Generate();

    public void Clear()
    {
        while(transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    protected int Counter(string name)
    {
        Transform[] children = this.GetComponentsInChildren<Transform>(true);
        int counter = 0;
        foreach (Transform child in children)
        {
            if (child == this.transform)
                continue;
            if (child.name.Contains(name))
                counter++;
        }
        return counter;
    }
}

