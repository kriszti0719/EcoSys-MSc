using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status
{
    // Moving vs. Wandering
    //  - moving: when it moves towards to a specific stuff,
    //  - wandering: when there's no target, just moving around
    RESTING,
    MOVING,
    WANDERING,
    DRINKING,
    EATING,
    MATING,
    WAITING,
    SEARCHINGFOOD,
    SEARCHINGDRINK,
    SEARCHINGMATE,
    DIE,
    CAUGHT
}