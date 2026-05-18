using UnityEngine;

public class EventHandler : MonoBehaviour
{
    private Animal animal;
    void Start() {  animal = GetComponent<Animal>(); }
    public void HandleDrowning()
    {
        animal.die.HandleDeath(CauseOfDeath.DROWN);
    }
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
        if (animal.status == Status.EAT) animal.eat.FinishEating();
        animal.breakCounter = 0;
        HandleBreakEnded();
    }
    public void HandleHungerDepleted()
    {
        animal.die.HandleDeath(CauseOfDeath.HUNGER);
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
        if (animal.status == Status.DRINK) animal.drink.FinishDrinking();
        animal.breakCounter = 0;
        HandleBreakEnded();
    }
    public void HandleThirstDepleted()
    {
        animal.die.HandleDeath(CauseOfDeath.THIRST);
    }
    public void HandleRestFull()
    {
        if (animal.status == Status.REST)
        {
            animal.breakCounter = 0;
            HandleBreakEnded();
        }
    }
    public void HandleRestDepleted()
    {
        animal.prevStatus = animal.status = Status.REST;
    }
    public void HandleAgeLimitReached()
    {
        animal.die.HandleDeath(CauseOfDeath.AGE);
    }
    public void HandleFoodConsumed()
    {
        animal.targetRef = null;
        animal.sensor.targetMask = LayerMask.GetMask("None");
        (animal.prevStatus, animal.status) = (animal.status, Status.WANDER);
    }
    public void HandleBreakEnded()
    {
        switch (animal.status)
        {
            case Status.REST:
            {
                animal.status = animal.prevStatus;
                break;
            }
            case Status.EAT:
            case Status.DRINK:
                {
                    animal.targetRef = null;
                    animal.sensor.targetMask = LayerMask.GetMask("None");
                    (animal.prevStatus, animal.status) = (animal.status, Status.WANDER);
                    break;
                }
            case Status.MATE:
                {
                    animal.targetRef = null;
                    animal.sensor.targetMask = LayerMask.GetMask("None");
                    (animal.prevStatus, animal.status) = (animal.status, Status.WANDER);
                    animal.mating.IsSuccess();
                    break;
                }
        }

    }
}
