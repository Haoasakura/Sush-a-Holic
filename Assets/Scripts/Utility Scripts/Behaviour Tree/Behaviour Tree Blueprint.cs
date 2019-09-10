
public interface BehaviourTreeTask {

    int Run();
}

public abstract class BehaviourTreeComposite : BehaviourTreeTask
{
    protected BehaviourTreeTask[] tasks;
    protected int index;

    public BehaviourTreeComposite(BehaviourTreeTask[] Tasks) {
        this.tasks = Tasks;
        index = 0;
    }

    public abstract int Run();
}

public abstract class BehaviourTreeDecorator: BehaviourTreeTask
{
    public BehaviourTreeTask Task;

    public BehaviourTreeDecorator(BehaviourTreeTask Task) {
        this.Task = Task;
    }

    public abstract int Run();
}
