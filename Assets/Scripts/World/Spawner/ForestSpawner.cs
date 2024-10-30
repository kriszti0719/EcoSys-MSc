using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ForestSpawner : Spawner
{
    [Header("Prefab 2")]
    [SerializeField] protected GameObject prefab2;
    [SerializeField] protected int amount2;

    [Header("Prefab 3")]
    [SerializeField] protected GameObject prefab3;
    [SerializeField] protected int amount3;

    [Header("Prefab 4")]
    [SerializeField] protected GameObject prefab4;
    [SerializeField] protected int amount4;

    [Header("Prefab 5")]
    [SerializeField] protected GameObject prefab5;
    [SerializeField] protected int amount5;

    [Header("Prefab 6")]
    [SerializeField] protected GameObject prefab6;
    [SerializeField] protected int amount6;
    
    [Header("Prefab 7")]
    [SerializeField] protected GameObject prefab7;
    [SerializeField] protected int amount7;

    [Header("Prefab 8")]
    [SerializeField] protected GameObject prefab8;
    [SerializeField] protected int amount8;

    [Header("Prefab 9")]
    [SerializeField] protected GameObject prefab9;
    [SerializeField] protected int amount9;

    [Header("Prefab 10")]
    [SerializeField] protected GameObject prefab10;
    [SerializeField] protected int amount10;

    [Header("Prefab 11")]
    [SerializeField] protected GameObject prefab11;
    [SerializeField] protected int amount11;
    
    [Header("Prefab 12")]
    [SerializeField] protected GameObject prefab12;
    [SerializeField] protected int amount12;

    [Header("Prefab 13")]
    [SerializeField] protected GameObject prefab13;
    [SerializeField] protected int amount13;

    // Start is called before the first frame update
    public override void Generate()
    {
        Clear();
        SpawnTree(prefab, amount);
        SpawnTree(prefab2, amount2);
        SpawnTree(prefab3, amount3);
        SpawnTree(prefab4, amount4);
        SpawnTree(prefab5, amount5);
        SpawnTree(prefab6, amount6);
        SpawnTree(prefab7, amount7);
        SpawnTree(prefab8, amount8);
        SpawnTree(prefab9, amount9);
        SpawnTree(prefab10, amount10);
        SpawnTree(prefab11, amount11);
        SpawnTree(prefab12, amount12);
        SpawnTree(prefab13, amount13);
    }

    public void SpawnTree(GameObject prefab, int amount)
    {
        while (amount > 0)
        {
            float sampleX = Random.Range(xRange.x, xRange.y);
            float sampleY = Random.Range(zRange.x, zRange.y);
            Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);

            if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
                continue;
            if (hit.point.y < minHeight)
                continue;

            // Instantiate the prefab and set its position, rotation, and scale
            GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            instantiatedPrefab.transform.position = hit.point;
            instantiatedPrefab.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);
            instantiatedPrefab.layer = LayerMask.NameToLayer("UI");

            instantiatedPrefab.transform.localScale = new Vector3(
                Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                Random.Range(minScale.z, maxScale.z)
            );
            amount--;
        }
    }
}
