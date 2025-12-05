public static class Mutation
{
    public static float rate = 0.1f;
    public static float strength = 0.2f;
    public static void MutateFCM(FCMGenome g)
    {
        for (int i = 0; i < g.weights.Length; i++)
            g.weights[i] = Mut(g.weights[i]);
    }

    private static float Mut(float value)
    {
        if (UnityEngine.Random.value < rate)
            return value + UnityEngine.Random.Range(-strength, strength);
        return value;
    }
}
