using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Status
{
    WANDER,

    FLEE,
    WAIT,
    REST,

    SEARCH_FOOD,
    SEARCH_DRINK,
    SEARCH_MATE,

    MOVE_TOWARDS,

    DIE,
    CAUGHT
    }