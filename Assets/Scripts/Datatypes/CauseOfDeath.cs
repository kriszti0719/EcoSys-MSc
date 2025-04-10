using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
public enum CauseOfDeath
{
    NONE,
    DROWN,
    HUNGER,
    THIRST,
    AGE,
    CONSUMED
}

public static class CauseOfDeathExtensions
{
    public static string ToPrint(this CauseOfDeath cause)
    {
        return cause switch
        {
            CauseOfDeath.NONE => "None",
            CauseOfDeath.DROWN => "Drowned",
            CauseOfDeath.HUNGER => "Hunger",
            CauseOfDeath.THIRST => "Thirst",
            CauseOfDeath.AGE => "Age",
            CauseOfDeath.CONSUMED => "Eaten",
            _ => cause.ToString()
        };
    }
}
