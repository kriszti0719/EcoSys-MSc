public static class FCMFactory
{
    public static FCMModel Create(FCMGenome genome)
    {
        FCMModel model = new FCMModel();

        var hunger = model.AddNode("Hunger");
        var thirst = model.AddNode("Thirst");
        var energy = model.AddNode("Energy");
        var fear = model.AddNode("Fear");
        var mate = model.AddNode("Mate");

        var eat = model.AddNode("Eat");
        var drink = model.AddNode("Drink");
        var rest = model.AddNode("Rest");
        var flee = model.AddNode("Flee");
        var mateAct = model.AddNode("MateAct");

        int idx = 0;
        model.AddLink(hunger, eat, genome.weights[idx++]);
        model.AddLink(thirst, drink, genome.weights[idx++]);
        model.AddLink(energy, rest, genome.weights[idx++]);
        model.AddLink(fear, flee, genome.weights[idx++]);
        model.AddLink(mate, mateAct, genome.weights[idx++]);

        model.AddLink(fear, eat, genome.weights[idx++]);
        model.AddLink(energy, mateAct, genome.weights[idx++]);
        model.AddLink(hunger, rest, genome.weights[idx++]);

        return model;
    }

    public static int GetLinkCount()
    {
        return 8; //TODO: How about a head-count?
    }
}
