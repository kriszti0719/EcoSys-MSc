using Assets.Scripts.Animals.Common.Behaviour;
using System.Collections;
using System.Collections.Generic;
<<<<<<< Updated upstream
using Unity.Burst.CompilerServices;
=======
using System.Diagnostics.Tracing;
using System.Linq;
>>>>>>> Stashed changes
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
<<<<<<< Updated upstream
=======
    public CauseOfDeath cause;
    public Status status;
    public Status prevStatus;
    public Species species;
    public List<Species> predators;



>>>>>>> Stashed changes
    public GameObject prefab;
    public Material color;
    public int breakCounter = 0;
    public List<GameObject> destructibles = new List<GameObject>();
    public GameObject targetRef;
    public List<GameObject> noTargetRefs = new List<GameObject>();   // TODO: idõvel felejtsen

    public CauseOfDeath cause;
    public Status status;
    public Status prevStatus;

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
<<<<<<< Updated upstream
    public int framesPerChange = 6; // 60 frame = 1 másodperc, ha 60 FPS


=======
    public int framesPerChange = 6; // 60 frame = 1 másodperc, HA 60 FPS
    public event System.Action OnDeath;
    private bool IsDying() => status == Status.DIE && !movement.isDying;
    public abstract void setTargetLayerToMate();
    public abstract void setTargetLayerToEat();
    public abstract int getTargetLayerToMate();
    public abstract int getTargetLayerToEat();
    public abstract int getPredatorLayers();
    public abstract void setSpeciesSpecificTraits();
