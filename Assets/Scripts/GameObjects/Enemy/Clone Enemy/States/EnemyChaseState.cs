using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyChaseState : EnemyState
{

    public EnemyChaseState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.hasLostLineOfSight = false;
        //_enemy.ChangeAnimationState("Run");


    }
    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_enemy.xDistance <1.25f)
        {
            _enemy.ChangeAnimationState("Idle");
        }
        else
            _enemy.ChangeAnimationState("Run");


        if (_enemy.hasLineOfSight == false)
        {
            _enemy.StateMachine.ChangeState(_enemy.LostTargetState);
        }

        if (_enemy.closeToEnemy == true)
        {
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }

        if (_enemy.IsGrounded == false)
        {
            _enemy.StateMachine.ChangeState(_enemy.InAirState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _enemy.Move(_enemyStats.MaxRunSpeed);
    }
}
