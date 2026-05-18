using UnityEngine;

public class UtilityDecision
{
    private UtilityGenome genome;

    public UtilityDecision(UtilityGenome genome)
    {
        this.genome = genome;
    }

    public Status Decide(Animal a)
    {
        float hungerNormalized = 1f - (a.eat.currentHunger / 100f);     
        float thirstNormalized = 1f - (a.drink.currentThirst / 100f);   
        float energyNormalized = 1f - (a.rest.currentStamina / a.rest.maxStamina); 
        float mateNormalized = a.reproduction.reproductiveUrge / 100f;  

        float eatU = genome.hungerWeight * hungerNormalized;
        float drinkU = genome.thirstWeight * thirstNormalized;
        float restU = genome.energyWeight * energyNormalized;
        float mateU = genome.mateWeight * mateNormalized;

        float max = Mathf.Max(eatU, drinkU, restU, mateU);

        if (max == restU) return Status.REST;

        if (max == eatU) a.setTargetLayerToEat();
        else if (max == drinkU) a.sensor.targetMask = LayerMask.GetMask("Drink");
        else a.setTargetLayerToMate();

        return Status.SEARCH;
    }

}