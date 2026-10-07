using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundAttackState : PlayerState
{
    PlayerMelee _playerMelee;

    private float AnimationRandomiser;

    public PlayerGroundAttackState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        _playerMelee = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMelee>();

        _player.IsJumping = false;

        
        _playerMelee.attackRange = _moveStats.GroundedAttackRange;
        


        if (!_player.IsGrounded)
        {
            _player.StateMachine.ChangeState(_player.AirAttackState);
        }


        if (Mathf.Abs(InputManager.Movement.x) > _moveStats.MoveThreshold && InputManager.RunIsHeld)
        {
            
            _playerMelee.Attack();
            _player.ChangeAnimationState("RunAttack");
            _player.StateMachine.ChangeState(_player.RunState);
            

            _playerMelee.AttackPointToUse = _playerMelee.meleeAttackPoint;
        }
        if (Mathf.Abs(InputManager.Movement.x) < _moveStats.MoveThreshold)
        {

            /*AnimationRandomiser = 1;

            if (AnimationRandomiser == 1)
            {
                _player.ChangeAnimationState("IdleAttack1");
                

            }
            else if (AnimationRandomiser == 2)
            {
                _player.ChangeAnimationState("IdleAttack3");
            }
            _playerMelee.Attack();
            _player.StateMachine.ChangeState(_player.IdleState);*/

            ///sub this in for varying attack animation framework and Up attack framework
            if (InputManager.UpIsPressed && _player.CanIdleUpAttack())
            {
                _playerMelee.AttackPointToUse = _playerMelee.upMeleeAttackPoint;
                _player.ChangeAnimationState("IdleUpAttack");
                _playerMelee.Attack();
                _player.StateMachine.ChangeState(_player.IdleState);
            } else
            {
                AnimationRandomiser = 1; // Random.Range(1, 3);

                if (AnimationRandomiser == 1)
                {

                    _playerMelee.AttackPointToUse = _playerMelee.meleeAttackPoint;
                    _player.ChangeAnimationState("IdleAttack1");

                }
                else if (AnimationRandomiser == 2)
                {
                    _player.ChangeAnimationState("IdleAttack3");
                }
                _playerMelee.Attack();
                _player.StateMachine.ChangeState(_player.IdleState);
            }

            if (InputManager.ParryIsPressed && _player.CanParry())
            {
                _player.StateMachine.ChangeState(_player.GroundParryState);
            }

        }

    }

    public override void StateUpdate()
    {
        base.StateUpdate();



        

    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
        //this gets called here for acceleration/movement
        _player.Move(_moveStats.GroundAcceleration, _moveStats.GroundDeceleration, InputManager.Movement);
    }
}
