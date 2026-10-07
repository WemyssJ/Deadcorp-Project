using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirAttackState : PlayerState
{
    PlayerMelee _playerMelee;

    public PlayerAirAttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _playerMelee = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMelee>();


        //increase attackrange for air tolerance
        _playerMelee.attackRange = _moveStats.AirAttackRange;
        


        if (_player.VerticalVelocity < 0f && _player.CanPogo())
        {
            _playerMelee.attackRange = _moveStats.PogoAttackRange;
            _playerMelee.AttackPointToUse = _playerMelee.PogoAttackPoint;
            _player.StateMachine.ChangeState(_player.AirPogoAttackState);

        }
        else
        {
            _playerMelee.attackRange = _moveStats.AirAttackRange;
            _playerMelee.AttackPointToUse = _playerMelee.meleeAttackPoint;
            _player.ChangeAnimationState("AirAttack");
            _playerMelee.Attack();
            _player.StateMachine.ChangeState(_player.InAirState);
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();







    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

    }
}
