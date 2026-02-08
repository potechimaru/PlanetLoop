using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IDisposable
{
    private PlayerInput _playerActions = new();

    // ---- Move（時計回り / 反時計回り トグル） ----
    private Subject<Unit> _onMove = new();
    public IObservable<Unit> OnMove => _onMove;

    // ---- Jump ----
    private Subject<Unit> _onJump = new();
    public IObservable<Unit> OnJump => _onJump;

    internal InputService()
    {
        var move = _playerActions.Player.Move;
        var jump = _playerActions.Player.Jump;

        move.actionMap.Enable();

        // --- Move（トグル） ---
        move.performed += OnMoveToggle;

        // --- Jump ---
        jump.performed += OnJumpPerformed;

    }

    private void OnMoveToggle(InputAction.CallbackContext ctx)
    {
        _onMove.OnNext(Unit.Default);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _onJump.OnNext(Unit.Default);
    }

    public void Dispose()
    {
        _onMove?.Dispose();
        _onJump?.Dispose();

        _playerActions?.Dispose();
    }
}
