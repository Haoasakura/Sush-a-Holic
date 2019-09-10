using System;

public class BehaviourTreeAction : BehaviourTreeTask
{
    public Func<bool> Action;

    public BehaviourTreeAction (Func<bool> Action) {
        this.Action = Action;
    }

    public int Run() {
        return Action() ? 1 : 0;
    }
}
