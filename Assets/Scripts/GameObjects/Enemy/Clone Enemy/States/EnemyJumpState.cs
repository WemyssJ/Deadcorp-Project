using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpState : EnemyState
{
    public EnemyJumpState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        //_enemy.InitiateJump();

        _enemy.StateMachine.ChangeState(_enemy.InAirState);
    }

    public override void StateExit()
    {
        base.StateExit();    
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        //even though we exit this state immediately, we need to account for movement in case a physics update is called before the transition is made for the sake of smooth movement

        //_enemy.Move(_enemyStats.AirAcceleration);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

    }
}
