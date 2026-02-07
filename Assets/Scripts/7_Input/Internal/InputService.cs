using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IDisposable
{
    private PlayerInput _playerActions = new();

    // ---- Move（時計回り / 反時計回り トグル） ----
    private Subject<Vector2> _onVector2Input = new();
    public IObservable<Vector2> OnVector2Input => _onVector2Input;

    // ---- Jump ----
    private Subject<Unit> _onJump = new();
    public IObservable<Unit> OnJump => _onJump;

    // 現在の回転方向
    // true  = 時計回り
    // false = 反時計回り
    private bool _isClockwise = true;

    internal InputService()
    {
        var move = _playerActions.Player.Move;
        var jump = _playerActions.Player.Jump;

        move.actionMap.Enable();

        // --- Move（トグル） ---
        move.performed += OnMoveToggle;

        // --- Jump ---
        jump.performed += OnJumpPerformed;

        // 初期方向を通知
        EmitMoveDirection();
    }

    private void OnMoveToggle(InputAction.CallbackContext ctx)
    {
        // 押すたびに反転
        _isClockwise = !_isClockwise;

        EmitMoveDirection();
    }

    private void EmitMoveDirection()
    {
        float dir = _isClockwise ? 1f : -1f;
        _onVector2Input.OnNext(new Vector2(0f, dir));
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _onJump.OnNext(Unit.Default);
    }

    public void Dispose()
    {
        _onVector2Input?.Dispose();
        _onJump?.Dispose();

        _playerActions?.Dispose();
    }
}
