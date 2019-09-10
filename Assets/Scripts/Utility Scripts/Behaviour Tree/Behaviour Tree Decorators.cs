using System;

public class BehaviourTreeDecoratorFilter : BehaviourTreeDecorator
{
    private Func<bool> Condition;

    public BehaviourTreeDecoratorFilter(Func<bool> Condition, BehaviourTreeTask task) : base(task) {
        this.Condition = Condition;
    }

    public override int Run() {
        return Condition() ? Task.Run() : 0;
    }
}

public class BehaviourTreeDecoratorLimit : BehaviourTreeDecorator
{
    public int maxRepetition;
    public int count;

    public BehaviourTreeDecoratorLimit(int maxRepetition, BehaviourTreeTask task) : base(task) {
        this.maxRepetition = maxRepetition;
        count = 0;
    }

    public override int Run() {
        if (count >= maxRepetition)
            return 0;
        int result = Task.Run();
        if (result != -1)
            count++;
        return result;
    }
}

public class BehaviourTreeDecoratorUntilFail : BehaviourTreeDecorator
{
    public BehaviourTreeDecoratorUntilFail(BehaviourTreeTask task) : base(task) { }

    public override int Run() {
        if (Task.Run() != 0)
            return -1;
        return 1;
    }
}

public class BehaviourTreeDecoratorInverter : BehaviourTreeDecorator
{
    public BehaviourTreeDecoratorInverter(BehaviourTreeTask task) : base(task) { }

    public override int Run() {
        int result = Task.Run();
        if (result == 0 || result == 1)
            return 1 - result;

        return result;
    }
}
