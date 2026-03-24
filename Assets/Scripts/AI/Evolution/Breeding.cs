public static class Breeding
{
    public static UtilityGenome BreedUtility(UtilityGenome m, UtilityGenome f)
    {
        UtilityGenome child = new UtilityGenome();
        child.hungerWeight = Average(m.hungerWeight, f.hungerWeight);
        child.thirstWeight = Average(m.thirstWeight, f.thirstWeight);
        child.energyWeight = Average(m.energyWeight, f.energyWeight);
        child.mateWeight = Average(m.mateWeight, f.mateWeight);

        Mutation.MutateUtility(child);
        return child;
    }

    public static FCMGenome BreedFCM(FCMGenome m, FCMGenome f)
    {
        FCMGenome child = new FCMGenome(m.weights.Length);

        for (int i = 0; i < m.weights.Length; i++)
            child.weights[i] = Average(m.weights[i], f.weights[i]);

        Mutation.MutateFCM(child);
        return child;
    }

    static float Average(float a, float b) => (a + b) * 0.5f;
}
