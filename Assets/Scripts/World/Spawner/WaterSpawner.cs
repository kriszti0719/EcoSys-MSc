using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaterSpawner : MonoBehaviour
{
     float minHeight = 23f;
     float maxHeight = 23.5f;
    public float minDistance = 5f;
    Vector2 xRange = new Vector2(-1205, 1205);
    Vector2 zRange = new Vector2(-1205, 1205);
    Vector2 rotationRange = new Vector2(0, 360);

    //Autoupdate whenever we change one of the values
    [Space]
    [SerializeField] public bool autoUpdate;
    [SerializeField] protected int amountEstimation = 1000;
    public void Clear()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void Generate()
    {
        Clear();
        SpawnWater(amountEstimation);
    }

    private List<Vector3> spawnedPositions = new List<Vector3>();

    private void SpawnWater(int amount)
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
            Vector3 spawnPosition = hit.point;
            if (IsTooClose(spawnPosition, minDistance))
                continue;
            GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeObject.transform.position = spawnPosition;
            if (cubeObject.GetComponent<Collider>() == null)
            {
                cubeObject.AddComponent<BoxCollider>();
            }
            cubeObject.layer = LayerMask.NameToLayer("Drink");
            cubeObject.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);
            cubeObject.transform.localScale = new Vector3(3, 3, 3);
            cubeObject.transform.parent = transform;
            //cubeObject.GetComponent<Renderer>().enabled = false;
            spawnedPositions.Add(spawnPosition);
            amount--;
        }
    }
    bool IsTooClose(Vector3 position, float minDistance)
    {
        foreach (var spawnedPos in spawnedPositions)
        {
            if (Vector3.Distance(position, spawnedPos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

}

