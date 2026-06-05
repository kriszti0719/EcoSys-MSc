using Assets.Scripts.Animals.Common.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    public Status status;
    public Status prevStatus;
    
    public GameObject targetRef;
    public List<GameObject> rejectedBy = new List<GameObject>();
    public List<GameObject> spottedThreats = new List<GameObject>();
    
    public Species species;
    [HideInInspector] public List<Species> predators = new List<Species>();

    [HideInInspector] public CauseOfDeath cause;
    [HideInInspector] public GameObject prefab;
    [HideInInspector] public Material color;
    [HideInInspector] public List<GameObject> destructibles = new List<GameObject>();

    public bool isMale;
    [HideInInspector] public int bravery;

    private int maxOxygen = 10;
    private int oxygen;
    // TODO: remove (only for testing purposes)
    [HideInInspector] public int triedForBaby = 0;
    [HideInInspector] public int gaveBirth = 0;
    [HideInInspector] public int kids = 0;
    [HideInInspector] public int generation;
    

    [HideInInspector] public Sensor sensor;
    [HideInInspector] public Reproduction reproduction;
    [HideInInspector] public Rest rest;
    [HideInInspector] public Drink drink;
    [HideInInspector] public Eat eat;
    [HideInInspector] public Die die;
    [HideInInspector] public Age aging;
    [HideInInspector] public Mate mating;
    [HideInInspector] public Movement movement;
    [HideInInspector] public GameObject barsContainer;
    [HideInInspector] public AnimalEventHandler animalEventHandler;
    
    [HideInInspector] public DecisionController aiController;
    [HideInInspector] public UtilityGenome utilityGenome;
    [HideInInspector] public FCMGenome fcmGenome;
    public event Action OnDrowned;
    public Species getSpecies()
    {
        return species;
    }
    public abstract int getTargetLayerToMate();
    public abstract int getTargetLayerToEat();
    public abstract int getPredatorLayers();
    public abstract void setTargetLayerToMate();
    public abstract void setTargetLayerToEat();
    public abstract void setSpeciesSpecificTraits();
    protected void SetComponents()
    {
        sensor = GetComponent<Sensor>();
        sensor.targetMask = LayerMask.GetMask("None");
        reproduction = GetComponent<Reproduction>();
        rest = GetComponent<Rest>();
        drink = GetComponent<Drink>();
        eat = GetComponent<Eat>();
        die = GetComponent<Die>();
        aging = GetComponent<Age>();
        mating = GetComponent<Mate>();
        movement = GetComponent<Movement>();
        movement.animal = this;
        animalEventHandler = GetComponent<AnimalEventHandler>();
    }
    public void SetTraits(float rnd)
    {
        SetComponents();
        setSpeciesSpecificTraits();

        generation = 1;
        float randomLifeSpan = UnityEngine.Random.Range(4f, 6f);
        aging.setAging(UnityEngine.Random.Range(1f, randomLifeSpan - 0.5f), rnd, randomLifeSpan);
        bravery = UnityEngine.Random.Range(20, 40);
        if(species == Species.BUNNY)
            movement.moveSpeed = UnityEngine.Random.Range(1f, 7f);
        else if(species == Species.FOX)
            movement.moveSpeed = UnityEngine.Random.Range(7f, 15f);
        movement.rotSpeed = movement.moveSpeed * 30;
        sensor.setSensor(
            _radius: UnityEngine.Random.Range(30, 70),
            _camouflage: UnityEngine.Random.Range(30, 60),
            _stealth: UnityEngine.Random.Range(20, 50)
        );

        reproduction.setReproduction(
            _pregnancyDuration: UnityEngine.Random.Range(3, 6)
        );
        mating.enableMating = true;
        mating.charm = UnityEngine.Random.Range(20, 100);
        drink.critical = UnityEngine.Random.Range(25, 35);
        eat.critical = UnityEngine.Random.Range(15, 25);
    }
    public void SetTraits(Animal mother, MateTraits father)
    {
        SetComponents();
        setSpeciesSpecificTraits();
        
        generation = mother.generation + 1;
        aging.setAging(
            _age: 0.01f,
            _size: (mother.aging.adultSize + father.size) / 2f,
            _lifeSpan: MutateTrait(mother.aging.lifeSpan, father.lifeSpan)
        );
        bravery = Mathf.RoundToInt(MutateTrait(mother.bravery, father.bravery));
        eat.critical = Mathf.RoundToInt(MutateTrait(mother.eat.critical, father.eat_critical));
        drink.critical = Mathf.RoundToInt(MutateTrait(mother.drink.critical, father.drink_critical));
        movement.moveSpeed = MutateTrait(mother.movement.moveSpeed, father.moveSpeed);
        movement.rotSpeed = movement.moveSpeed * 30;
        sensor.setSensor(
            _radius: Mathf.RoundToInt(MutateTrait(mother.sensor.radius, father.radius)),
            _camouflage: Mathf.RoundToInt(MutateTrait(mother.sensor.camouflage, father.camouflage)),
            _stealth: Mathf.RoundToInt(MutateTrait(mother.sensor.stealth, father.stealth))
        );
        reproduction.setReproduction(
            _pregnancyDuration: Mathf.RoundToInt(MutateTrait(mother.reproduction.pregnancyDuration, father.pregnancyDuration))
        );
        mating.enableMating = false;
        mating.charm = Mathf.RoundToInt(MutateTrait(mother.mating.charm, father.charm));
        
    }
    protected float MutateTrait(float motherTrait, float fatherTrait)
    {
        float averageTrait = (motherTrait + fatherTrait) / 2f;
        float mutationFactor = UnityEngine.Random.Range(0.8f, 1.2f);
        float improvementBias = 1.05f;
        float mutatedTrait = averageTrait * mutationFactor * improvementBias;
        mutatedTrait = Mathf.Max(mutatedTrait, 0.1f);

        return mutatedTrait;
    }
    public void SetBars(GameObject barsContainer, bool randomize = false)
    {
        rest.setBar(barsContainer, randomize);
        eat.setBar(barsContainer, randomize);
        drink.setBar(barsContainer, randomize);
        mating.setBar(barsContainer, randomize);
        destructibles.Add(barsContainer);
    }
    public void SetAnimalData(GameObject prefab, Material color)
    {
        this.prefab = prefab;
        this.color = color;
        this.rejectedBy.Add(this.gameObject);
        //TODO: set charm according to color
    }
    protected void Subscribe()
    {
        eat.OnHungerCritical += animalEventHandler.HandleHungerCritical;
        eat.OnHungerDepleted += animalEventHandler.HandleHungerDepleted;

        drink.OnThirstCritical += animalEventHandler.HandleThirstCritical;
        drink.OnThirstDepleted += animalEventHandler.HandleThirstDepleted;

        rest.OnRestDepleted += animalEventHandler.HandleRestDepleted;
        rest.OnBreakEnded += animalEventHandler.HandleBreakEnded;

        aging.OnAgeLimitReached += animalEventHandler.HandleAgeLimitReached;

        sensor.OnTargetSpotted += animalEventHandler.HandleTargetSpotted;

        OnDrowned += animalEventHandler.HandleDrowning;
    }
    
    private void InitBrain()
    {
        aiController = new DecisionController();

        if (DecisionController.GlobalMode == DecisionMode.Utility)
        {
            if (utilityGenome == null) utilityGenome = new UtilityGenome();
            aiController.utility = new UtilityDecision(utilityGenome);
        }
        else if (DecisionController.GlobalMode == DecisionMode.FCM)
        {
            if (fcmGenome == null) fcmGenome = new FCMGenome(FCMFactory.GetLinkCount());
            aiController.fcm = new FCMDecision(fcmGenome);
        }
        else
        {
            aiController.classic = new ClassicFSMDecision();
        }
    }
    
    protected virtual void Start()
    {
        InitBrain();
        Subscribe();
        cause = CauseOfDeath.NONE;
        status = Status.WANDER;
        oxygen = maxOxygen;
        movement.StartMoving();
    }
    public void Step()
    {
        oxygen = (transform.localPosition.y < 20 && targetRef == null) ? oxygen - 1 : maxOxygen;
        if (oxygen == 0)
        {
            OnDrowned?.Invoke();
        }

        rest.Step();
        eat.Step();
        drink.Step();
        mating.Step();
        if (reproduction.IsPregnant()) reproduction.StepPregnancy();

        rest.updateBar();
        eat.updateBar();
        drink.updateBar();
        mating.updateBar();
    }
    public void Decide()
    {
        if (predators?.Any() == true)
        {
            sensor.CheckForPredators();
            if (sensor.danger && status != Status.FLEE)
            {
                float fleeChance = Mathf.Clamp01(1f - bravery / 100f);
                float roll = UnityEngine.Random.value;

                if (roll < fleeChance)
                {
                    (prevStatus, status) = (Status.WANDER, Status.FLEE);
                    targetRef = null;
                    sensor.targetMask = LayerMask.GetMask("None");
                    return;
                }
            }
            else if (!sensor.danger && status == Status.FLEE)
            {
                (status, prevStatus) = (prevStatus, status);
                return;
            }
        }

        Status nextStatus = aiController.Decide(this);

        if ((nextStatus == Status.REST || nextStatus == Status.FLEE) && status != nextStatus)
        {
            (prevStatus, status) = (status, nextStatus);
            targetRef = null;
            sensor.targetMask = LayerMask.GetMask("None");
            return;
        }

        switch (status)
        {
            case Status.SEARCH_FOOD:
            case Status.SEARCH_DRINK:
            case Status.SEARCH_MATE:
            case Status.WANDER:
            case Status.REST:
                {
                    if (nextStatus == Status.SEARCH_FOOD || nextStatus == Status.SEARCH_DRINK || nextStatus == Status.SEARCH_MATE)
                    {
                        status = nextStatus;
                        if (status == Status.SEARCH_FOOD) setTargetLayerToEat();
                        else if (status == Status.SEARCH_DRINK) sensor.targetMask = LayerMask.GetMask("Drink");
                        else if (status == Status.SEARCH_MATE) setTargetLayerToMate();
                    }
                    else if (nextStatus == Status.WANDER)
                    {
                        status = Status.WANDER;
                        sensor.targetMask = LayerMask.GetMask("None");
                    }

                    if (status == Status.SEARCH_MATE && targetRef != null)
                    {
                        Animal targetedMate = targetRef.GetComponent<Animal>();
                        if (targetedMate == null || targetedMate.isMale == this.isMale || !targetedMate.mating.IsAcceptable(this))
                        {
                            this.rejectedBy.Add(targetRef);
                            targetRef = null;
                        }
                        else
                        {
                            reproduction.mate = targetedMate;
                            targetedMate.reproduction.mate = this;

                            targetRef = targetedMate.gameObject;
                            (prevStatus, status) = (status, Status.MOVE_TOWARDS);

                            targetedMate.setTargetLayerToMate();
                            targetedMate.targetRef = this.gameObject;
                            (targetedMate.prevStatus, targetedMate.status) = (targetedMate.status, Status.MOVE_TOWARDS);
                            return;
                        }
                    }

                    if (targetRef != null && status != Status.MOVE_TOWARDS)
                    {
                        (prevStatus, status) = (status, Status.MOVE_TOWARDS);
                        return;
                    }
                    break;
                }
            case Status.MOVE_TOWARDS:
                {
                    if (targetRef != null)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);

                        if (targetRef.TryGetComponent<Animal>(out var mateAnimal) && mateAnimal.species == this.species)
                        {
                            if (distanceToTarget < 7f)
                            {
                                mating.Mating(mateAnimal);
                                mateAnimal.mating.Mating(this);
                                (prevStatus, status) = (Status.WANDER, Status.WANDER);
                                return;
                            }
                        }
                        else if (distanceToTarget < 10f)
                        {
                            if (targetRef.GetComponent<IEdible>() != null)
                            {
                                eat.Eating();
                                (prevStatus, status) = (Status.WANDER, Status.WANDER);
                                return;
                            }
                            else if (targetRef.layer == LayerMask.NameToLayer("Drink"))
                            {
                                drink.Drinking();
                                (prevStatus, status) = (Status.WANDER, Status.WANDER);
                                return;
                            }
                        }
                    }
                    else
                    {
                        (status, prevStatus) = (prevStatus, Status.WANDER);
                        return;
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }
}