using System;
using System.Collections.Generic;

public class FSMState
{
    public Action[] enterActions;
    public Action[] stayActions;
    public Action[] exitActions;

    private Dictionary<FSMTransition, FSMState> links;

    public FSMState() {
        links = new Dictionary<FSMTransition, FSMState>();
    }

    public void AddTransition(FSMTransition transition, FSMState targetState) {
        links[transition] = targetState;
    }

    public FSMTransition VerifyTransition() {
        foreach(FSMTransition transition in links.Keys)
            if (transition.Condition())
                return transition;

        return null;
    }

    public FSMState NextState(FSMTransition transition) {
        return links[transition];
    }

    public void Enter() {
        foreach (Action action in enterActions)
            action();
    }

    public void Stay() {
        foreach (Action action in stayActions)
            action();
    }

    public void Exit() {
        foreach (Action action in exitActions)
            action();
    }
}
