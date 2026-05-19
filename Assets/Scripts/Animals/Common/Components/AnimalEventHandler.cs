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
        
        if (animal.status == Status.SEARCH || animal.status == Status.WANDER) animal.prevStatus = Status.WANDER;
        else if (animal.status == Status.MOVE_TOWARDS) animal.prevStatus = Status.SEARCH;

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
    public void HandleRestDepleted()
    {
        (animal.prevStatus, animal.status) = (animal.status, Status.REST);
    }
    public void HandleTargetSpotted()
    {
        if (animal.status == Status.SEARCH || animal.status == Status.WANDER)
        {
            (animal.prevStatus, animal.status) = (animal.status, Status.MOVE_TOWARDS);
        }
    }
}
