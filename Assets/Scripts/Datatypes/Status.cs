using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status
{
<<<<<<< Updated upstream
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
    DIE
=======
    WANDER,

    FLEE,
    WAIT,
    REST,

    SEARCH_FOOD,
    SEARCH_DRINK,
    SEARCH_MATE,

    MOVE_TOWARDS,

    EAT,
    DRINK,
    MATE,

    DIE,
    CAUGHT
>>>>>>> Stashed changes
}