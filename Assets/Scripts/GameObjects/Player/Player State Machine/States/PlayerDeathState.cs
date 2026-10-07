using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerState
{

    public PlayerDeathState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();


    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_player.currentAnimatorState != "Dead")
        {
            _player.SpeedParticles.Stop();
            _player.TrailRenderer.emitting = false;
            _player.ChangeAnimationState("Dead");
            _player.Die();
        }



    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        _player.RB.velocity = new Vector2(0, 0);

    }
}
