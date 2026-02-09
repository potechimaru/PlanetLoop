using System;
using UniRx;
using UnityEngine.InputSystem;

public class InputService : IDisposable
{
    private PlayerInput _playerActions = new();

    // ---- Move（方向反転）----
    private Subject<Unit> _onMove = new();
    public IObservable<Unit> OnMove => _onMove;

    // ---- Jump ----
    private Subject<Unit> _onJumpPressed = new();
    private Subject<Unit> _onJumpReleased = new();

    public IObservable<Unit> OnJumpPressed => _onJumpPressed;
    public IObservable<Unit> OnJumpReleased => _onJumpReleased;

    internal InputService()
    {
        var move = _playerActions.Player.Move;
        var jump = _playerActions.Player.Jump;

        move.actionMap.Enable();

        // --- Move（右クリック） ---
        move.performed += _ => _onMove.OnNext(Unit.Default);

        // --- Jump（左クリック） ---
        jump.started += _ => _onJumpPressed.OnNext(Unit.Default);
        jump.canceled += _ => _onJumpReleased.OnNext(Unit.Default);
    }

    public void Dispose()
    {
        _onMove?.Dispose();
        _onJumpPressed?.Dispose();
        _onJumpReleased?.Dispose();

        _playerActions?.Dispose();
    }
}
