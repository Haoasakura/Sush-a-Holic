using System;
using System.Collections.Generic;

public class DecisionTreeDecision : DecisionTreeNode {

    private Func<object> Selector;
    private Dictionary<object, DecisionTreeNode> links;

    public DecisionTreeDecision(Func<object> Selector) {
        this.Selector = Selector;
        links = new Dictionary<object, DecisionTreeNode>();
    }

    public void AddLink (object value, DecisionTreeNode next) {
        links.Add(value, next);
    }

    public DecisionTreeAction Walk() {
        object obj = Selector();
        return links.ContainsKey(obj) ? links[obj].Walk() : null;
    }
}
