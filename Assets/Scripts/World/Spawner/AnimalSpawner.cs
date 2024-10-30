using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Animals.Common.Behaviour;

public class AnimalSpawner : Spawner
{
    public GameObject staminaCanvas;
    public GameObject hungerCanvas;
    public GameObject thirstCanvas;
    public GameObject matingCanvas;
    public Transform mainCamera;

    public Material maleColor;
    public Material femaleColor;

    private float animalSize;

    protected int step = 0;


    // Start is called before the first frame update
    public override void Generate()
    {
        Clear();
        SpawnAnimals(prefab, amount, null, null);
    }
    protected virtual void Start()
    {
        StartCoroutine(RegisterPopulation());
    }
    public void SpawnAnimals(GameObject gameObjprefab, int amount, Animal mother, Animal father)
    {
        int cnt = amount;
        while(cnt > 0)
        {
            GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(this.prefab, transform);
            placeAnimal(instantiatedPrefab, mother, father);
            instantiatedPrefab.layer = LayerMask.NameToLayer("Bunny");

            setCollider(instantiatedPrefab);

            // Attach scripts:
            Movement instantiatedMovement = instantiatedPrefab.AddComponent<Movement>();
            Gravity instantiatedGravity = instantiatedPrefab.AddComponent<Gravity>();
            Bunny instantiatedBunny = instantiatedPrefab.AddComponent<Bunny>();
            Sensor instantiatedSensor = instantiatedPrefab.AddComponent<Sensor>();
            Reproduction instantiatedReproduction = instantiatedPrefab.AddComponent<Reproduction>();
            Rest instantiatedRest = instantiatedPrefab.AddComponent<Rest>();
            Drink instantiatedDrink = instantiatedPrefab.AddComponent<Drink>();
            Eat instantiatedEat = instantiatedPrefab.AddComponent<Eat>();
            Die instantiatedDie = instantiatedPrefab.AddComponent<Die>();
            Age instantiatedAge = instantiatedPrefab.AddComponent<Age>();
            Mate instantiatedMate = instantiatedPrefab.AddComponent<Mate>();

            //Boy OR Girl?
            isMale(instantiatedBunny, instantiatedPrefab);

            if(mother != null)
            {
                instantiatedBunny.SetTraits(mother, father);
                instantiatedMovement.ChangeDirection(amount, cnt);
            }
            else
            {
                instantiatedBunny.SetTraits(animalSize);
            }

            // Create a BarsContainer as a child of instantiatedPrefab
            setBars(instantiatedBunny, instantiatedPrefab);

            cnt--;
        }
    }
    private void isMale(Bunny instantiatedBunny, GameObject instantiatedPrefab)
    {
        int randomValue = Random.Range(0, 2);
        if (randomValue == 0)
        {
            instantiatedBunny.isMale = false;      //TODO: ezt bevinni
            instantiatedBunny.SetAnimalData(prefab, femaleColor);
        }
        else
        {
            instantiatedBunny.isMale = true;
            instantiatedBunny.SetAnimalData(prefab, maleColor);
        }

        //Change color
        Transform[] children = instantiatedPrefab.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child == instantiatedPrefab.transform)
                continue;
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (!(child.name.Contains("Nose")) && !(child.name.Contains("Eyes")))
                {
                    if (randomValue == 0)    // TODO: ne rnd, hanem fele-fele legyen
                    {
                        renderer.material = femaleColor;
                    }
                    else
                    {
                        renderer.material = maleColor;
                    }
                }
            }
        }
    }
    private void setBars(Bunny instantiatedBunny, GameObject instantiatedPrefab)
    {
        GameObject barsContainer = new GameObject("BarsContainer");
        barsContainer.transform.SetParent(instantiatedPrefab.transform);
        instantiatedBunny.barsContainer = barsContainer;

        GameObject instantiatedStamina = (GameObject)PrefabUtility.InstantiatePrefab(this.staminaCanvas, barsContainer.transform);
        instantiatedStamina.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + 20, instantiatedPrefab.transform.position.z);
        instantiatedStamina.GetComponent<Billboard>().cam = mainCamera;

        GameObject instantiatedHunger = (GameObject)PrefabUtility.InstantiatePrefab(this.hungerCanvas, barsContainer.transform);
        instantiatedHunger.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + 16, instantiatedPrefab.transform.position.z);
        instantiatedHunger.GetComponent<Billboard>().cam = mainCamera;

        GameObject instantiatedThirst = (GameObject)PrefabUtility.InstantiatePrefab(this.thirstCanvas, barsContainer.transform);
        instantiatedThirst.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + 12, instantiatedPrefab.transform.position.z);
        instantiatedThirst.GetComponent<Billboard>().cam = mainCamera;

        GameObject instantiatedMating = (GameObject)PrefabUtility.InstantiatePrefab(this.matingCanvas, barsContainer.transform);
        instantiatedMating.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + 8, instantiatedPrefab.transform.position.z);
        instantiatedMating.GetComponent<Billboard>().cam = mainCamera;

        instantiatedBunny.SetBars(barsContainer);
    }
    private void setCollider(GameObject instantiatedPrefab)
    {
        CapsuleCollider capsuleCollider = instantiatedPrefab.AddComponent<CapsuleCollider>();
        capsuleCollider.height = 20;
        capsuleCollider.radius = 8;
    }
    private void placeAnimal(GameObject instantiatedPrefab, Animal mother, Animal father)
    {
        if (mother != null & father != null)
        {
            instantiatedPrefab.transform.position = mother.transform.position;
            instantiatedPrefab.transform.rotation = mother.transform.rotation;
            instantiatedPrefab.transform.localScale = mother.transform.localScale;
        }
        else
        {
            bool success = false;
            while (!success)
            {
                float sampleX = Random.Range(xRange.x, xRange.y);
                float sampleY = Random.Range(zRange.x, zRange.y);
                Vector3 rayStart = new Vector3(sampleX, maxHeight, sampleY);

                int groundLayerMask = LayerMask.GetMask("Island");

                if (!Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
                    continue;
                if (hit.point.y < minHeight)
                    continue;
                success = true;

                // Instantiate the prefab and set its position, rotation, and scale:
                instantiatedPrefab.transform.position = hit.point;
                instantiatedPrefab.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);
                animalSize = Random.Range(minScale.x, maxScale.x);
                instantiatedPrefab.transform.localScale = new Vector3(
                    animalSize,
                    animalSize,
                    animalSize
                );
            }
        }
    }
    IEnumerator RegisterPopulation()
    {
        while (true)
        {
            step++;
            Transform[] children = this.GetComponentsInChildren<Transform>(true);
            int counter = Counter("Bunny");

            string filePath = "c:\\Work\\EcosystemSimulation\\Data\\PopulationData.csv";

            // Ellenõrizd, hogy a fájl létezik-e, és ha nem, hozd létre
            if (!File.Exists(filePath))
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Time,Bunnies");
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
    public void RegisterDeath(Animal animal)
    {
        string filePath = "c:\\Work\\EcosystemSimulation\\Data\\DeathData.csv";

        // Ellenõrizd, hogy a fájl létezik-e, és ha nem, hozd létre
        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Step\tDeathCause\tAge\tSpeed\tSight\tReproductiveUrge\tLifeSpan\tCharm\tPregnancyDuration\tStatus\tStarving\tDrying");
            }
        }

        // Írás a fájlba
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            string dataLine = $"{step}\t{animal.cause.ToString()}\t{animal.aging.currentAge}\t{animal.movement.moveSpeed}\t{animal.sensor.radius}\t{animal.reproduction.reproductiveUrge}\t{animal.aging.lifeSpan}\t{animal.mating.Charm}\t{animal.reproduction.pregnancyDuration}\t{animal.prevStatus.ToString()}\t{animal.eat.starving}\t{animal.drink.drying}";
            writer.WriteLine(dataLine);
        }
    }
}