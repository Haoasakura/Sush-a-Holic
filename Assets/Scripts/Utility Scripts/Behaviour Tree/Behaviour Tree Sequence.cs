
public class BehaviourTreeSequence : BehaviourTreeComposite {

    public BehaviourTreeSequence(BehaviourTreeTask[] tasks): base(tasks) { }

    public override int Run() {
        while (index < tasks.Length) {
            int result = tasks[index].Run();
            if (result == -1)
                return -1;
            if (result == 0) {
                index=0;
                return 0;
            }
            if (result == 1) {
                index++;
                return -1;
            }
        }
        index = 0;
        return 1;
    }
}
