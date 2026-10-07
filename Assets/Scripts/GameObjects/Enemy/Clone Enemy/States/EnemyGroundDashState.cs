using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundDashState : EnemyState
{

    private float AnimationRandomiser;

    public EnemyGroundDashState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.closeToEnemy = false;
        _enemy.DashChargeUp();



    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if(_enemy.isDashing && _enemy.isAttacking)
        {
            _enemy.Attack();
            _enemy.NextAttackTime = Time.time + (_enemyStats.attackCooldown/2);

            if(_enemy.isTouchingPlayer)
            {
                _enemy.StateMachine.ChangeState(_enemy.ChaseState);
            }
            
        }
        if (!_enemy.isDashing || _enemy.playerIsDead())
        {
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);
        }



    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        if (_enemy.isDashing && _enemy.isAttacking)
        {
            _enemy.Dash();
        }


    }


}
