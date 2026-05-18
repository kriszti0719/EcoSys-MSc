using UnityEngine;

public class EventHandler : MonoBehaviour
{
    private Animal animal;
    void Start() {  animal = GetComponent<Animal>(); }
    public void HandleDrowning()
    {
        animal.die.HandleDeath(CauseOfDeath.DROWN);
    }
    public void HandleAgeLimitReached()
    {
        animal.die.HandleDeath(CauseOfDeath.AGE);
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
        }

    }
    public void HandleHungerCritical()
    {
        if (animal.status == Status.SEARCH || animal.status == Status.REST)
        {
            animal.setTargetLayerToEat();
            animal.status = Status.SEARCH;
        }
    }
    public void HandleHungerDepleted()
    {
        animal.die.HandleDeath(CauseOfDeath.HUNGER);
    }
    public void HandleThirstCritical()
    {
        if (animal.status == Status.SEARCH || animal.status == Status.REST)
        {
            animal.sensor.targetMask = LayerMask.GetMask("Drink");
            animal.status = Status.SEARCH;
        }
    }
    public void HandleThirstDepleted()
    {
        animal.die.HandleDeath(CauseOfDeath.THIRST);
    }
    public void HandleRestFull()
    {
        animal.rest.breakCounter = 0;
        HandleBreakEnded();
    }
    public void HandleRestDepleted()
    {
        (animal.prevStatus, animal.status) = (animal.status, Status.REST);
    }
}
