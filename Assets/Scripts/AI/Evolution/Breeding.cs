public static class Breeding
{
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
