using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyIdleState : EnemyState
{

    public EnemyIdleState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        _enemy.ChangeAnimationState("Idle");


        if (_enemyStats.BossName == "OnyxShooter")
        {
            _enemy.StateMachine.ChangeState(_enemy.ShootState);
        }
        else
        {
            if (_enemy.Patroling == true && !_enemy.isHoldingPosition)
            {
                if (!_enemyStats.isCashCow)
                {
                    _enemy.StateMachine.ChangeState(_enemy.PatrolState);
                }
                else if (_enemyStats.isCashCow)
                {
                    _enemy.StateMachine.ChangeState(_enemy.RunState);
                }


            }

            if (_enemy.hasLineOfSight == true && _enemyStats.BossName != "OnyxShooter")
            {

                if (_enemyStats.BossName == "Swordsman")
                {
                    _enemy.StateMachine.ChangeState(_enemy.CobaltGoliathAttackState);
                }
                else if (!_enemy.closeToEnemy)
                {
                    _enemy.StateMachine.ChangeState(_enemy.ChaseState);

                }
                else if (_enemy.closeToEnemy && _enemy.CanAttack())
                {
                    _enemy.StateMachine.ChangeState(_enemy.GroundAttackState);
                }
            }

            if (!_enemy.IsGrounded)
            {
                _enemy.StateMachine.ChangeState(_enemy.InAirState);
            }
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
