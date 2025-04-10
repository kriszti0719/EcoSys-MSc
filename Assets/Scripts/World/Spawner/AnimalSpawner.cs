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

    private int BunnyCntr;
    private int FoxCntr;

    // Start is called before the first frame update
    public override void Generate()
    {
        Clear();
        BunnyCntr = 0;
        FoxCntr = 0;
        GenerateAnimal animal = new GenerateAnimal(null, null, Species.FOX, animalSizeMin, animalSizeMax);
        SpawnAnimals(animal, amount);
        animal = new GenerateAnimal(null, null, Species.BUNNY, animalSizeMin2, animalSizeMax2);
        SpawnAnimals(animal, amount2);
    }
    protected virtual void Start()
    {
        // A projekt gyökérmappájának meghatározása
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string dataDirectory = Path.Combine(projectRoot, "Data");

        Directory.CreateDirectory(dataDirectory);
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePathDeathData = Path.Combine(dataDirectory, $"{timestamp}_DeathData.csv");

        using (StreamWriter writer = new StreamWriter(filePathDeathData))
        {
            writer.WriteLine("Step;Species;DeathCause;Age;Speed;Sight;ReproductiveUrge;LifeSpan;Charm;PregnancyDuration;Status;Starving;Drying");
        }

        filePathPopulation = Path.Combine(dataDirectory, $"{timestamp}_PopulationData.csv");

        if (!File.Exists(filePathPopulation))
        {
            using (StreamWriter writer = new StreamWriter(filePathPopulation))
            {
                writer.WriteLine("Time;BunnyPop;FoxPop");
            }
        }
        FoxCntr = amount;
        BunnyCntr = amount2;
        StartCoroutine(RegisterPopulation());
    }
    private void SpawnAnimals(GenerateAnimal animal, int amount)
    {
        int cnt = amount;
        while (cnt > 0)
        {
            GameObject prefabToInstantiate = animal.species switch
            {
                Species.FOX => prefab,
                Species.BUNNY => prefab2,
                _ => throw new System.ArgumentException($"Unhandled species: {animal.species}")
            };
            GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(prefabToInstantiate, transform);
            placeAnimal(instantiatedPrefab, animal);


            // Attach scripts:
            Drink instantiatedDrink = instantiatedPrefab.AddComponent<Drink>();
            Eat instantiatedEat = instantiatedPrefab.AddComponent<Eat>();
            Rest instantiatedRest = instantiatedPrefab.AddComponent<Rest>();
            Mate instantiatedMate = instantiatedPrefab.AddComponent<Mate>();
            Reproduction instantiatedReproduction = instantiatedPrefab.AddComponent<Reproduction>();
            Movement instantiatedMovement = instantiatedPrefab.AddComponent<Movement>();
            Gravity instantiatedGravity = instantiatedPrefab.AddComponent<Gravity>();
            Sensor instantiatedSensor = instantiatedPrefab.AddComponent<Sensor>();
            Die instantiatedDie = instantiatedPrefab.AddComponent<Die>();
            Age instantiatedAge = instantiatedPrefab.AddComponent<Age>();



            switch (animal.species)
            {
                case Species.BUNNY:
                    BunnyCntr++;
                    instantiatedPrefab.name = $"{animal.species}_{BunnyCntr}";
                    instantiatedPrefab.layer = LayerMask.NameToLayer("Bunny");
                    Bunny instantiatedBunny = instantiatedPrefab.AddComponent<Bunny>();

                    isMale(instantiatedBunny, instantiatedPrefab);
                    changeBunnyColor(instantiatedBunny.isMale, instantiatedPrefab);

                    if (animal.mother != null)
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
                    FoxCntr++;
                    instantiatedPrefab.name = $"{animal.species}_{FoxCntr}";
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
    public void SpawnBabies(GameObject _gameObjprefab, int _amount, Animal _mother, MateTraits _father)
    {
        GenerateAnimal animal = new GenerateAnimal(_mother, _father, _mother.getSpecies());
        SpawnAnimals(animal, _amount);
    }
    private void isMale(Animal instantiatedAnimal, GameObject instantiatedPrefab)
    {
        int randomValue = UnityEngine.Random.Range(0, 2);
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
        if (animal.species == Species.BUNNY)
        {
            bottom = 8;
        }
        else if (animal.species == Species.FOX)
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
        if (animal.mother == null)
        {
            if (animal.species == Species.BUNNY)
            {
                capsuleCollider.height = (float)(2.5 / currentAnimalSize);
                capsuleCollider.radius = (float)(2.5 / currentAnimalSize);
            }
            else if (animal.species == Species.FOX)
            {
                capsuleCollider.height = (float)(8 / currentAnimalSize);
                capsuleCollider.radius = (float)(8 / currentAnimalSize);
            }
        }
        else
        {
            if (animal.species == Species.BUNNY)
            {
                capsuleCollider.height = (float)(2.5 / animal.mother.transform.localScale.x);
                capsuleCollider.radius = (float)(2.5 / animal.mother.transform.localScale.x);
            }
            else if (animal.species == Species.FOX)
            {
                capsuleCollider.height = (float)(8 / animal.mother.transform.localScale.x);
                capsuleCollider.radius = (float)(8 / animal.mother.transform.localScale.x);
            }
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
            int counterBunny = Counter("BUNNY");
            int counterFox = Counter("FOX");

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
            //string dataLine = $"{step};{animal.species.ToPrint()};{animal.cause.ToPrint()};{animal.aging.currentAge};{animal.movement.moveSpeed};{animal.sensor.radius};{animal.reproduction.reproductiveUrge};{animal.aging.lifeSpan};{animal.mating.charm};{animal.reproduction.pregnancyDuration};{animal.prevStatus};{animal.eat.starving};{animal.drink.drying}";
            //writer.WriteLine(dataLine); // Pontosvesszõ használata
        }
    }
}