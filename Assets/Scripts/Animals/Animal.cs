using Assets.Scripts.Animals.Common.Behaviour;
using System.Collections.Generic;
using System.Linq;
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

    public GameObject barsContainer;

    public int frameCounter = 0;
    public int stepCnt = 0;
    public int maxStepCnt = 8;
    public int decCnt = 0;
    public int maxDecCnt = 2;
    public int framesPerChange = 6; // 60 frame = 1 másodperc, HA 60 FPS
    public event System.Action OnDeath;
    private bool IsDying() => status == Status.DIE && !movement.isDying;
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
        eventHandler = GetComponent<EventHandler>();
    }
    protected void SubscribeToNeeds()
    {
        eat.OnHungerCritical += eventHandler.HandleHungerCritical;
        eat.OnHungerFull += eventHandler.HandleHungerFull;
        eat.OnHungerDepleted += eventHandler.HandleHungerDepleted;

        drink.OnThirstCritical += eventHandler.HandleThirstCritical;
        drink.OnThirstFull += eventHandler.HandleThirstFull;
        drink.OnThirstDepleted += eventHandler.HandleThirstDepleted;

        rest.OnRestFull += eventHandler.HandleRestFull;
        rest.OnRestDepleted += eventHandler.HandleRestDepleted;

        aging.OnAgeLimitReached += eventHandler.HandleAgeLimitReached;
    }
    public void SetTraits(Animal mother, MateTraits father)
    {
        SetComponents();

        aging.setAging(0.01f, (mother.aging.size + father.size) / 2f, MutateTrait(mother.aging.lifeSpan, father.lifeSpan));
        eat.critical = Mathf.RoundToInt(MutateTrait(mother.eat.critical, father.starving));
        drink.critical = Mathf.RoundToInt(MutateTrait(mother.drink.critical, father.drying));
        movement.moveSpeed = MutateTrait(mother.movement.moveSpeed, father.moveSpeed);
        sensor.radius = Mathf.RoundToInt(MutateTrait(mother.sensor.radius, father.radius));

        reproduction.setReproduction(Mathf.RoundToInt(MutateTrait(mother.reproduction.reproductiveUrge, father.reproductiveUrge)),
                                    Mathf.RoundToInt(MutateTrait(mother.reproduction.pregnancyDuration, father.pregnancyDuration)),
                                    false);
        mating.enableMating = false;
        mating.charm = Mathf.RoundToInt(MutateTrait(mother.mating.charm, father.charm));

        setSpeciesSpecificTraits();
    }
    public void SetTraits(float rnd)
    {
        SetComponents();

        aging.setAging(1, rnd, Random.Range(4f, 6f));
        movement.moveSpeed = Random.Range(1f, 10f);
        sensor.radius = Random.Range(30, 70);
        reproduction.setReproduction(Random.Range(30, 50), 
                                    Random.Range(30, 60),
                                    true);
        mating.enableMating = true;
        mating.charm = Random.Range(20, 100);
        drink.critical = Random.Range(25, 35);
        eat.critical = Random.Range(15, 25);

        setSpeciesSpecificTraits();
        SubscribeToNeeds();
    }
    protected float MutateTrait(float motherTrait, float fatherTrait)
    {
        float averageTrait = (motherTrait + fatherTrait) / 2f;
        float mutationFactor = Random.Range(0.8f, 1.2f);
        float improvementBias = 1.05f;
        float mutatedTrait = averageTrait * mutationFactor * improvementBias;
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
        this.rejectedBy.Add(this.GameObject()); ;
        //TODO: set charm according to color
    }
    void Update()
    {
        frameCounter++;
        if (frameCounter == framesPerChange)
        {
            aging.Aging();
            Step();
            Decide();
            if (predators?.Any() == true)
            {
                sensor.CheckForPredators();
                if (sensor.danger && status != Status.FLEE)
                {
                    (prevStatus, status) = (Status.WANDER, Status.FLEE);
                    targetRef = null;
                    sensor.targetMask = LayerMask.GetMask("None");

                }
                else if (!sensor.danger && status == Status.FLEE)
                {
                    (status, prevStatus) = (prevStatus, status);
                }
            }
            frameCounter = 0;
        }
    }
    public void Step()
    {
        stepCnt++;
        if (stepCnt == maxStepCnt)   
        {
            breakCounter = System.Math.Max(0, breakCounter - 1);
            oxygen = (transform.localPosition.y < 20 && targetRef == null) ? oxygen - 1 : maxOxygen;

            if (!(status == Status.DIE || status == Status.CAUGHT))
            {
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

            if (status == Status.MATE && (breakCounter == 0))
                mating.IsSuccess();

            if (status != Status.DIE)
            {
                if (oxygen == 0) cause = CauseOfDeath.DROWN;
            }

            if (status == Status.CAUGHT && breakCounter == 0)
            {
                die.Destroy();
                return;
            }

            if (IsDying())
            {
                OnDeath?.Invoke();
            }

            stepCnt = 0;
        }
    }
    public void Decide()
    {
        decCnt++;

        if (decCnt == maxDecCnt)
        {   
            switch (status)
            {                
                case Status.SEARCH_MATE:
                    {
                        if (rest.ChanceToRest()) prevStatus = status = Status.REST;
                        if (targetRef != null)
                        {
                            Animal tmp = targetRef.GetComponent<Animal>();
                            if (tmp == null || tmp.isMale == this.isMale || !tmp.mating.IsAcceptable(this))
                            {
                                this.rejectedBy.Add(targetRef);
                                targetRef = null;
                            }
                            else
                            {
                                tmp.reproduction = targetRef.GetComponent<Reproduction>();
                                reproduction.mate = tmp;
                                tmp.reproduction.mate = this;

                                bool canSee = sensor.FieldOfViewCheck(getTargetLayerToMate(), reproduction.mate.transform.gameObject);
                                if (!canSee)
                                {
                                    sensor.targetMask = LayerMask.GetMask("None");
                                    targetRef = null;
                                    (prevStatus, status) = (status, Status.WAIT);
                                }
                                else
                                {
                                    setTargetLayerToMate();
                                    targetRef = reproduction.mate.transform.gameObject;
                                    (prevStatus, status) = (status, Status.MOVE_TOWARDS);
                                }
                            }
                        }
                        break;
                    }
                case Status.SEARCH_FOOD:
                case Status.SEARCH_DRINK:
                case Status.WANDER:
                    {
                        if (rest.ChanceToRest()) prevStatus = status = Status.REST;
                        if (sensor.targetMask == LayerMask.GetMask("None"))
                        {
                            if (eat.currentHunger < drink.currentThirst && eat.IsHungry())   // TODO: ez tul sokszor lesz igaz tho
                            {
                                setTargetLayerToEat();
                                status = Status.SEARCH_FOOD;
                            }
                            else if (!(reproduction.isFertile && mating.enableMating) || drink.IsThirsty())
                            {
                                sensor.targetMask = LayerMask.GetMask("Drink");
                                status = Status.SEARCH_DRINK;
                            }
                            else
                            {
                                setTargetLayerToMate();
                                status = Status.SEARCH_MATE;
                            }
                        }
                        if (targetRef != null)
                        {
                            (prevStatus, status) = (status, Status.MOVE_TOWARDS);
                        }
                        break;
                    }
                case Status.MOVE_TOWARDS:
                    {
                        if (rest.ChanceToRest()) prevStatus = status = Status.REST;
                        if (targetRef != null)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
                            if (prevStatus == Status.SEARCH_MATE && distanceToTarget < 7f)      // TODO: allat meret fuggo
                            {
                                Animal mate = targetRef.GetComponent<Animal>();
                                if (mate != null)
                                {
                                    if(mate.status == Status.WAIT)
                                    {
                                        mate.mating.ToMate();
                                        (mate.status, mate.prevStatus) = (mate.prevStatus, Status.WANDER);
                                    }
                                    mating.ToMate();
                                    (status, prevStatus) = (prevStatus, Status.MATE);
                                }
                            }
                            if (distanceToTarget < 5f)
                            {
                                if (prevStatus == Status.SEARCH_FOOD)
                                {
                                    eat.StartEating();
                                    status = Status.EAT;
                                }
                                else if (sensor.targetMask == LayerMask.GetMask("Drink"))
                                {
                                    drink.StartDrinking();
                                    status = Status.DRINK;
                                }
                            }
                        }
                        else
                        {
                            (status, prevStatus) = (prevStatus, Status.WANDER);
                        }
                        break;
                    }
                case Status.REST:
                    {
                        if (breakCounter == 0)
                        {
                            status = prevStatus;
                        }
                        break;
                    }
                case Status.EAT:
                case Status.DRINK:
                case Status.MATE:
                    {
                        if (targetRef == null || breakCounter == 0)
                        {
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");
                            (prevStatus, status) = (status, Status.WANDER);
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
}