using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private float AnimationCurrentTime;


    public PlayerIdleState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
        _player.TrailRenderer.emitting = false;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (!_player.IsGrounded)
        {
            {
                _player.StateMachine.ChangeState(_player.InAirState);
            }
        }

        //if run or air attack/run attacki is playing, transition into idle attack, otherwise, idle animation
        if (_player.currentAnimatorState == "RunAttack" && _player.isAnimationPlaying(_player.animator, "RunAttack") || (_player.currentAnimatorState == "AirAttack" && _player.isAnimationPlaying(_player.animator, "AirAttack")))
        {
            AnimationCurrentTime = _player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _player.MidAnimationTransitionState("IdleAttack1", 0, AnimationCurrentTime);
        }
        else if (_player.currentAnimatorState == "IdleAttack1" && !_player.isAnimationPlaying(_player.animator, "IdleAttack1") /*|| (_player.currentAnimatorState == "IdleAttack2" && !_player.isAnimationPlaying(_player.animator, "IdleAttack2")) || (_player.currentAnimatorState == "IdleAttack3" && !_player.isAnimationPlaying(_player.animator, "IdleAttack3"))*/)
        {
            _player.ChangeAnimationState("Idle");
        }
        else if (_player.currentAnimatorState == "IdleUpAttack" && !_player.isAnimationPlaying(_player.animator, "IdleUpAttack") /*|| (_player.currentAnimatorState == "IdleAttack2" && !_player.isAnimationPlaying(_player.animator, "IdleAttack2")) || (_player.currentAnimatorState == "IdleAttack3" && !_player.isAnimationPlaying(_player.animator, "IdleAttack3"))*/)
        {
            _player.ChangeAnimationState("Idle");
        }
        else if (_player.currentAnimatorState == "Run" || _player.currentAnimatorState == "Land" || _player.currentAnimatorState == _player.DashAnim || _player.currentAnimatorState == "Parry" || _player.currentAnimatorState == "Fall" || _player.currentAnimatorState == "Jump"    )
        {
            _player.ChangeAnimationState("Idle");
        }



        if (Mathf.Abs(InputManager.Movement.x) > _moveStats.MoveThreshold && InputManager.RunIsHeld)
        {
            _player.StateMachine.ChangeState(_player.RunState);
        }

        else if (InputManager.JumpWasPressed)
        {
            if (_player.CanJump())
            {
                _player.SpawnJumpParticles(_player.JumpParticles);

                _player.StateMachine.ChangeState(_player.JumpState);
            }
        }

        else if (_player.JumpBufferedOrCoyoteTimed())
        {
            _player.SpawnJumpParticles(_player.JumpParticles);

            _player.StateMachine.ChangeState(_player.JumpState);
        }

        if (InputManager.DashWasPressed && (_player.CanDash() || _player.CanAirDash()))
        {
            _player.StateMachine.ChangeState(_player.DashState);
        }

        if (InputManager.AttackIsPressed &&  _player.CanAttack())
        {
            _player.StateMachine.ChangeState(_player.GroundAttackState);
        }

        if (InputManager.ParryIsPressed && _player.CanParry())
        {
            _player.StateMachine.ChangeState(_player.GroundParryState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        if (_player.IsGrounded)
        {
            _player.Move(_moveStats.GroundAcceleration, _moveStats.GroundDeceleration, InputManager.Movement);

        }
        else if (!_player.IsGrounded && !_player.IsFastFalling)
        {
            //_player.JumpPhysics();
            //_player.WallJumpPhysics();
            //_player.DashPhysics();
            //_player.Move(_moveStats.AirAcceleration, _moveStats.AirDeceleration, InputManager.Movement);
        }
        
    }
}
