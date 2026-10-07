using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;
    public static bool RunIsHeld;
    public static bool DashWasPressed;
    public static bool AttackIsPressed;
    public static bool ParryIsPressed;
    public static bool AttackIsHeld;
    public static bool UpIsPressed;
    public static bool DownIsPressed;
    public static bool PauseIsPressed;
    public static Vector2 Aim;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _attackAction;
    private InputAction _aimAction;
    private InputAction _upAction;
    private InputAction _downAction;
    private InputAction _pauseAction;
    private InputAction _parryAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _dashAction = PlayerInput.actions["Dash"];
        _attackAction = PlayerInput.actions["Attack"];
        _aimAction = PlayerInput.actions["Aim"];
        _upAction = PlayerInput.actions["Up"];
        _downAction = PlayerInput.actions["Down"];
        _parryAction = PlayerInput.actions["Parry"];
        _pauseAction = PlayerInput.actions["Pause"];
    }

    private void Update()
    {
        Movement = _moveAction.ReadValue<Vector2>();
        Aim = _aimAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        PauseIsPressed = _pauseAction.WasPressedThisFrame();

        //Run set to always be true
        //RunIsHeld = _runAction.IsPressed();
        RunIsHeld = true;

        DashWasPressed = _dashAction.WasPressedThisFrame();

        AttackIsPressed = _attackAction.IsPressed();
        AttackIsHeld = _attackAction.IsPressed();

        UpIsPressed = _upAction.IsPressed();
        DownIsPressed = _downAction.IsPressed();

        ParryIsPressed = _parryAction.IsPressed();


    }
}
