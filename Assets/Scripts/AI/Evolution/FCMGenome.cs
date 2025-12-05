using System;
public class FCMGenome
{
    public float[] weights;

    public FCMGenome(int linkCount)
    {
        weights = new float[linkCount];
        for (int i = 0; i < linkCount; i++)
            weights[i] = UnityEngine.Random.Range(-1f, 1f);
    }

    public FCMGenome Clone()
    {
        FCMGenome g = new FCMGenome(weights.Length);
        Array.Copy(weights, g.weights, weights.Length);
        return g;
    }
}
