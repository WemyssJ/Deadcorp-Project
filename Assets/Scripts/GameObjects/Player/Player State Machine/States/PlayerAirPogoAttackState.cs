using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirPogoAttackState : PlayerState
{
    PlayerMelee _playerMelee;

    private float AnimationRandomiser;

    public PlayerAirPogoAttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _playerMelee = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMelee>();

        _player.ChangeAnimationState(_player.PogoAttackAnim);
        _player.isPogoing = true;
        _player.IsDashFastFalling = false;
        _playerMelee.Attack();

    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        

        _player.StateMachine.ChangeState(_player.InAirState);



    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        //even though we exit this state immediately, we need to account for movement in case a physics update is called before the transition is made for the sake of smooth movement

        _player.Move(_moveStats.AirAcceleration, _moveStats.AirDeceleration, InputManager.Movement);
    }
}
