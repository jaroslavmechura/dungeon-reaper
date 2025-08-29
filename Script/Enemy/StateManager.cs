using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State currState;

    private void Update()
    {
        RunStateMachine();
    }

    private void RunStateMachine() {
        State nextState = currState?.RunCurrState();

        if (nextState != null)
        {
            SwitchNextState(nextState);
        }
    }

    private void SwitchNextState(State nextState) 
    {
        currState = nextState;
    }
}
