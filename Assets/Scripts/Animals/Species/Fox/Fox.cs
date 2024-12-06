using UnityEngine;
using Assets.Scripts.Datatypes;

public class Fox : Animal
{
    protected override void Start()
    {
        base.Start();
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
