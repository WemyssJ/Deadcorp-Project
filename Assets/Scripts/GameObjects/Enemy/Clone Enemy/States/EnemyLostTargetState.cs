using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyLostTargetState : EnemyState
{

    public EnemyLostTargetState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _enemy.ChangeAnimationState("Idle");
        _enemy.LostTarget();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_enemy.hasLineOfSight)
        {
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        }

        if (!_enemy.hasLostLineOfSight)
        {
            _enemy.StateMachine.ChangeState(_enemy.PatrolState);
        }

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _enemy.Move(0);
    }
}
