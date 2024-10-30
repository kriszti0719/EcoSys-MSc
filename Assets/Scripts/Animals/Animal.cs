using Assets.Scripts.Animals.Common.Behaviour;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class Animal : MonoBehaviour
{
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

    public GameObject barsContainer;

    public int frameCounter = 0;
    public int stepCnt = 0;
    public int maxStepCnt = 8;
    public int decCnt = 0;
    public int maxDecCnt = 2;
    public int framesPerChange = 6; // 60 frame = 1 másodperc, ha 60 FPS


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
;
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
        drink.drying = Random.Range(25, 35);
        eat.starving = Random.Range(15, 25);
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
            //if (movement.isWandering)
            //{
            //    movement.WanderTo();
            //}

            frameCounter = 0;
        }
    }
    public void Step()
    {
        stepCnt++;
        if (stepCnt == maxStepCnt)   
        {
            if (transform.localPosition.y < 20 && targetRef == null)
            {
                oxygen--;
            }
            else
            {
                oxygen = maxOxygen;
            }

            if (breakCounter != 0)
                breakCounter--;

            if (reproduction.isStillPregnant())
            {
                reproduction.StepPregnancy();
            }

            switch (status)
            {
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
                case Status.SEARCHINGMATE:
                    {
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
                        if (targetRef != null)
                        {
                            Animal tmp = targetRef.GetComponent<Animal>();
                            tmp.reproduction = targetRef.GetComponent<Reproduction>();
                            if (tmp != null && tmp.isMale == this.isMale)
                            {
                                this.noTargetRefs.Add(targetRef);
                                this.targetRef = null;
                            }
                            else
                            {
                                if (tmp != null && tmp.mating.isAcceptable(this))
                                {
                                    reproduction.mate = tmp;
                                    tmp.reproduction.mate = this;
                                    prevStatus = status;
                                    status = Status.MOVING;
                                }
                                else
                                {
                                    this.noTargetRefs.Add(targetRef);
                                    this.targetRef = null;
                                }
                            }
                        }
                        break;
                    }
                case Status.SEARCHINGFOOD:
                case Status.SEARCHINGDRINK:
                case Status.WANDERING:
                    {
                        rest.ChanceToRest();
                        if(!reproduction.isFertile)
                        {
                            if (eat.currentHunger < drink.currentThirst && sensor.targetMask == LayerMask.GetMask("None"))
                            {
                                setTargetLayerToEat();
                                status = Status.SEARCHINGFOOD;
                            }
                            else if (sensor.targetMask == LayerMask.GetMask("None"))
                            {
                                sensor.targetMask = LayerMask.GetMask("Drink");
                                status = Status.SEARCHINGDRINK;
                            }
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
                            {
                                setTargetLayerToMate();
                                status = Status.SEARCHINGMATE;
                            }
                            if (targetRef != null)
                            {
                                prevStatus = status;
                                status = Status.MOVING;
                            }
                        }
                        break;
                    }
                case Status.MOVING:
                    {
                        rest.ChanceToRest();
                        if (targetRef != null)
                        {
                            float distanceToTarget = Vector3.Distance(transform.position, targetRef.transform.position);
                            if (prevStatus == Status.SEARCHINGMATE && distanceToTarget < 7f)
                            {
                                Animal mate = targetRef.GetComponent<Animal>();
                                if (mate != null)
                                {
                                    if(mate.status == Status.WAITING)
                                        mate.mating.ToMate();
                                    mating.ToMate();
                                }
                            }
                            if (distanceToTarget < 5f)
                            {
                                if (prevStatus == Status.SEARCHINGFOOD)
                                {
                                    eat.StartEating();
                                }
                                else if (sensor.targetMask == LayerMask.GetMask("Drink"))
                                {
                                    drink.StartDrinking();
                                }
                            }
                        }
                        else
                            status = prevStatus;
                        break;
                    }
                case Status.RESTING:
                    {
                        if (breakCounter == 0 || eat.isStarving() || drink.isDrying())
                        {
                            status = prevStatus;
                        }
                        break;
                    }
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
                    {
                        if (breakCounter == 0)
                        {
                            targetRef = null;
                            sensor.targetMask = LayerMask.GetMask("None");                            
                            status = Status.WANDERING;
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
    protected abstract void setTargetLayerToMate();
    protected abstract void setTargetLayerToEat();
}