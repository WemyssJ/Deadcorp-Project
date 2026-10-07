using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInAirState : EnemyState
{
    private float AnimationCurrentTime;

    public EnemyInAirState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.ChangeAnimationState("Fall");
    }

    public override void StateExit()
    {
        base.StateExit();
        
    }

    public override void StateUpdate()
    {
        base.StateUpdate();


        if (_enemy.IsGrounded)
        {
            if(_enemy.currentAnimatorState == "Fall")
            {
                _enemy.ChangeAnimationState("Land");
            }
            

            if (_enemy.currentAnimatorState == "Land" && !_enemy.isAnimationPlaying(_enemy.animator, "Land"))
            {
                _enemy.StateMachine.ChangeState(_enemy.IdleState);
            }
        }
      
      
    }
    

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();


    }
}
