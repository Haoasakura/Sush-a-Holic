using System;

public class DecisionTreeAction : DecisionTreeNode {

    public Func<object> Action;

    public DecisionTreeAction(Func<object> Action) {
        this.Action = Action;
    }

    public DecisionTreeAction Walk() {
        return this;
    }
}
