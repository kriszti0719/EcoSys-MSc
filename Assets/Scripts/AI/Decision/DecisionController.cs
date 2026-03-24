public class DecisionController
{
    public DecisionMode mode;
    public ClassicFSMDecision classic;
    public UtilityDecision utility;
    public FCMDecision fcm;

    public Status Decide(Animal a)
    {
        switch (mode)
        {
            case DecisionMode.ClassicFSM: return classic.Decide(a);
            case DecisionMode.Utility: return utility.Decide(a);
            case DecisionMode.FCM: return fcm.Decide(a);
            default: return Status.WANDER;
        }
    }
}