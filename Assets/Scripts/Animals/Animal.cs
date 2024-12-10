using Assets.Scripts;
using Assets.Scripts.Animals.Common.Behaviour;
using Assets.Scripts.Datatypes;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    public GameObject barsContainer;

    public int stepCnt = 0;
    public int maxStepCnt = 10;
    public int decCnt = 0;
    public int maxDecCnt = 2;
    public float timeBetweenSteps = 0.1f; // TizedMásodperc
    private float timer = 0f;
    private int started = 0;
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
    public void SetTraits(Animal mother, MateTraits father)
    {
        SetComponents();

        aging.setAging(0.01f, (mother.aging.size + father.size) / 2f, MutateTrait(mother.aging.lifeSpan, father.lifeSpan));
        eat.starving = Mathf.RoundToInt(MutateTrait(mother.eat.starving, father.starving));
        drink.drying = Mathf.RoundToInt(MutateTrait(mother.drink.drying, father.drying));
        movement.moveSpeed = MutateTrait(mother.movement.moveSpeed, father.moveSpeed);
        sensor.radius = Mathf.RoundToInt(MutateTrait(mother.sensor.radius, father.radius));

        reproduction.setReproduction(Mathf.RoundToInt(MutateTrait(mother.reproduction.reproductiveUrge, father.reproductiveUrge)),
                                    Mathf.RoundToInt(MutateTrait(mother.reproduction.pregnancyDuration, father.pregnancyDuration)),
                                    false);
        mating.enableMating = false;
        mating.charm = Mathf.RoundToInt(MutateTrait(mother.mating.charm, father.charm));

        setSpeciesSpecificTraits();
    }
    public abstract void SetTraits(float rnd);
    protected float MutateTrait(float motherTrait, float fatherTrait)
    {
        // Átlag kiszámítása a szülõk tulajdonságai alapján
        float averageTrait = (motherTrait + fatherTrait) / 2f;

        // Mutációs faktor: lehet +/- 20%-os eltérés
        float mutationFactor = Random.Range(0.8f, 1.2f);

        // Javulási torzítás (enyhe fejlõdést ösztönöz)
        float improvementBias = 1.05f;

        // Mutáció alkalmazása
        float mutatedTrait = averageTrait * mutationFactor * improvementBias;

        // Minimum érték biztosítása, hogy ne essen túl alacsonyra
        mutatedTrait = Mathf.Max(mutatedTrait, 0.1f);

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
            if (status != Status.CAUGHT && status != Status.DIE && predators != null && predators.Any())
            {
                sensor.CheckForPredators();
                if (spottedThreats.Any())
                {
                    if (status != Status.FLEE)
                    {
                        prevStatus = status;
                        status = Status.FLEE;
                    }
                }
                else if(status == Status.FLEE)
                {
                    status = Status.WANDER;
                    prevStatus = Status.FLEE;
                }
            }
        }
    }
    public void Step()
    {
        stepCnt++;
        if (stepCnt == maxStepCnt)   
        {
            if (breakCounter != 0)
            {
                breakCounter--;
            }
            stepCnt = 0;
            if (transform.localPosition.y < 20 && targetRef == null) {  oxygen--;  }
            else {  oxygen = maxOxygen;  }
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
                            mating.Success();
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
                        break;
                    }
            }
            rest.updateBar();
            eat.updateBar();
            drink.updateBar();
            mating.updateBar();
        }
    }
    public void Decide()
    {
        decCnt++;

        if (status != Status.DIE && eat.isStarvedToDeath())
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
                                break;
                            }
                            else
                            {
                                if (tmp != null && tmp.mating.isAcceptable(this))
                                {
                                    reproduction.mate = tmp;
                                    reproduction.mateTraits = new MateTraits(tmp);
                                    tmp.reproduction.mate = this;
                                    tmp.reproduction.mateTraits = new MateTraits(this);
                                    if (reproduction.mate == null ||
                                        tmp.reproduction.mate == null ||
                                        tmp.reproduction.mateTraits == null ||
                                        reproduction.mate == null)
                                    {
                                        throw new System.Exception("BigRip");
                                    }
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
                            prevStatus = status;
                            status = Status.MOVE_TOWARDS;
                        }
                        break;
                    }
                case Status.WANDER:
                    {
                        rest.ChanceToRest();
                        if (sensor.targetMask == LayerMask.GetMask("None"))
                        {
                            if (reproduction.isFertile && mating.enableMating && !eat.isHungry() && !drink.isThirsty())
                            {
                                setTargetLayerToMate();
                                status = Status.SEARCH_MATE;
                            }
                            else if (drink.isThirsty())
                            {
                                sensor.targetMask = LayerMask.GetMask("Drink");
                                status = Status.SEARCH_DRINK;
                            }
                            else
                            {
                                setTargetLayerToEat();
                                status = Status.SEARCH_FOOD;
                            }
                        }
                        break;
                    }
                case Status.MOVE_TOWARDS:
                    {
                        if (targetRef != null)
                        {
                            Animal targetAnimal = targetRef.GetComponent<Animal>();
                            if (targetAnimal != null)
                            {
                                if (targetAnimal.status == Status.CAUGHT)
                                {
                                    rejectedBy.Add(targetRef);
                                    targetRef = null;
                                    prevStatus = Status.WANDER;
                                    status = Status.SEARCH_FOOD;
                                    break;
                                }
                            }

                            float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
                            if (distanceToTarget < 7f)
                            {
                                if (reproduction.mate != null)
                                {
                                    if(reproduction.mate.status == Status.WAIT)
                                        reproduction.mate.mating.ToMate();
                                    mating.ToMate();
                                    rejectedBy = new List<GameObject>
                                    {
                                        this.gameObject
                                    };
                                    break;
                                }

                                if (distanceToTarget < 5f)
                                {
                                    if (prevStatus == Status.SEARCH_FOOD)
                                    {
                                        //if (started > 0)
                                        //{
                                        //    throw new System.Exception("Already did that");
                                        //}
                                        //started++;
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
                            prevStatus = Status.REST;
                            status = Status.SEARCH_FOOD;
                        }
                        else if (drink.isDrying())
                        {
                            sensor.targetMask = LayerMask.GetMask("Drink");
                            prevStatus = Status.REST;
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
                        if (targetRef == null | breakCounter == 0 | eat.food == null | eat.isFull())
                        {
                            breakCounter = 0;
                            //started--;
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
                            if(prevStatus == Status.EAT)
                            {
                                status = Status.EAT;
                                prevStatus = Status.SEARCH_FOOD;
                            }
                            else if(prevStatus == Status.DRINK)
                            {
                                status = Status.DRINK;
                                prevStatus = Status.SEARCH_DRINK;
                            }
                            else
                            {
                                targetRef = null;
                                sensor.targetMask = LayerMask.GetMask("None");
                                prevStatus = Status.MATE;
                                status = Status.WANDER;
                            }
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
    public Species getSpecies()
    {
        return species;
    }
}