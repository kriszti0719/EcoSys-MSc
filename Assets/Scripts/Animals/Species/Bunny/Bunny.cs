using Assets.Scripts;
using Assets.Scripts.Datatypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bunny : Animal, Edible
{
    private int eatDuration = 3;
    private int nutrition = 20;
    public int isCaught = 0;
    protected override void Start()
    {
        base.Start();
    }
    public override void SetTraits(float rnd)
    {
        SetComponents();

        aging.setAging(1, rnd, Random.Range(4f, 6f));
        movement.moveSpeed = Random.Range(3f, 8f);
        sensor.radius = Random.Range(30, 70);
        reproduction.setReproduction(Random.Range(30, 50),
                                    Random.Range(30, 60),
                                    true);
        mating.enableMating = true;
        mating.charm = Random.Range(20, 100);
        drink.drying = Random.Range(30, 40);
        eat.starving = Random.Range(20, 40);

        setSpeciesSpecificTraits();
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
        if(isCaught > 0)
        {
            throw new System.Exception("Already CAUGHT: " + isCaught);
        }
        isCaught++;
        status = Status.CAUGHT;
        breakCounter = eatDuration; ;
    }
    public int getNutrition()
    {
        return nutrition;
    }
    public void Eaten()
    {
        cause = CauseOfDeath.CONSUMED;
        die.ToDie();
    }
    public override void setSpeciesSpecificTraits()
    {
        species = Species.BUNNY;
        predators = new List<Species>();
        predators.Add(Species.FOX);
    }
}
