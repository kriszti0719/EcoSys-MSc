using Assets.Scripts.Animals.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static Unity.VisualScripting.Metadata;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class FoodSpawner : Spawner
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
    
    [Header("Carrying Capacities")]
    [SerializeField] private int maxHugeBunch = 200;
    [SerializeField] private int maxMediumBunch = 200;
    [SerializeField] private int maxMiniBunch = 200;
    [SerializeField] private int maxSmallBunch = 200;

    [Header("Regen Settings")]
    [SerializeField] private int fixedRegenAmount = 5;
    public override void Generate()
    {
        Clear();
        SpawnBunnyFood(prefab, amount);
        SpawnBunnyFood(prefab2, amount2);
        SpawnBunnyFood(prefab3, amount3);
        SpawnBunnyFood(prefab4, amount4);
    }
    public override void Clear()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    protected virtual void Start()
    {
        StartCoroutine(ReSpawn());
        StartCoroutine(RegisterPopulation());
    }
    IEnumerator ReSpawn()
    {
        while (true)
        {
            int currentHuge = Counter("HugeBunch");
            if (currentHuge < maxHugeBunch)
            {
                int toSpawn = Mathf.Clamp(Mathf.RoundToInt((maxHugeBunch - currentHuge) * 0.05f), 1, fixedRegenAmount);
                SpawnBunnyFood(prefab, toSpawn);
            }

            int currentMedium = Counter("MediumBunch");
            if (currentMedium < maxMediumBunch)
            {
                int toSpawn = Mathf.Clamp(Mathf.RoundToInt((maxMediumBunch - currentMedium) * 0.05f), 1, fixedRegenAmount);
                SpawnBunnyFood(prefab2, toSpawn);
            }

            int currentMini = Counter("MiniBunch");
            if (currentMini < maxMiniBunch)
            {
                int toSpawn = Mathf.Clamp(Mathf.RoundToInt((maxMiniBunch - currentMini) * 0.05f), 1, fixedRegenAmount);
                SpawnBunnyFood(prefab3, toSpawn);
            }

            int currentSmall = Counter("SmallBunch");
            if (currentSmall < maxSmallBunch)
            {
                int toSpawn = Mathf.Clamp(Mathf.RoundToInt((maxSmallBunch - currentSmall) * 0.05f), 1, fixedRegenAmount);
                SpawnBunnyFood(prefab4, toSpawn);
            }

            yield return new WaitForSeconds(3f);
        }
    }
    public void SpawnBunnyFood(GameObject prefab, int amount)
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
            instantiatedPrefab.layer = LayerMask.NameToLayer("BunnyFood");

            CapsuleCollider capsuleCollider = instantiatedPrefab.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 3;

            instantiatedPrefab.transform.localScale = new Vector3(
                Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                Random.Range(minScale.z, maxScale.z)
            );
            Plant instantiatedPlant = instantiatedPrefab.AddComponent<Plant>();
            amount--;
        }
    }
    IEnumerator RegisterPopulation()
    {
        int step = 0;
        DebugLogger.setLogPath();

        while (true)
        {
            step++;
            DebugLogger.RegisterFood(step: step, cnt: Counter("Bunch"));
            yield return new WaitForSeconds(10f);
        }
    }
}