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

    [Header("Prefab 5")]
    [SerializeField] protected GameObject prefab5;
    [SerializeField] protected int amount5;

    [Header("Prefab 6")]
    [SerializeField] protected GameObject prefab6;
    [SerializeField] protected int amount6;

    public override void Generate()
    {
        DebugLogger.Error("Null reference during eating, FOX[ID=87]");
        DebugLogger.Warning("Over 90% of rabbits are hungry");
        DebugLogger.Notice("Bunny Population dropped below 50");
        DebugLogger.Info("153 bunnies are active");
        Clear();
        SpawnBush(prefab, amount);
        SpawnBush(prefab2, amount2);
        SpawnBunnyFood(prefab3, amount3);
        SpawnBunnyFood(prefab4, amount4);
        SpawnBunnyFood(prefab5, amount5);
        SpawnBunnyFood(prefab6, amount6);
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
            int counter = Counter("HugeBunch");
            if(counter == 1)
            {
                SpawnBunnyFood(prefab3, 1);
            }
            SpawnBunnyFood(prefab3, Mathf.RoundToInt((float)counter / 100f));
            counter = Counter("MediumBunch");
            if (counter == 1)
            {
                SpawnBunnyFood(prefab4, 1);
            }
            SpawnBunnyFood(prefab4, Mathf.RoundToInt((float)counter / 100f));
            counter = Counter("MiniBunch");
            if (counter == 1)
            {
                SpawnBunnyFood(prefab5, 1);
            }
            SpawnBunnyFood(prefab5, Mathf.RoundToInt((float)counter / 100f));
            counter = Counter("SmallBunch");
            if (counter == 1)
            {
                SpawnBunnyFood(prefab6, 1);
            }
            SpawnBunnyFood(prefab6, Mathf.RoundToInt((float)counter / 100f));
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
    public void SpawnBush(GameObject prefab, int amount)
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
            //TODO: instantiatedPrefab.layer = XY
            //CapsuleCollider capsuleCollider = instantiatedPrefab.AddComponent<CapsuleCollider>();
            instantiatedPrefab.layer = LayerMask.NameToLayer("UI");
            instantiatedPrefab.transform.localScale = new Vector3(
                Random.Range(minScale.x, maxScale.x),
                Random.Range(minScale.y, maxScale.y),
                Random.Range(minScale.z, maxScale.z)
            );
            amount--;
        }
    }
    IEnumerator RegisterPopulation()
    {
        int step = 0;
        while (true)
        {
            step++;
            Transform[] children = this.GetComponentsInChildren<Transform>(true);
            int counter = Counter("Bunch");

            string filePath = "C:\\Work\\GitHub\\EcoSys-MSc\\FoodPopulationData.csv";

            // Ellenõrizd, hogy a fájl létezik-e, és ha nem, hozd létre
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Time,Food");
                }
            }

            // Írás a fájlba
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine($"{step},{counter}");
            }
            yield return new WaitForSeconds(10f);
        }
    }
}

