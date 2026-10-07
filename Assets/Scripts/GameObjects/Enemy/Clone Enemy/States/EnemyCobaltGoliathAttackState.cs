using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyCobaltGoliathAttackState : EnemyState
{

    public EnemyCobaltGoliathAttackState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_enemy.standardAttackNo == 6)
        {
            _enemy.standardAttackNo = 1;
            _enemy.StateMachine.ChangeState(_enemy.InvisibleState);
        }
        else if (_enemy.standardAttackNo == 3)
        {
            _enemy.standardAttackNo++;
            _enemy.StateMachine.ChangeState(_enemy.GroundDashState);
        }
        else if (!_enemy.closeToEnemy)
        {
            _enemy.StateMachine.ChangeState(_enemy.ChaseState);

        }
        else if (_enemy.closeToEnemy)
        {
            if ((_enemy.standardAttackNo == 1 || _enemy.standardAttackNo == 2 || _enemy.standardAttackNo == 4 || _enemy.standardAttackNo == 5) && _enemy.CanAttack())
            {
                _enemy.standardAttackNo++;
                _enemy.StateMachine.ChangeState(_enemy.GroundAttackState);
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
