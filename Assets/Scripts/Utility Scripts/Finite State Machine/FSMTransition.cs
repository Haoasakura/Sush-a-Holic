using System;

public class FSMTransition
{
    public Action[] Actions;
    public Func<bool> Condition;
    
    public FSMTransition(Action[] Actions, Func<bool> Condition=null) {
        this.Actions = Actions;
        this.Condition = Condition;
    }

    public void Fire() {
        if (Actions != null)
            foreach (Action Action in Actions)
                Action();
    }
}
