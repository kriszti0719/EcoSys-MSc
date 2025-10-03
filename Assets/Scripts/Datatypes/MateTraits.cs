using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MateTraits
{
    public float size;
    public float lifeSpan;
    public int starving;
    public int drying;
    public float moveSpeed;
    public int radius;
    public int reproductiveUrge;
    public int pregnancyDuration;
    public int charm;

    public MateTraits(Animal animal)
    {
        this.size = animal.aging.size;
        this.lifeSpan = animal.aging.lifeSpan;
        this.starving = animal.eat.critical;
        this.drying = animal.drink.critical;
        this.moveSpeed = animal.movement.moveSpeed;
        this.radius = animal.sensor.radius;
        this.reproductiveUrge = animal.reproduction.reproductiveUrge;
        this.pregnancyDuration = animal.reproduction.pregnancyDuration;
        this.charm = animal.mating.charm;
    }
}