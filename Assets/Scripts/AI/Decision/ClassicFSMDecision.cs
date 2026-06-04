public class ClassicFSMDecision
{
    public Status Decide(Animal a)
    {
        if (a.rest.ChanceToRest())
        {
            return Status.REST;
        }

        if (a.eat.currentHunger < a.drink.currentThirst && a.eat.IsHungry())
        {
            return Status.SEARCH_FOOD;
        }
        else if (!a.mating.enableMating || a.drink.IsThirsty())
        {
            return Status.SEARCH_DRINK;
        }
        else
        {
            return Status.SEARCH_MATE;
        }
    }
}