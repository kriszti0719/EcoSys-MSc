using UnityEngine;

public class AnimalEventHandler : MonoBehaviour
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
        animal.status = animal.prevStatus;
        
        if (animal.status == Status.SEARCH_FOOD || animal.status == Status.SEARCH_DRINK || animal.status == Status.SEARCH_MATE || animal.status == Status.WANDER) 
            animal.prevStatus = Status.WANDER;
        else if (animal.status == Status.MOVE_TOWARDS) 
            animal.prevStatus = Status.SEARCH_MATE; // Alapértelmezett, majd a Decide korrigálja

    }
    public void HandleHungerCritical()
    {
        if (IsSearchable(animal.status) || animal.status == Status.REST)
        {
            animal.setTargetLayerToEat();
            animal.status = Status.SEARCH_FOOD;
        }
    }
    public void HandleHungerDepleted()
    {
        animal.die.HandleDeath(CauseOfDeath.HUNGER);
    }
    public void HandleThirstCritical()
    {
        if (IsSearchable(animal.status) || animal.status == Status.REST)
        {
            animal.sensor.targetMask = LayerMask.GetMask("Drink");
            animal.status = Status.SEARCH_DRINK;
        }
    }
    public void HandleThirstDepleted()
    {
        animal.die.HandleDeath(CauseOfDeath.THIRST);
    }
    public void HandleRestDepleted()
    {
        (animal.prevStatus, animal.status) = (animal.status, Status.REST);
    }
    public void HandleTargetSpotted()
    {
        if (IsSearchable(animal.status) || animal.status == Status.WANDER)
        {
            (animal.prevStatus, animal.status) = (animal.status, Status.MOVE_TOWARDS);
        }
    }

    private bool IsSearchable(Status s)
    {
        return s == Status.SEARCH_FOOD || s == Status.SEARCH_DRINK || s == Status.SEARCH_MATE;
    }
}
