using Assets.Scripts.Animals.Common.Behaviour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public abstract class EventHandler : MonoBehaviour
{
    private Animal animal;
    void Start() {  animal = GetComponent<Animal>(); }
    public void HandleHungerCritical()
    {
        if (animal.status == Status.SEARCH_MATE || animal.status == Status.REST)
        {
            animal.setTargetLayerToEat();
            animal.status = Status.SEARCH_FOOD;
        }
    }
    public void HandleHungerFull()
    {
        animal.breakCounter = 0;
        if (animal.status == Status.EAT) animal.eat.FinishEating();
    }
    public void HandleHungerDepleted()
    {
        if (animal.status != Status.DIE)
        {
            animal.cause = CauseOfDeath.HUNGER;
            (animal.prevStatus, animal.status) = (animal.status, Status.DIE);
        }
    }
    public void HandleThirstCritical()
    {
        if (animal.status == Status.SEARCH_MATE || animal.status == Status.REST)
        {
            animal.sensor.targetMask = LayerMask.GetMask("Drink");
            animal.status = Status.SEARCH_DRINK;
        }
    }
    public void HandleThirstFull()
    {
        animal.breakCounter = 0;
        if (animal.status == Status.EAT) animal.drink.FinishDrinking();
    }
    public void HandleThirstDepleted()
    {
        if (animal.status != Status.DIE)
        {
            animal.cause = CauseOfDeath.THIRST;
            (animal.prevStatus, animal.status) = (animal.status, Status.DIE);
        }
    }
    public void HandleRestFull()
    {
        if (animal.status == Status.REST)
            animal.breakCounter = 0;
    }
    public void HandleRestDepleted()
    {
        animal.prevStatus = animal.status = Status.REST;
    }
    public void HandleAgeLimitReached()
    {
        if (animal.status != Status.DIE)
        {
            animal.cause = CauseOfDeath.AGE;
            (animal.prevStatus, animal.status) = (animal.status, Status.DIE);
        }
    }

    public void HandleFoodConsumed()
    {
        animal.targetRef = null;
        animal.sensor.targetMask = LayerMask.GetMask("None");
        (animal.prevStatus, animal.status) = (animal.status, Status.WANDER);
    }
}
