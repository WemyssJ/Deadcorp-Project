using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyState
{

    public EnemyPatrolState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (_enemy.currentAnimatorState == "RunAttack" && !_enemy.isAnimationPlaying(_enemy.animator, "RunAttack") || _enemy.currentAnimatorState == "Run" || _enemy.currentAnimatorState == "Idle")
        {
            _enemy.ChangeAnimationState("Walk");
        }



        if (!_enemy.Patroling && _enemy.isHoldingPosition == true)
        {
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }

        if (_enemy.hasLineOfSight == true)
        {
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        }

        if (!_enemy.IsGrounded)
        {
            _enemy.StateMachine.ChangeState(_enemy.InAirState);
        }


    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        if (_enemy.Patroling == true || _enemy.isChasing == true)
        {
            _enemy.Move(_enemyStats.MaxWalkSpeed);
        }
        else if (!_enemy.isHoldingPosition)
        {
            _enemy.Move(0);
        }

    }
}
