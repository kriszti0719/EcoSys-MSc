using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CauseOfDeath
{
    NONE,
    DROWN,
    HUNGER,
    THIRST,
    AGE,
<<<<<<< Updated upstream
    DROWN
}
=======
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
>>>>>>> Stashed changes
