using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bunny : Animal, IEdible
{
    private int eatDuration = 5;
    private int nutrition = 20;
    public event Action OnConsumed;

    protected override void Start()
    {
        base.Start();
    }
    public override int getTargetLayerToMate()
    {
        return LayerMask.GetMask("Bunny");
    }
    public override int getTargetLayerToEat()
    {
        return LayerMask.GetMask("BunnyFood");
    }
    public override void setTargetLayerToEat()
    {
        sensor.targetMask = LayerMask.GetMask("BunnyFood");
    }
    public override void setTargetLayerToMate()
    {
        sensor.targetMask = LayerMask.GetMask("Bunny");
    }
    public override int getPredatorLayers()
    {
        return predators.ElementAt(0).ToLayer();
    }
    public int getNutrition()
    {
        return nutrition;
    }
    public override void setSpeciesSpecificTraits()
    {
        species = Species.BUNNY;
        predators = new List<Species>();
        predators.Add(Species.FOX);
    }
    public void ToBeConsumed()
    {
        (prevStatus, status) = (status, Status.CAUGHT);
        Invoke(nameof(Consumed), eatDuration);
    }
    private void Consumed()
    {
        OnConsumed?.Invoke();
        die.HandleDeath(CauseOfDeath.CONSUMED);
    }
}
