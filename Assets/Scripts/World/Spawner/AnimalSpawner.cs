using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using Assets.Scripts.Animals.Common.Behaviour;
using Assets.Scripts.Datatypes;

public class AnimalSpawner : Spawner
{
    [Header("Fox")]
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected int amount;
    [SerializeField] protected float animalSizeMin;
    [SerializeField] protected float animalSizeMax;
    [Header("Bunny")]
    [SerializeField] protected GameObject prefab2;
    [SerializeField] protected int amount2;
    [SerializeField] protected float animalSizeMin2;
    [SerializeField] protected float animalSizeMax2;

    [Header("Canvas")]
    public GameObject staminaCanvas;
    public GameObject hungerCanvas;
    public GameObject thirstCanvas;
    public GameObject matingCanvas;
    public Transform mainCamera;

    public Material maleColor;
    public Material femaleColor;

    private float currentAnimalSize;

    protected int step = 0;

    private string filePathDeathData;
    private string filePathPopulation;

    // Start is called before the first frame update
    public override void Generate()
    {
        Clear();
        GenerateAnimal animal = new GenerateAnimal(prefab, null, null, Species.FOX, animalSizeMin, animalSizeMax);
        SpawnAnimals(animal, amount);
        animal = new GenerateAnimal(prefab2, null, null, Species.BUNNY, animalSizeMin2, animalSizeMax2);
        SpawnAnimals(animal, amount2);
    }
    protected virtual void Start()
    {
        // Fájl elérési út generálása egyszer, a program futásának kezdetén
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePathDeathData = $"C:\\Work\\GitHub\\EcoSys_MSc\\Data\\{timestamp}_DeathData.csv";

        using (StreamWriter writer = new StreamWriter(filePathDeathData))
        {
            writer.WriteLine("Step;Species;DeathCause;Age;Speed;Sight;ReproductiveUrge;LifeSpan;Charm;PregnancyDuration;Status;Starving;Drying");
        }

        filePathPopulation = $"C:\\Work\\GitHub\\EcoSys_MSc\\Data\\{timestamp}_PopulationData.csv";

        if (!File.Exists(filePathPopulation))
        {
            using (StreamWriter writer = new StreamWriter(filePathPopulation))
            {
                writer.WriteLine("Time;BunnyPop;FoxPop");
            }
        }
        StartCoroutine(RegisterPopulation());
    }
    private void SpawnAnimals(GenerateAnimal animal, int amount)
    {
        int cnt = amount;
        while(cnt > 0)
        {
            GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(animal.prefab, transform);
            placeAnimal(instantiatedPrefab, animal);

            // Attach scripts:
            Movement instantiatedMovement = instantiatedPrefab.AddComponent<Movement>();
            Gravity instantiatedGravity = instantiatedPrefab.AddComponent<Gravity>();
            Sensor instantiatedSensor = instantiatedPrefab.AddComponent<Sensor>();
            Reproduction instantiatedReproduction = instantiatedPrefab.AddComponent<Reproduction>();
            Rest instantiatedRest = instantiatedPrefab.AddComponent<Rest>();
            Drink instantiatedDrink = instantiatedPrefab.AddComponent<Drink>();
            Eat instantiatedEat = instantiatedPrefab.AddComponent<Eat>();
            Die instantiatedDie = instantiatedPrefab.AddComponent<Die>();
            Age instantiatedAge = instantiatedPrefab.AddComponent<Age>();
            Mate instantiatedMate = instantiatedPrefab.AddComponent<Mate>();

            switch (animal.species)
            {
                case Species.BUNNY:
                    instantiatedPrefab.layer = LayerMask.NameToLayer("Bunny");
                    Bunny instantiatedBunny = instantiatedPrefab.AddComponent<Bunny>();

                    isMale(instantiatedBunny, instantiatedPrefab);
                    changeBunnyColor(instantiatedBunny.isMale, instantiatedPrefab);

                    if(animal.mother != null)
                    {
                        instantiatedBunny.SetTraits(animal.mother, animal.father);
                        instantiatedMovement.ChangeDirection(amount, cnt);
                    }
                    else
                    {
                        instantiatedBunny.SetTraits(currentAnimalSize);
                    }
                    setBars(instantiatedBunny, instantiatedPrefab, animal);
                    break;
                case Species.FOX:
                    instantiatedPrefab.layer = LayerMask.NameToLayer("Fox");
                    Fox instantiatedFox = instantiatedPrefab.AddComponent<Fox>();

                    isMale(instantiatedFox, instantiatedPrefab);
                    if (animal.mother != null)
                    {
                        instantiatedFox.SetTraits(animal.mother, animal.father);
                        instantiatedMovement.ChangeDirection(amount, cnt);
                    }
                    else
                    {
                        instantiatedFox.SetTraits(currentAnimalSize);
                    }
                    setBars(instantiatedFox, instantiatedPrefab, animal);
                    break;
            }

            setCollider(instantiatedPrefab, animal);

            cnt--;
        }
    }
    private void isMale(Animal instantiatedAnimal, GameObject instantiatedPrefab)
    {
        int randomValue = Random.Range(0, 2);
        if (randomValue == 0)
        {
            instantiatedAnimal.isMale = false;      //TODO: ezt bevinni
            instantiatedAnimal.SetAnimalData(prefab, femaleColor);
        }
        else
        {
            instantiatedAnimal.isMale = true;
            instantiatedAnimal.SetAnimalData(prefab, maleColor);
        }
    }
    private void changeBunnyColor(bool isMale, GameObject instantiatedPrefab)
    {
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
                    if (isMale == true)
                    {
                        renderer.material = maleColor;
                    }
                    else
                    {
                        renderer.material = femaleColor;
                    }
                }
            }
        }
    }
    private void setBars(Animal instantiatedAnimal, GameObject instantiatedPrefab, GenerateAnimal animal)
    {
        int bottom = 0;
        if(animal.species == Species.BUNNY)
        {
            bottom = 8;
        }
        else if(animal.species == Species.FOX)
        {
            bottom = 16;
        }

        GameObject barsContainer = new GameObject("BarsContainer");
        barsContainer.transform.SetParent(instantiatedPrefab.transform);
        instantiatedAnimal.barsContainer = barsContainer;
        barsContainer.transform.rotation = instantiatedAnimal.transform.rotation;

        GameObject instantiatedStamina = (GameObject)PrefabUtility.InstantiatePrefab(this.staminaCanvas, barsContainer.transform);
        instantiatedStamina.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + bottom + 12, instantiatedPrefab.transform.position.z);
        instantiatedStamina.GetComponent<Billboard>().cam = mainCamera;

        GameObject instantiatedHunger = (GameObject)PrefabUtility.InstantiatePrefab(this.hungerCanvas, barsContainer.transform);
        instantiatedHunger.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + bottom + 8, instantiatedPrefab.transform.position.z);
        instantiatedHunger.GetComponent<Billboard>().cam = mainCamera;

        GameObject instantiatedThirst = (GameObject)PrefabUtility.InstantiatePrefab(this.thirstCanvas, barsContainer.transform);
        instantiatedThirst.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + bottom + 4, instantiatedPrefab.transform.position.z);
        instantiatedThirst.GetComponent<Billboard>().cam = mainCamera;

        GameObject instantiatedMating = (GameObject)PrefabUtility.InstantiatePrefab(this.matingCanvas, barsContainer.transform);
        instantiatedMating.transform.position = new Vector3(instantiatedPrefab.transform.position.x, instantiatedPrefab.transform.position.y + bottom, instantiatedPrefab.transform.position.z);
        instantiatedMating.GetComponent<Billboard>().cam = mainCamera;

        instantiatedAnimal.SetBars(barsContainer);
    }
    private void setCollider(GameObject instantiatedPrefab, GenerateAnimal animal)
    {
        CapsuleCollider capsuleCollider = instantiatedPrefab.AddComponent<CapsuleCollider>();
        if (animal.species == Species.BUNNY)
        {
            capsuleCollider.height = (float)(2.5 /currentAnimalSize);
            capsuleCollider.radius = (float)(2.5 / currentAnimalSize);
        }
        else if (animal.species == Species.FOX)
        {
            capsuleCollider.height = (float)(8 / currentAnimalSize);
            capsuleCollider.radius = (float)(8 / currentAnimalSize);
        }
    }
    private void placeAnimal(GameObject instantiatedPrefab, GenerateAnimal animal)
    {
        if (animal.mother != null & animal.father != null)
        {
            instantiatedPrefab.transform.position = animal.mother.transform.position;
            instantiatedPrefab.transform.rotation = animal.mother.transform.rotation;
            instantiatedPrefab.transform.localScale = animal.mother.transform.localScale;
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
                currentAnimalSize = Random.Range(animal.sizeMin, animal.sizeMax);
                instantiatedPrefab.transform.localScale = new Vector3(
                    currentAnimalSize,
                    currentAnimalSize,
                    currentAnimalSize
                );
                instantiatedPrefab.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                instantiatedPrefab.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y));
            }
        }
    }
    IEnumerator RegisterPopulation()
    {
        while (true)
        {
            step++;
            Transform[] children = this.GetComponentsInChildren<Transform>(true);
            int counterBunny = Counter("Bunny");
            int counterFox = Counter("Fox");

            // Írás a fájlba
            using (StreamWriter writer = new StreamWriter(filePathPopulation, true))
            {
                writer.WriteLine($"{step};{counterBunny};{counterFox}");
            }
            yield return new WaitForSeconds(10f);
        }
    }
    public void RegisterDeath(Animal animal)
    {
        // Adatok hozzáfûzése a fájlhoz
        using (StreamWriter writer = new StreamWriter(filePathDeathData, true))
        {
            string dataLine = $"{step};{animal.species.ToPrint()};{animal.cause.ToPrint()};{animal.aging.currentAge};{animal.movement.moveSpeed};{animal.sensor.radius};{animal.reproduction.reproductiveUrge};{animal.aging.lifeSpan};{animal.mating.Charm};{animal.reproduction.pregnancyDuration};{animal.prevStatus};{animal.eat.starving};{animal.drink.drying}";
            writer.WriteLine(dataLine); // Pontosvesszõ használata
        }
    }
}