using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum Species
{
    NONE,
    BUNNY,
    FOX
}

public static class SpeciesExtensions
{
    public static string ToPrint(this Species cause)
    {
        return cause switch
        {
            Species.NONE => "None",
            Species.BUNNY => "Bunny",
            Species.FOX => "Fox",
            _ => cause.ToString()
        };
    }
}