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
        float hungerNormalized = 1f - (a.eat.currentHunger / (float)a.eat.maxHunger);     
        float thirstNormalized = 1f - (a.drink.currentThirst / (float)a.drink.maxThirst);   
        float energyNormalized = 1f - (a.rest.currentStamina / a.rest.maxStamina); 
        float mateNormalized = 1f - (a.mating.currentMatingUrge / (float)a.mating.maxMatingUrge);  

        float eatU = genome.hungerWeight * hungerNormalized;
        float drinkU = genome.thirstWeight * thirstNormalized;
        float restU = genome.energyWeight * energyNormalized;
        float mateU = genome.mateWeight * mateNormalized;

        float max = Mathf.Max(eatU, drinkU, restU, mateU);

        if (max == restU) return Status.REST;

        if (max == eatU) return Status.SEARCH_FOOD;
        if (max == drinkU) return Status.SEARCH_DRINK;
        
        return Status.SEARCH_MATE;
    }

}