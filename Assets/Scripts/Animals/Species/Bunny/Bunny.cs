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
            if (OnConsumed != null)
            {
                yield return new WaitForSeconds(1f);
                currentDuration--;
            }
            else
            {
                yield break;
            }
        }
        OnConsumed?.Invoke();
        Consumed();
    }
    private void Consumed()
    {
        die.Destroy();
    }
    public bool DamageHealth(int damage = 10)
    {
        // --- Damage roll ---
        float roll = UnityEngine.Random.Range(0.75f, 1.25f);
        int actualDamage = Mathf.RoundToInt(damage * roll);

        currentHealth = Mathf.Max(0, currentHealth - actualDamage);

        if (currentHealth == 0)
        {
            AboutToBeConsumed();
            return true;
        }

        return false;
    }

}
