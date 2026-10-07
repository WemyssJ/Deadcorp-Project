using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }

    public void InitializeDefaultState(EnemyState startState)
    {
        CurrentState = startState;
        CurrentState.StateEnter();
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentState.StateExit();
        CurrentState = newState;
        CurrentState.StateEnter();
    }
}
