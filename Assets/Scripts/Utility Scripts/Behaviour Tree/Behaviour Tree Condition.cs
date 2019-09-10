using System;

public class BehaviourTreeCondition : BehaviourTreeTask {

    public Func<bool> BTCall;

    public BehaviourTreeCondition(Func<bool> Condition) {
        this.BTCall = Condition;
    }

    public int Run() {
        return BTCall() ? 1 : 0;
    }
}
