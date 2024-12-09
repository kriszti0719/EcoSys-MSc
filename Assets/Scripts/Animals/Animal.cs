using Assets.Scripts.Animals.Common.Behaviour;
using Assets.Scripts.Datatypes;
using System.Collections.Generic;
using System.Linq;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
    public CauseOfDeath cause;
    public Status status;
    public Status prevStatus;
    public Species species;
    public List<Species> predators;

    public GameObject prefab;
    public Material color;
    public int breakCounter = 0;
    public List<GameObject> destructibles = new List<GameObject>();
    public GameObject targetRef;
    public List<GameObject> spottedThreats;
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

    public GameObject barsContainer;

    public int stepCnt = 0;
    public int maxStepCnt = 10;
    public int decCnt = 0;
    public int maxDecCnt = 2;
    public float timeBetweenSteps = 0.1f; // TizedMásodperc
    private float timer = 0f;
    protected virtual void Start()
    {
        cause = CauseOfDeath.NONE;
        status = Status.WANDER;
        oxygen = maxOxygen;
        movement.StartMoving();
    }
    protected void SetComponents()
    {
        sensor = GetComponent<Sensor>();
        sensor.targetMask = sensor.targetMask = LayerMask.GetMask("None");
        reproduction = GetComponent<Reproduction>();
        rest = GetComponent<Rest>();
        drink = GetComponent<Drink>();
        eat = GetComponent<Eat>();
        die = GetComponent<Die>();
        aging = GetComponent<Age>();
        mating = GetComponent<Mate>();
        movement = GetComponent<Movement>();
    }
    public void SetTraits(Animal mother, Animal father)
    {
        SetComponents();

        aging.setAging(0.01f, (mother.aging.size + father.aging.size) / 2f, MutateTrait((mother.aging.lifeSpan + father.aging.lifeSpan) / 2f));
        eat.starving = Mathf.RoundToInt(MutateTrait((mother.eat.starving + father.eat.starving) / 2f));
        drink.drying = Mathf.RoundToInt(MutateTrait((mother.drink.drying + father.drink.drying) / 2f));
        movement.moveSpeed = MutateTrait((mother.movement.moveSpeed + father.movement.moveSpeed) / 2f);
        sensor.radius = Mathf.RoundToInt(MutateTrait((mother.sensor.radius + father.sensor.radius) / 2f));

        reproduction.setReproduction(Mathf.RoundToInt(MutateTrait((mother.reproduction.reproductiveUrge + father.reproduction.reproductiveUrge) / 2f)),
                                    Mathf.RoundToInt(MutateTrait((mother.reproduction.pregnancyDuration + father.reproduction.pregnancyDuration) / 2f)),
                                    false);
        mating.enableMating = false;
        mating.Charm = Mathf.RoundToInt(MutateTrait((mother.mating.Charm + father.mating.Charm) / 2f));

        setSpeciesSpecificTraits();
    }
    public void SetTraits(float rnd)
    {
        SetComponents();

        aging.setAging(1, rnd, Random.Range(4f, 6f));
        movement.moveSpeed = Random.Range(5f, 10f);
        sensor.radius = Random.Range(30, 70);
        reproduction.setReproduction(Random.Range(30, 50), 
                                    Random.Range(30, 60),
                                    true);
        mating.enableMating = true;
        mating.Charm = Random.Range(20, 100);
        drink.drying = Random.Range(30, 40);
        eat.starving = Random.Range(10, 50);

        setSpeciesSpecificTraits();
    }
    protected float MutateTrait(float averageTrait)
    {
        float mutationFactor = Random.Range(0.9f, 1.1f);

        // A véletlenszerû mutáció alkalmazása
        float mutatedTrait = averageTrait * mutationFactor;

        // Kerekítés és visszatérés
        return mutatedTrait;
    }
    public void SetBars(GameObject barsContainer)
    {
        rest.setBar(barsContainer);
        eat.setBar(barsContainer);        
        drink.setBar(barsContainer);
        mating.setBar(barsContainer);
        this.destructibles.Add(barsContainer);
    }
    public void SetAnimalData(GameObject prefab, Material color)
    {
        this.prefab = prefab;
        this.color = color;
        this.rejectedBy.Add(this.GameObject());
        //TODO: set charm according to color
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeBetweenSteps)
        {
            timer = 0f; // Timer reset
            aging.Aging();
            Step();
            Decide();
            if (predators != null && predators.Any())
            {
                sensor.CheckForPredators();
                if (spottedThreats.Any())
                {
                    if (status != Status.FLEE)
                    {
                        prevStatus = status;
                        status = Status.FLEE;
                    }
                    // ...
                }
                else if(status == Status.FLEE)
                {
                    Status tmp = status;
                    status = prevStatus;
                    prevStatus = tmp;
                }
            }
        }
    }
    public void Step()
    {
        stepCnt++;
        if (stepCnt == maxStepCnt)   
        {
            if (transform.localPosition.y < 20 && targetRef == null) {  oxygen--;  }
            else {  oxygen = maxOxygen;  }

            if (breakCounter != 0)
                breakCounter--;

            if (reproduction.isStillPregnant())
            {
                reproduction.StepPregnancy();
            }

            switch (status)
            {
                case Status.REST:
                    {
                        rest.Resting();
                        eat.Step();
                        drink.Step();
                        if(reproduction.isFertile)
                            mating.Step();                       
                        break;
                    }
                case Status.EAT:
                    {
                        rest.Resting();
                        eat.Eating();                        
                        drink.Step();
                        mating.Step();
                        break;
                    }
                case Status.DRINK:
                    {
                        rest.Resting();
                        eat.Step();
                        drink.Drinking();                        
                        mating.Step();
                        break;
                    }
                case Status.MATE:
                    {
                        rest.Step();
                        eat.Step();
                        drink.Step();
                        mating.Mating();
                        if (breakCounter == 0)
                        {
                            mating.isSuccess();
                        }
                        break;
                    }
                case Status.SEARCH_FOOD:
                case Status.SEARCH_DRINK:
                case Status.SEARCH_MATE:
                    {
                        rest.Step();
                        eat.Step();
                        drink.Step();
                        mating.Step();
                        break;
                    }
                case Status.DIE: { break; }
                default:
                    {
                        rest.Step();
                        eat.Step();
                        drink.Step();
                        mating.Step();
                        break;
                    }
            }
            rest.updateBar();
            eat.updateBar();
            drink.updateBar();
            mating.updateBar();
            stepCnt = 0;
        }
    }
    public void Decide()
    {
        decCnt++;

        if(status == Status.CAUGHT && breakCounter == 0)
        {
            cause = CauseOfDeath.CONSUMED;
            die.ToDie();
        }
        else if (status != Status.DIE && eat.isStarvedToDeath())
        {
            cause = CauseOfDeath.HUNGER;
            die.ToDie();
        }
        else if (status != Status.DIE && drink.isDriedToDeath())
        {
            cause = CauseOfDeath.THIRST;
            die.ToDie();
        }
        else if (status != Status.DIE && oxygen == 0)
        {
            cause = CauseOfDeath.DROWN;
            die.ToDie();
        }

        if (rest.isFainted())
        {
            rest.ChanceToRest();
        }

        if (decCnt == maxDecCnt)
        {   
            switch (status)
            {                
                case Status.SEARCH_MATE:
                    {
                        if(eat.isStarving())
                        {
                            setTargetLayerToEat();
                            prevStatus = status;
                            status = Status.SEARCH_FOOD;
                        }
                        if(drink.isDrying())
                        {
                            sensor.targetMask = LayerMask.GetMask("Drink");
                            prevStatus = status;
                            status = Status.SEARCH_DRINK;
                        }
                        if (targetRef != null)
                        {
                            Animal tmp = targetRef.GetComponent<Animal>();
                            tmp.reproduction = targetRef.GetComponent<Reproduction>();
                            if (tmp != null && tmp.isMale == this.isMale)
                            {
                                this.rejectedBy.Add(targetRef);
                                this.targetRef = null;
                            }
                            else
                            {
                                if (tmp != null && tmp.mating.isAcceptable(this))
                                {
                                    reproduction.mate = tmp;
                                    tmp.reproduction.mate = this;
                                    prevStatus = status;
                                    status = Status.MOVE_TOWARDS;
                                }
                                else
                                {
                                    this.rejectedBy.Add(targetRef);
                                    this.targetRef = null;
                                }
                            }
                        }
                        break;
                    }
                case Status.SEARCH_FOOD:
                case Status.SEARCH_DRINK:
                    {
                        rest.ChanceToRest();
                        if (targetRef != null)
                        {
                            status = Status.MOVE_TOWARDS;
                        }
                        break;
                    }
                case Status.WANDER:
                    {
                        rest.ChanceToRest();
                        if (sensor.targetMask == LayerMask.GetMask("None"))
                        {
                            if (eat.currentHunger < drink.currentThirst && eat.isHungry())
                            {
                                setTargetLayerToEat();
                                status = Status.SEARCH_FOOD;
                            }
                            else if (drink.isThirsty())
                            {
                                sensor.targetMask = LayerMask.GetMask("Drink");
                                status = Status.SEARCH_DRINK;
                            }
                            else if (reproduction.isFertile && mating.enableMating)
                            {
                                setTargetLayerToMate();
                                status = Status.SEARCH_MATE;
                            }
                        }
                        break;
                    }
                case Status.MOVE_TOWARDS:
                    {
                        rest.ChanceToRest();
                        if (targetRef != null)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
                            if (distanceToTarget < 7f)
                            {
                                if(prevStatus == Status.SEARCH_MATE)
                                {
                                    Animal mate = targetRef.GetComponent<Animal>();
                                    if (mate != null)
                                    {
                                        if(mate.status == Status.WAIT)
                                            mate.mating.ToMate();
                                        mating.ToMate();
                                    }
                                }
                                if (distanceToTarget < 5f)
                                {
                                    if (prevStatus == Status.SEARCH_FOOD)
                                    {
                                        eat.StartEating();
                                    }
                                    else if (prevStatus == Status.SEARCH_DRINK)
                                    {
                                        drink.StartDrinking();
                                    }
                                }
                            }
                        }
                        else
                            status = prevStatus;
                        break;
                    }
                case Status.REST:
                    {
                        if (eat.isStarving())
                        {
                            setTargetLayerToEat();
                            prevStatus = status;
                            status = Status.SEARCH_FOOD;
                        }
                        else if (drink.isDrying())
                        {
                            sensor.targetMask = LayerMask.GetMask("Drink");
                            prevStatus = status;
                            status = Status.SEARCH_DRINK;
                        }
                        else if (rest.isRested() || breakCounter == 0)
                        {
                            status = prevStatus;
                            prevStatus = Status.REST;
                        }
                        break;
                    }
                case Status.EAT:
                    {
                        if (eat.isFull() || breakCounter == 0 || targetRef == null)
                        {
                            breakCounter = 0;
                            eat.FinishEating();
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");
                            status = Status.WANDER;
                        }
                        break;
                    }
                case Status.DRINK:
                    {
                        if (drink.isFull() || breakCounter == 0)
                        {
                            drink.FinishDrinking();
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");
                            status = Status.WANDER;
                        }
                        break;
                    }
                case Status.MATE:
                    {
                        if (breakCounter == 0)
                        {
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");                            
                            status = Status.WANDER;
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            decCnt = 0;
        }
    }
    public abstract void setTargetLayerToMate();
    protected abstract void setTargetLayerToEat();
    public abstract int getTargetLayerToMate();
    public abstract int getTargetLayerToEat();
    public abstract int getPredatorLayers();
    public abstract void setSpeciesSpecificTraits();
}