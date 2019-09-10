using System.Collections;
using System.Collections.Generic;

public class FSM {

    public FSMState currentState;

    public FSM(FSMState initialState) {
        currentState = initialState;
        currentState.Enter();
    }

    public void Update() {
        FSMTransition transition = currentState.VerifyTransition();
        if(transition!=null) {
            currentState.Exit();
            transition.Fire();
            currentState.NextState(transition);
            currentState.Enter();
        }
        else {
            currentState.Stay();
        }
    }
}
