using System;

public class UtilityGenome
{
    public float hungerWeight;
    public float thirstWeight;
    public float energyWeight;
    public float mateWeight;

    public UtilityGenome()
    {
        hungerWeight = UnityEngine.Random.Range(0.5f, 1.5f);
        thirstWeight = UnityEngine.Random.Range(0.5f, 1.5f);
        energyWeight = UnityEngine.Random.Range(0.5f, 1.5f);
        mateWeight = UnityEngine.Random.Range(0.5f, 1.5f);
    }

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
