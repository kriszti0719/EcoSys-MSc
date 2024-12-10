using UnityEngine;
using Assets.Scripts.Datatypes;

public class Fox : Animal
{
    protected override void Start()
    {
        base.Start();
    }
    public override void SetTraits(float rnd)
    {
        SetComponents();

        aging.setAging(1, rnd, Random.Range(4f, 6f));
        movement.moveSpeed = Random.Range(5f, 10f);
        sensor.radius = Random.Range(50, 90);
        reproduction.setReproduction(Random.Range(35, 45),
                                    Random.Range(30, 60),
                                    true);
        mating.enableMating = true;
        mating.charm = Random.Range(50, 100);
        drink.drying = Random.Range(30, 40);
        eat.starving = Random.Range(25, 45);

        setSpeciesSpecificTraits();
    }
    public override int getTargetLayerToMate()
    {
        return LayerMask.GetMask("Fox"); ;
    }
    public override int getTargetLayerToEat()
    {
        return LayerMask.GetMask("Bunny"); ;
    }
    protected override void setTargetLayerToEat()
    {
        sensor.targetMask = LayerMask.GetMask("Bunny");
    }
    public override void setTargetLayerToMate()
    {
        sensor.targetMask = LayerMask.GetMask("Fox");
    }
    public override int getPredatorLayers()
    {
        return -1;
    }

    public override void setSpeciesSpecificTraits()
    {
        species = Species.FOX;
    }
}
