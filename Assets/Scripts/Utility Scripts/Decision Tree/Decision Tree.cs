using System;
using System.Collections.Generic;

public class DecisionTree {

    private DecisionTreeNode root;

    public DecisionTree (DecisionTreeNode root) {
        this.root = root;
    }

    public object Walk() {
        DecisionTreeAction result = root.Walk();
        if (result != null)
            return result.Action();
        return null;
    }
}
