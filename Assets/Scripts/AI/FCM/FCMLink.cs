public class FCMLink
{
    public FCMNode from;
    public FCMNode to;
    public float weight;

    public FCMLink(FCMNode _from, FCMNode _to, float w)
    {
        from = _from;
        to = _to;
        weight = w;
    }
}