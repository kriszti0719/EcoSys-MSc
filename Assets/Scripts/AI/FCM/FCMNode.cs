public class FCMNode
{
    public string name;
    public float value;
    public float nextValue;

    public FCMNode(string name, float initial = 0f)
    {
        this.name = name;
        value = initial;
    }
}