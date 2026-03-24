using Assets.Scripts.Animals.Common.Behaviour;
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
    public int eat_critical;
    public int drink_critical;
    public float moveSpeed;
    public int radius;
    public int reproductiveUrge;
    public int pregnancyDuration;
    public int charm;
    public int camouflage;
    public int stealth;
    public int bravery;

    public MateTraits(Animal animal)
    {
        this.size = animal.aging.size;
        this.lifeSpan = animal.aging.lifeSpan;
        this.eat_critical = animal.eat.critical;
        this.drink_critical = animal.drink.critical;
        this.moveSpeed = animal.movement.moveSpeed;
        this.radius = animal.sensor.radius;
        this.reproductiveUrge = animal.reproduction.reproductiveUrge;
        this.pregnancyDuration = animal.reproduction.pregnancyDuration;
        this.charm = animal.mating.charm;
        this.camouflage = animal.sensor.camouflage;
        this.stealth = animal.sensor.stealth;
        this.bravery = animal.bravery;
    }
}