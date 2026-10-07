using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInvisibleState : EnemyState
{

    private float AnimationRandomiser;

    public EnemyInvisibleState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.isInvulnerable = true;
        _enemy.StartCoroutine(_enemy.FadeOut());
        _enemy.hasLineOfSight = true;


    }

    public override void StateUpdate()
    {
        base.StateUpdate();


    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _enemy.Move(_enemyStats.MaxRunSpeed * 1.5f);



    }


}
