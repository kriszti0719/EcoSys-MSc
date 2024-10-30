using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : Animal
{
    protected override void setTargetLayerToEat()
    {
        sensor.targetMask = LayerMask.GetMask("BunnyFood");
    }

    protected override void setTargetLayerToMate()
    {
        sensor.targetMask = LayerMask.GetMask("Bunny");
    }

    protected override void Start()
    {
        base.Start();
    }
}
