using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    AnimatorStateInfo animStateInfo;
    public float DashEndTime;

    public PlayerDashState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _player.InitiateDash();

        //animStateInfo = _player.animator.GetCurrentAnimatorStateInfo(0);
        

    }

    public override void StateExit()
    {

        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        //WALL SLIDE
        if (_player.ShouldWallSlide())
        {
            _player.StateMachine.ChangeState(_player.WallSlideState);
        }

        if (!_player.IsDashing && !_player.IsGrounded && !_isExitingState)
        {
            _player.ChangeAnimationState("Fall");
            _player.StateMachine.ChangeState(_player.InAirState);
        }

        else if (!_player.IsDashing && _player.IsGrounded && !_isExitingState )
        {
            _player.StateMachine.ChangeState(_player.IdleState);
        }

        else if (InputManager.JumpWasPressed)
        {
            if (_player.CanJump())
            {
                _player.SpawnJumpParticles(_player.JumpParticles);

                _player.StateMachine.ChangeState(_player.JumpState);
            }
        }

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        _player.DashPhysics();
    }
}
