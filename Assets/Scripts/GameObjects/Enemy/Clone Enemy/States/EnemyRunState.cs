using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRunState : EnemyState
{

    public EnemyRunState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();


    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        _enemy.ChangeAnimationState("Run");

        if (!_enemyStats.isCashCow)
        {
            if (_enemy.Patroling == false && _enemy.isHoldingPosition == true)
            {
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
            }
        }
        
        
        /*if (!_enemy.IsGrounded && !_isExitingState)
        {
            _enemy.ChangeAnimationState("Fall");
            _enemy.StateMachine.ChangeState(_enemy.InAirState);
        }*/

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        if (_enemy.Patroling == true)
        {
            _enemy.Move(_enemyStats.MaxRunSpeed);
        }
        else if (_enemy.Patroling == false)
        {
            _enemy.Move(0);
        }

    }
}
