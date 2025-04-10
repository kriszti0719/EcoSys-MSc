using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum Species
{
    NONE,
    BUNNY,
    FOX
}

public static class SpeciesExtensions
{
    public static string ToPrint(this Species species)
    {
        return species switch
        {
            Species.NONE => "None",
            Species.BUNNY => "Bunny",
            Species.FOX => "Fox",
            _ => species.ToString()
        };
    }
    public static int ToLayer(this Species species)
    {
        return species switch
        {
            Species.NONE => -1,
            Species.BUNNY => LayerMask.GetMask("Bunny"),
            Species.FOX => LayerMask.GetMask("Fox"),
            _ => -1
        };
    }
}