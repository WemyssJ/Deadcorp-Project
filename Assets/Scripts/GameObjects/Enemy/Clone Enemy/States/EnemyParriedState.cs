using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParriedState : EnemyState
{
    private float AnimationCurrentTime;

    public EnemyParriedState(Enemy enemy, EnemyStateMachine enemyStateMachine) : base(enemy, enemyStateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.ChangeAnimationState("Hurt");
    }

    public override void StateExit()
    {
        base.StateExit();
        
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        if (_enemy.currentAnimatorState == "Hurt" && !_enemy.isAnimationPlaying(_enemy.animator, "Hurt"))
        {
            _enemy.StateMachine.ChangeState(_enemy.IdleState);
        }
    }
    

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();


    }
}
