using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundParryState : PlayerState
{
    PlayerMelee _playerMelee;

    private float AnimationRandomiser;
    

    public PlayerGroundParryState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
        
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _player.audiosource.PlayOneShot(_player.Parry, .3f);
        _player.ChangeAnimationState("Parry");
        _player.IsParrying = true;



    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if ((_player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.25f && _player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.90f))
        {
            _player.IsParrying = true;
        }
        else
            _player.IsParrying = false;

        if (_player.currentAnimatorState == "Parry" && !_player.isAnimationPlaying(_player.animator, "Parry"))
        {
            _player.IsParrying = false;
            _player.StateMachine.ChangeState(_player.IdleState);
        }


    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        //this gets called here for acceleration/movement
        _player.Move(_moveStats.GroundAcceleration, _moveStats.GroundDeceleration, InputManager.Movement);
    }
}
