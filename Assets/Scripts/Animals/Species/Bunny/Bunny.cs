<<<<<<< Updated upstream
=======
using Assets.Scripts;
using Assets.Scripts.Datatypes;
using System;
>>>>>>> Stashed changes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< Updated upstream
public class Bunny : Animal
{
    protected override void setTargetLayerToEat()
=======
public class Bunny : Animal, IEdible
{
    private int eatDuration = 5;
    private int nutrition = 20;
    private int currentDuration;

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
>>>>>>> Stashed changes
    {
        sensor.targetMask = LayerMask.GetMask("BunnyFood");
    }

    protected override void setTargetLayerToMate()
    {
        sensor.targetMask = LayerMask.GetMask("Bunny");
    }
<<<<<<< Updated upstream

    protected override void Start()
    {
        base.Start();
=======
    public override int getPredatorLayers()
    {
        return predators.ElementAt(0).ToLayer();
    }
    public int getEatDuration()
    {
        return eatDuration;
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
>>>>>>> Stashed changes
    }
    public void AboutToBeConsumed()
    {
        cause = CauseOfDeath.CONSUMED;
        (prevStatus, status) = (status, Status.CAUGHT);
        currentDuration = eatDuration;
        StartCoroutine(ToBeConsumed());
    }
    public IEnumerator ToBeConsumed()
    {
        while (currentDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            currentDuration--;
        }
        OnConsumed?.Invoke();
        Consumed();
    }
    private void Consumed()
    {
        die.Destroy();
    }
}
