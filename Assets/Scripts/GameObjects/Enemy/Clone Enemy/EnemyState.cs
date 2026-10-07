using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy _enemy;
    protected EnemyStateMachine _enemyStateMachine;

    protected EnemyStats _enemyStats;

    protected bool _isExitingState;

    public EnemyState(Enemy enemy, EnemyStateMachine stateMachine)
    {
        _enemy = enemy;
        _enemyStateMachine = stateMachine;

        _enemyStats = _enemy.EnemyStats;
    }

    public virtual void StateEnter()
    {
        _isExitingState = false;

        if (_enemy.ShowEnteredStateDebugLog)
            Debug.Log("Entered State: " + _enemyStateMachine.CurrentState);
    }

    public virtual void StateExit()
    {
        _isExitingState = true;
        

    }


    public virtual void StateUpdate()
    {

    }

    public virtual void StateFixedUpdate()
    {
        _enemy.CollisionChecks();
        _enemy.enemyLineofSight();

    }



}
