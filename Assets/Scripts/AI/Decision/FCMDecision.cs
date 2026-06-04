using System.Linq;
using UnityEngine;

public class FCMDecision
{
    private FCMModel model;

    public FCMDecision(FCMGenome genome)
    {
        model = FCMFactory.Create(genome);
    }

    public Status Decide(Animal a)
    {
        float hungerNormalized = 1f - (a.eat.currentHunger / 100f);
        float thirstNormalized = 1f - (a.drink.currentThirst / 100f);
        float energyNormalized = 1f - (a.rest.currentStamina / a.rest.maxStamina);
        float fearNormalized = a.spottedThreats.Any() ? 1f : 0f;
        float mateNormalized = 1f - (a.mating.currentMatingUrge / 100f);

        model.SetInput("Hunger", hungerNormalized);
        model.SetInput("Thirst", thirstNormalized);
        model.SetInput("Energy", energyNormalized);
        model.SetInput("Fear", fearNormalized);
        model.SetInput("Mate", mateNormalized);

        model.Update();

        float eat = model.GetOutput("Eat");
        float drink = model.GetOutput("Drink");
        float rest = model.GetOutput("Rest");
        float flee = model.GetOutput("Flee");
        float mate = model.GetOutput("MateAct");

        float max = Mathf.Max(eat, drink, rest, flee, mate);

        if (max == flee) return Status.FLEE;
        if (max == rest) return Status.REST;

        if (max == eat) return Status.SEARCH_FOOD;
        if (max == drink) return Status.SEARCH_DRINK;
        
        return Status.SEARCH_MATE;
    }
}