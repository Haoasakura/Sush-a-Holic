
public class BehaviourTree {

    public BehaviourTreeTask root;

    public BehaviourTree(BehaviourTreeTask root) {
        this.root = root;
    }

    public bool Step() {
        return root.Run() < 0 ? true : false;
    }


}
