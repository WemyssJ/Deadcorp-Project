using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class EnemyShootState : EnemyState
{


    public EnemyShootState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.hasLostLineOfSight = false;
        _enemy.ChangeAnimationState("Idle");


    }
    public override void StateUpdate()
    {
        base.StateUpdate();


        if(_enemy.hasLineOfSight)
        {
            if (_enemy.standardAttackNo == 1)
            {
                //phase 1
                _enemy.HandleGunRotation();
                _enemy.HandleGunShooting();
            }
            else if (_enemy.standardAttackNo == 2)
            {
                //phase 2
            }
            else if (_enemy.standardAttackNo == 3)
            {
                //phase 3
            }
        }
       

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _enemy.Move(_enemyStats.MaxRunSpeed);
    }


}
