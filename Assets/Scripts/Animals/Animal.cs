using Assets.Scripts.Animals.Common.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    // TODO: remove (only for testing purposes)
    public int triedForBaby = 0;
    public int gaveBirth = 0;
    public int kids = 0;

    public CauseOfDeath cause;
    public Status status;
    public Status prevStatus;
    public Species species;
    public List<Species> predators = new List<Species>();

    public GameObject prefab;
    public Material color;
    public int breakCounter = 0;
    public List<GameObject> destructibles = new List<GameObject>();
    public GameObject targetRef;
    public List<GameObject> spottedThreats = new List<GameObject>();
    public List<GameObject> rejectedBy = new List<GameObject>();

    public bool isMale;

    int maxOxygen = 10;
    public int oxygen;

    public Sensor sensor;
    public Reproduction reproduction;
    public Rest rest;
    public Drink drink;
    public Eat eat;
    public Die die;
    public Age aging;
    public Mate mating;
    public Movement movement;
    public EventHandler eventHandler;

    public int bravery;

    public event Action OnBreakEnded;
    public event Action OnDrowned;

    public GameObject barsContainer;

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
    private bool IsBreakEnded() => breakCounter == 0;
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
        eventHandler = GetComponent<EventHandler>();
    }
    public void SetTraits(float rnd)
    {
        SetComponents();

        aging.setAging(1, rnd, UnityEngine.Random.Range(4f, 6f));
        bravery = UnityEngine.Random.Range(20, 40);
        movement.moveSpeed = UnityEngine.Random.Range(1f, 10f);
        sensor.setSensor(
            _radius: UnityEngine.Random.Range(30, 70),
            _camouflage: UnityEngine.Random.Range(30, 60),
            _stealth: UnityEngine.Random.Range(20, 50)
        );

        reproduction.setReproduction(
            _reproductiveUrge: UnityEngine.Random.Range(30, 50),
            _pregnancyDuration: UnityEngine.Random.Range(3, 6),
            _isFertile: true
        );
        mating.enableMating = true;
        mating.charm = UnityEngine.Random.Range(20, 100);
        drink.critical = UnityEngine.Random.Range(25, 35);
        eat.critical = UnityEngine.Random.Range(15, 25);

        setSpeciesSpecificTraits();
    }
    public void SetTraits(Animal mother, MateTraits father)
    {
        SetComponents();

        aging.setAging(
            _age: 0.01f,
            _size: (mother.aging.size + father.size) / 2f,
            _lifeSpan: MutateTrait(mother.aging.lifeSpan, father.lifeSpan)
        );
        bravery = Mathf.RoundToInt(MutateTrait(mother.bravery, father.bravery));
        eat.critical = Mathf.RoundToInt(MutateTrait(mother.eat.critical, father.eat_critical));
        drink.critical = Mathf.RoundToInt(MutateTrait(mother.drink.critical, father.drink_critical));
        movement.moveSpeed = MutateTrait(mother.movement.moveSpeed, father.moveSpeed);
        sensor.setSensor(
            _radius: Mathf.RoundToInt(MutateTrait(mother.sensor.radius, father.radius)),
            _camouflage: Mathf.RoundToInt(MutateTrait(mother.sensor.camouflage, father.camouflage)),
            _stealth: Mathf.RoundToInt(MutateTrait(mother.sensor.stealth, father.stealth))
        );
        reproduction.setReproduction(
            _reproductiveUrge: Mathf.RoundToInt(MutateTrait(mother.reproduction.reproductiveUrge, father.reproductiveUrge)),
            _pregnancyDuration: Mathf.RoundToInt(MutateTrait(mother.reproduction.pregnancyDuration, father.pregnancyDuration)),
            _isFertile: false
        );
        mating.enableMating = false;
        mating.charm = Mathf.RoundToInt(MutateTrait(mother.mating.charm, father.charm));

        setSpeciesSpecificTraits();
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
        this.destructibles.Add(barsContainer);
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
        eat.OnHungerCritical += eventHandler.HandleHungerCritical;
        eat.OnHungerDepleted += eventHandler.HandleHungerDepleted;

        drink.OnThirstCritical += eventHandler.HandleThirstCritical;
        drink.OnThirstDepleted += eventHandler.HandleThirstDepleted;

        rest.OnRestFull += eventHandler.HandleRestFull;
        rest.OnRestDepleted += eventHandler.HandleRestDepleted;

        aging.OnAgeLimitReached += eventHandler.HandleAgeLimitReached;

        OnDrowned += eventHandler.HandleDrowning;
        OnBreakEnded += eventHandler.HandleBreakEnded;
    }
    protected virtual void Start()
    {
        Subscribe();
        cause = CauseOfDeath.NONE;
        status = Status.WANDER;
        oxygen = maxOxygen; // TODO: delete
        movement.StartMoving();
    }
    public void Step()
    {
        if(breakCounter != 0)
        {
            breakCounter = System.Math.Max(0, breakCounter - 1);
            if (IsBreakEnded())
            {
                OnBreakEnded?.Invoke();
            }
        }

        oxygen = (transform.localPosition.y < 20 && targetRef == null) ? oxygen - 1 : maxOxygen;
        if (oxygen == 0)
        {
            OnDrowned?.Invoke();
        }

        rest.Step();
        eat.Step();
        drink.Step();
        if (reproduction.isFertile) mating.Step();
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
                // --- Bravery roll ---
                float fleeChance = Mathf.Clamp01(1f - bravery / 100f);
                float roll = UnityEngine.Random.value;

                if (roll < fleeChance)
                {
                    (prevStatus, status) = (Status.WANDER, Status.FLEE);
                    targetRef = null;
                    sensor.targetMask = LayerMask.GetMask("None");
                }
            }
            else if (!sensor.danger && status == Status.FLEE)
            {
                (status, prevStatus) = (prevStatus, status);
            }
        }

        switch (status)
        {
            case Status.SEARCH:
            case Status.WANDER:
                {
                    if (rest.ChanceToRest()) (prevStatus, status) = (status, Status.REST);
                    
                    // If we were searching for a mate specifically, check target acceptance
                    if (status == Status.SEARCH && sensor.targetMask == (1 << getTargetLayerToMate()))
                    {
                        if (targetRef != null)
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
                            }
                        }
                    }

                    if (sensor.targetMask == LayerMask.GetMask("None"))
                    {
                        if (eat.currentHunger < drink.currentThirst && eat.IsHungry())
                        {
                            setTargetLayerToEat();
                            status = Status.SEARCH;
                        }
                        else if (!(reproduction.isFertile && mating.enableMating) || drink.IsThirsty())
                        {
                            sensor.targetMask = LayerMask.GetMask("Drink");
                            status = Status.SEARCH;
                        }
                        else
                        {
                            setTargetLayerToMate();
                            status = Status.SEARCH;
                        }
                    }

                    if (targetRef != null && status != Status.MOVE_TOWARDS)
                    {
                        (prevStatus, status) = (status, Status.MOVE_TOWARDS);
                    }
                    break;
                }
            case Status.MOVE_TOWARDS:
                {
                    if (rest.ChanceToRest()) (prevStatus, status) = (status, Status.REST);
                    if (targetRef != null)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
                        
                        if (targetRef.TryGetComponent<Animal>(out var mateAnimal) && mateAnimal.species == this.species)
                        {
                            if (distanceToTarget < 7f)
                            {
                                reproduction.mate = mateAnimal;
                                mateAnimal.reproduction.mate = this;

                                mating.Mating();
                                mateAnimal.mating.Mating();
                                (prevStatus, status) = (Status.WANDER, Status.WANDER);
                            }
                        }
                        else if (distanceToTarget < 5f)
                        {
                            if (targetRef.GetComponent<IEdible>() != null)
                            {
                                eat.Eating();
                                (prevStatus, status) = (Status.WANDER, Status.WANDER);
                            }
                            else if (targetRef.layer == LayerMask.NameToLayer("Drink"))
                            {
                                drink.Drinking();
                                (prevStatus, status) = (Status.WANDER, Status.WANDER);
                            }
                        }
                    }
                    else
                    {
                        (status, prevStatus) = (prevStatus, Status.WANDER);
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