>>>>>>> Stashed changes
    protected virtual void Start()
    {
        cause = CauseOfDeath.NONE;
        status = Status.WANDERING;
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
        eat.food.OnConsumed += eventHandler.HandleFoodConsumed;

        drink.OnThirstCritical += eventHandler.HandleThirstCritical;
        drink.OnThirstFull += eventHandler.HandleThirstFull;
        drink.OnThirstDepleted += eventHandler.HandleThirstDepleted;

        rest.OnRestFull += eventHandler.HandleRestFull;
        rest.OnRestDepleted += eventHandler.HandleRestDepleted;

        aging.OnAgeLimitReached += eventHandler.HandleAgeLimitReached;
    }
    public void SetTraits(Animal mother, Animal father)
    {
        SetComponents();

        aging.setAging(0.01f, (mother.aging.size + father.aging.size) / 2f, MutateTrait((mother.aging.lifeSpan + father.aging.lifeSpan) / 2f));
        eat.critical = Mathf.RoundToInt(MutateTrait((mother.eat.critical + father.eat.critical) / 2f));
        drink.critical = Mathf.RoundToInt(MutateTrait((mother.drink.critical + father.drink.critical) / 2f));
        movement.moveSpeed = MutateTrait((mother.movement.moveSpeed + father.movement.moveSpeed) / 2f);
        sensor.radius = Mathf.RoundToInt(MutateTrait((mother.sensor.radius + father.sensor.radius) / 2f));

        reproduction.setReproduction(Mathf.RoundToInt(MutateTrait((mother.reproduction.reproductiveUrge + father.reproduction.reproductiveUrge) / 2f)),
                                    Mathf.RoundToInt(MutateTrait((mother.reproduction.pregnancyDuration + father.reproduction.pregnancyDuration) / 2f)),
                                    false);
        mating.enableMating = false;
        mating.Charm = Mathf.RoundToInt(MutateTrait((mother.mating.Charm + father.mating.Charm) / 2f));
<<<<<<< Updated upstream
;
=======

        setSpeciesSpecificTraits();
        SubscribeToNeeds();
>>>>>>> Stashed changes
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
        mating.Charm = Random.Range(20, 100);
<<<<<<< Updated upstream
        drink.drying = Random.Range(25, 35);
        eat.starving = Random.Range(15, 25);
=======
        drink.critical = Random.Range(25, 35);
        eat.critical = Random.Range(15, 25);

        setSpeciesSpecificTraits();
        SubscribeToNeeds();
>>>>>>> Stashed changes
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
        this.noTargetRefs.Add(this.GameObject());
        //TODO: set charm according to color
    }
    void Update()
    {
        frameCounter++;
        if (frameCounter == framesPerChange)
        {
            // Végezzen el egy mûveletet
            aging.Aging();
            Step();
            Decide();
<<<<<<< Updated upstream
            //if (movement.isWandering)
            //{
            //    movement.WanderTo();
            //}

=======
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
>>>>>>> Stashed changes
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

            if (IsDying())
            {
<<<<<<< Updated upstream
                case Status.RESTING:
                    {
                        rest.Resting();
                        if (rest.isRested())
                        {
                            breakCounter = 0;
                        }
                        eat.Step();
                        drink.Step();
                        if(reproduction.isFertile)
                            mating.Step();                       
                        break;
                    }
                case Status.EATING:
                    {
                        rest.Resting();
                        eat.Eating();
                        if (eat.isFull())
                        {
                            eat.FinishEating();
                            breakCounter = 0;
                        }
                        drink.Step();
                        mating.Step();
                        break;
                    }
                case Status.DRINKING:
                    {
                        rest.Resting();
                        eat.Step();
                        drink.Drinking();
                        if (drink.isFull())
                        {
                            drink.FinishDrinking();
                            breakCounter = 0;
                        }
                        mating.Step();
                        break;
                    }
                case Status.MATING:
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
                case Status.SEARCHINGFOOD:
                case Status.SEARCHINGDRINK:
                case Status.SEARCHINGMATE:
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
=======
                OnDeath?.Invoke();
>>>>>>> Stashed changes
            }

            stepCnt = 0;
        }
    }
    public void Decide()
    {
        decCnt++;

<<<<<<< Updated upstream
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

=======
>>>>>>> Stashed changes
        if (decCnt == maxDecCnt)
        {   
            switch (status)
            {                
                case Status.SEARCHINGMATE:
                    {
<<<<<<< Updated upstream
                        if(eat.isStarving())
                        {
                            setTargetLayerToEat();
                            status = Status.SEARCHINGFOOD;
                        }
                        if(drink.isDrying())
                        {
                            sensor.targetMask = LayerMask.GetMask("Drink");
                            status = Status.SEARCHINGDRINK;
                        }
=======
                        if (rest.ChanceToRest()) prevStatus = status = Status.REST;
>>>>>>> Stashed changes
                        if (targetRef != null)
                        {
                            Animal tmp = targetRef.GetComponent<Animal>();
                            if (tmp == null || tmp.isMale == this.isMale || !tmp.mating.IsAcceptable(this))
                            {
                                noTargetRefs.Add(targetRef);
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
<<<<<<< Updated upstream
                                    reproduction.mate = tmp;
                                    tmp.reproduction.mate = this;
                                    prevStatus = status;
                                    status = Status.MOVING;
=======
                                    sensor.targetMask = LayerMask.GetMask("None");
                                    targetRef = null;
                                    (prevStatus, status) = (status, Status.WAIT);
>>>>>>> Stashed changes
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
                case Status.SEARCHINGFOOD:
                case Status.SEARCHINGDRINK:
                case Status.WANDERING:
                    {
                        if (rest.ChanceToRest()) prevStatus = status = Status.REST;
                        if (sensor.targetMask == LayerMask.GetMask("None"))
                        {
                            if (eat.currentHunger < drink.currentThirst && eat.IsHungry())   // TODO: ez tul sokszor lesz igaz tho
                            {
                                setTargetLayerToEat();
                                status = Status.SEARCHINGFOOD;
                            }
                            else if (!(reproduction.isFertile && mating.enableMating) || drink.IsThirsty())
                            {
                                sensor.targetMask = LayerMask.GetMask("Drink");
                                status = Status.SEARCHINGDRINK;
                            }
<<<<<<< Updated upstream
                            if (targetRef != null)
                            {
                                prevStatus = status;
                                status = Status.MOVING;
                            }
                        }
                        else
                        {
                            if(eat.currentHunger < drink.currentThirst && eat.isHungry() && sensor.targetMask == LayerMask.GetMask("None"))
                            {
                                setTargetLayerToEat();
                                status = Status.SEARCHINGFOOD;
                            }
                            else if(drink.isThirsty() && sensor.targetMask == LayerMask.GetMask("None"))
                            {
                                sensor.targetMask = LayerMask.GetMask("Drink");
                                status = Status.SEARCHINGDRINK;
                            }
                            else if(mating.enableMating && sensor.targetMask == LayerMask.GetMask("None"))   // TODO: ne konkrét határok, hanem % legyen
=======
                            else
>>>>>>> Stashed changes
                            {
                                setTargetLayerToMate();
                                status = Status.SEARCHINGMATE;
                            }
<<<<<<< Updated upstream
                            if (targetRef != null)
                            {
                                prevStatus = status;
                                status = Status.MOVING;
                            }
=======
                        }
                        if (targetRef != null)
                        {
                            (prevStatus, status) = (status, Status.MOVE_TOWARDS);
>>>>>>> Stashed changes
                        }
                        break;
                    }
                case Status.MOVING:
                    {
                        if (rest.ChanceToRest()) prevStatus = status = Status.REST;
                        if (targetRef != null)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
<<<<<<< Updated upstream
                            if (prevStatus == Status.SEARCHINGMATE && distanceToTarget < 7f)
=======
                            if (prevStatus == Status.SEARCH_MATE && distanceToTarget < 7f)      // TODO: allat meret fuggo
>>>>>>> Stashed changes
                            {
                                Animal mate = targetRef.GetComponent<Animal>();
                                if (mate != null)
                                {
<<<<<<< Updated upstream
                                    if(mate.status == Status.WAITING)
=======
                                    if(mate.status == Status.WAIT)
                                    {
>>>>>>> Stashed changes
                                        mate.mating.ToMate();
                                        (mate.status, mate.prevStatus) = (mate.prevStatus, Status.WANDER);
                                    }
                                    mating.ToMate();
                                    (status, prevStatus) = (prevStatus, Status.MATE);
                                }
                            }
                            if (distanceToTarget < 5f)
                            {
                                if (prevStatus == Status.SEARCHINGFOOD)
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
                case Status.RESTING:
                    {
                        if (breakCounter == 0)
                        {
                            status = prevStatus;
                        }
                        break;
                    }
<<<<<<< Updated upstream
                case Status.EATING:
                    {
                        if(breakCounter == 0)
                        {                            
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");
                            status = Status.WANDERING;
                        }
                        break;
                    }
                case Status.DRINKING:
                    {
                        if (breakCounter == 0)
                        {
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");
                            status = Status.WANDERING;
                        }
                        break;
                    }
                case Status.MATING:
=======
                case Status.EAT:
                case Status.DRINK:
                case Status.MATE:
>>>>>>> Stashed changes
                    {
                        if (targetRef == null)
                        {
<<<<<<< Updated upstream
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");                            
                            status = Status.WANDERING;
=======
                            sensor.targetMask = LayerMask.GetMask("None");
                            (prevStatus, status) = (status, Status.WANDER);
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
    protected abstract void setTargetLayerToMate();
    protected abstract void setTargetLayerToEat();
=======
>>>>>>> Stashed changes
}