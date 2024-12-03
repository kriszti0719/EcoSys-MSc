using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

public enum CauseOfDeath
{
    NONE,
    HUNGER,
    THIRST,
    EATEN,
    AGE,
    DROWN,
    CONSUMED
}

public static class CauseOfDeathExtensions
{
    public static string ToPrint(this CauseOfDeath cause)
    {
        return cause switch
        {
            CauseOfDeath.NONE => "None",
            CauseOfDeath.HUNGER => "Hunger",
            CauseOfDeath.THIRST => "Thirst",
            CauseOfDeath.EATEN => "Eaten",
            CauseOfDeath.AGE => "Age",
            CauseOfDeath.DROWN => "Drowned",
            CauseOfDeath.CONSUMED => "Consumed",
            _ => cause.ToString()
        };
    }
}
