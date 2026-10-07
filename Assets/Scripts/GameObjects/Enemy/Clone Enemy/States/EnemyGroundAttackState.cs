using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundAttackState : EnemyState
{

    private float AnimationRandomiser;

    public EnemyGroundAttackState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.closeToEnemy = false;
        _enemy.AttackDelay();


    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        _enemy.AnimatorApplyDamage(_enemy.animator, "RunAttack");


        if (_enemy.currentAnimatorState == "RunAttack" && !_enemy.isAnimationPlaying (_enemy.animator, "RunAttack"))
        {
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }

        if (!_enemy.IsGrounded)
        {
            _enemy.StateMachine.ChangeState(_enemy.InAirState);
        }

        if(_enemy.isDead)
        {
            _enemy.StateMachine.ChangeState(_enemy.DeathState);
        }

        if (_enemy.hasLineOfSight == false)
        {
            _enemy.ChangeAnimationState("Idle");
            _enemy.StateMachine.ChangeState(_enemy.LostTargetState);
        }




    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        if (_enemy.currentAnimatorState == "RunAttack")
        {
            
        }
        else _enemy.ChangeAnimationState("Run");

        _enemy.Move(_enemyStats.MaxRunSpeed);


    }


}
