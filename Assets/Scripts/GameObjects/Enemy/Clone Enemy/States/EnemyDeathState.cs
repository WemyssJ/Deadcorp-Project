using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyDeathState : EnemyState
{
    public float DestroyTime;
    private Vector3 currentScale;



    public EnemyDeathState(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _enemy.DisableCollider();
        DestroyTime = Time.time + _enemyStats.TimeUntiDespawn;

        currentScale = _enemy.transform.localScale;

    }

    public override void StateUpdate()
    {
        base.StateUpdate();
        _enemy.transform.localScale = currentScale;
        _enemy.rend.color = _enemy.startingColour;

        if (_enemy.currentAnimatorState != "Dead")
        {
            _enemy.RB.velocity = new Vector2(0, 0);
            _enemy.ChangeAnimationState("Dead");
        }

        if(Time.time > DestroyTime)
        {
            _enemy.Despawn();
        }

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

    }



}
