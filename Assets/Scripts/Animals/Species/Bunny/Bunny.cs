using Assets.Scripts;
using Assets.Scripts.Datatypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bunny : Animal, Edible
{
    private int eatDuration = 5;
    private int nutrition = 20;
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
    protected override void setTargetLayerToEat()
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

    public int getEatDuration()
    {
        return eatDuration;
    }

    public void setStatusCaught()
    {
        status = Status.CAUGHT;
        breakCounter = eatDuration; ;
    }

    public int getNutrition()
    {
        return nutrition;
    }

    public void Eaten()
    {

    }
    public override void setSpeciesSpecificTraits()
    {
        species = Species.BUNNY;
        predators = new List<Species>();
        predators.Add(Species.FOX);
    }
}
