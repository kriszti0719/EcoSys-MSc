using System;

public class UtilityGenome
{
    public float hungerWeight = 1f;
    public float thirstWeight = 1f;
    public float energyWeight = 1f;
    public float mateWeight = 1f;

    public UtilityGenome Clone()
    {
        return new UtilityGenome
        {
            hungerWeight = this.hungerWeight,
            thirstWeight = this.thirstWeight,
            energyWeight = this.energyWeight,
            mateWeight = this.mateWeight
        };
    }
}
