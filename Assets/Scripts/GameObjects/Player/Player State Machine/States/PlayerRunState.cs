using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRunState : PlayerState
{
    private float AnimationCurrentTime;

    public PlayerRunState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }

    public override void StateExit()
    {
        base.StateExit();

        if (_player.SpeedParticles.isPlaying)
        {
            _player.SpeedParticles.Stop();
        }

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

       
        //if idle attack is playing, transition into run attack
        if (_player.currentAnimatorState == "IdleAttack1" && _player.isAnimationPlaying(_player.animator, "IdleAttack1"))
        {
            AnimationCurrentTime = _player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _player.MidAnimationTransitionState("RunAttack", 0, AnimationCurrentTime);
        }else if (_player.currentAnimatorState == "RunAttack" && !_player.isAnimationPlaying(_player.animator, "RunAttack"))
        {
            _player.ChangeAnimationState("Run");
        }
        else if (_player.currentAnimatorState == _player.DashAnim && !_player.isAnimationPlaying(_player.animator, _player.DashAnim))
        {
            _player.ChangeAnimationState("Run");
        }
        else if (_player.currentAnimatorState == "IdleUpAttack")
        {
            _player.ChangeAnimationState("Run");
        }
        else if (_player.currentAnimatorState == "Idle")
        {
            _player.ChangeAnimationState("Run");
        }
        else if (_player.currentAnimatorState == "Run" || _player.currentAnimatorState == "Land" || _player.currentAnimatorState == "Parry" || _player.currentAnimatorState == "Fall" || _player.currentAnimatorState == "Jump")
        {
            _player.ChangeAnimationState("Run");
        }





        //transitions

        if (InputManager.AttackIsPressed && _player.CanAttack())
        {
            _player.StateMachine.ChangeState(_player.GroundAttackState);
        }

        if (Mathf.Abs(InputManager.Movement.x) < _moveStats.MoveThreshold)
        {
            
            //_player.ChangeAnimationState("Idle");


            _player.StateMachine.ChangeState(_player.IdleState);
        }
        else if (!_player.IsDashing && !_player.IsGrounded && !_isExitingState)
            {
                _player.ChangeAnimationState("Fall");
                _player.StateMachine.ChangeState(_player.InAirState);
            }

        /*else if (Mathf.Abs(InputManager.Movement.x) > _moveStats.MoveThreshold && !InputManager.RunIsHeld)
        {

            if (!_player.isAnimationPlaying(_player.animator, _player.DashAnim))
            {
                _player.ChangeAnimationState("Walk");
            }

            _player.StateMachine.ChangeState(_player.WalkState);
        }*/

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

        if (InputManager.ParryIsPressed && _player.CanParry())
        {
            _player.StateMachine.ChangeState(_player.GroundParryState);
        }

        //FX
        HandleSpeedParticles();
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        //this gets called here for acceleration/movement
        _player.Move(_moveStats.GroundAcceleration, _moveStats.GroundDeceleration, InputManager.Movement);
    }

    private void HandleSpeedParticles()
    {
        if (Mathf.Abs(_player.HorizontalVelocity) >= _player.MoveStats.MaxRunSpeed - 2f)
        {
            if (!_player.SpeedParticles.isPlaying)
            {
                _player.SpeedParticles.Play();
            }
        }

        else
        {
            if (_player.SpeedParticles.isPlaying)
            {
                _player.SpeedParticles.Stop();
            }
        }
    }
}
