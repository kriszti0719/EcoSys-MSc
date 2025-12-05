using System.Collections.Generic;
using UnityEngine;

public class FCMModel
{
    private Dictionary<string, FCMNode> nodes = new Dictionary<string, FCMNode>();
    private List<FCMLink> links = new List<FCMLink>();

    public FCMNode AddNode(string name)
    {
        var n = new FCMNode(name);
        nodes[name] = n;
        return n;
    }

    public void AddLink(FCMNode from, FCMNode to, float weight)
    {
        links.Add(new FCMLink(from, to, weight));
    }

    public void SetInput(string name, float value)
    {
        nodes[name].value = Mathf.Clamp01(value);
    }

    public float GetOutput(string name)
    {
        return Mathf.Clamp01(nodes[name].value);
    }

    public void Update()
    {
        foreach (var node in nodes.Values)
            node.nextValue = 0f;

        foreach (var link in links)
            link.to.nextValue += link.from.value * link.weight;

        foreach (var node in nodes.Values)
            node.value = Mathf.Clamp01(Sigmoid(node.nextValue));
    }

    private float Sigmoid(float x)
    {
        return 1f / (1f + Mathf.Exp(-x));
    }
}
