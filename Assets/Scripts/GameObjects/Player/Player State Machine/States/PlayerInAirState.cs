using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private float AnimationCurrentTime;

    public PlayerInAirState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        _player.SpeedParticles.Stop();


        //if idle attack is playing, transition into jump attack
        if (_player.currentAnimatorState == "IdleAttack1" && _player.isAnimationPlaying(_player.animator, "IdleAttack1"))
        {
            AnimationCurrentTime = _player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _player.MidAnimationTransitionState("AirAttack", 0, AnimationCurrentTime);
        }else if (_player.currentAnimatorState == "RunAttack" && _player.isAnimationPlaying(_player.animator, "RunAttack"))
        {
            AnimationCurrentTime = _player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _player.MidAnimationTransitionState("AirAttack", 0, AnimationCurrentTime);
        }
        else if (_player.currentAnimatorState == "AirAttack" && !_player.isAnimationPlaying(_player.animator, "AirAttack"))
        {
            _player.ChangeAnimationState("Fall");
        }else if (_player.currentAnimatorState == _player.PogoAttackAnim && !_player.isAnimationPlaying(_player.animator, _player.PogoAttackAnim))
        {
            _player.isPogoing = false;
            _player.IsFastFalling = false;
            _player.ChangeAnimationState("Fall");
        }
        else if (_player.currentAnimatorState != "AirAttack" && _player.currentAnimatorState != _player.PogoAttackAnim)
        {
           _player.ChangeAnimationState("Jump");
        }
        else if (_player.currentAnimatorState == "Idle")
        {
            _player.ChangeAnimationState("Fall");
        }



        //other state transitions

        //JUMP/WallJump
        if (InputManager.JumpWasPressed)
        {
            if (_player.CanJump())
            {
                _player.SpawnJumpParticles(_player.JumpParticles);

                _player.StateMachine.ChangeState(_player.JumpState);
            }

            if (_player.CanAirJump())
            {
                _player.SpawnJumpParticles(_player.SecondJumpParticles);

                _player.StateMachine.ChangeState(_player.JumpState);
            }

            if (_player.CanWallJumpDueToPostBufferTimer())
            {
                _player.UseWallJumpMoveStats = true;
                _player.StateMachine.ChangeState(_player.WallJumpState);
            }
        }

        else if (_player.JumpBufferedOrCoyoteTimed())
        {
            _player.SpawnJumpParticles(_player.JumpParticles);
            _player.StateMachine.ChangeState(_player.JumpState);
        }

        //LAND
        if (_player.HasLanded())
        {   
            _player.SpawnJumpParticles(_player.LandParticles);

            _player.StateMachine.ChangeState(_player.IdleState);
        }

        //WALL SLIDE
        if (_player.ShouldWallSlide())
        {
            _player.StateMachine.ChangeState(_player.WallSlideState);
        }

        //DASH
        if (InputManager.DashWasPressed && (_player.CanAirDash()))
        {
            _player.StateMachine.ChangeState(_player.DashState);
        }

        if (InputManager.AttackIsPressed && _player.CanAttack())
        {
            _player.StateMachine.ChangeState(_player.AirAttackState);
        }
    }
    

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        _player.JumpPhysics();
        _player.WallJumpPhysics();
        _player.DashPhysics();


        //movement
        if (_player.UseWallJumpMoveStats)
        {
            _player.Move(_moveStats.WallJumpMoveAcceleration, _moveStats.WallJumpMoveDeceleration, InputManager.Movement);
        }

        else
        {
            _player.Move(_moveStats.AirAcceleration, _moveStats.AirDeceleration, InputManager.Movement);     
        }
    }
}
