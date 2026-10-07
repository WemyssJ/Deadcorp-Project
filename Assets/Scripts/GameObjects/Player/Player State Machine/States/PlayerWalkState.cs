using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerState
{
    Player _playerScript;
    public PlayerWalkState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        
    }


    public override void StateExit()
    {
        base.StateExit();
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (!_playerScript.isAnimationPlaying(_player.animator, _player.DashAnim))
        {
            _playerScript.ChangeAnimationState("Walk");
        }

        if (Mathf.Abs(InputManager.Movement.x) > _moveStats.MoveThreshold && InputManager.RunIsHeld)
        {
            _player.StateMachine.ChangeState(_player.RunState);
        }

        else if (Mathf.Abs(InputManager.Movement.x) < _moveStats.MoveThreshold)
        {
            if (!_playerScript.isAnimationPlaying(_player.animator, _player.DashAnim))
            {
                _playerScript.ChangeAnimationState("Walk");
            }

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

        else if (_player.JumpBufferedOrCoyoteTimed())
        {
            _player.SpawnJumpParticles(_player.JumpParticles);

            _player.StateMachine.ChangeState(_player.JumpState);
        }

        if (InputManager.DashWasPressed && (_player.CanDash() || _player.CanAirDash()))
        {
            _player.StateMachine.ChangeState(_player.DashState);
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        //this gets called here for acceleration/movement
        _player.Move(_moveStats.GroundAcceleration, _moveStats.GroundDeceleration, InputManager.Movement);
    }
}
