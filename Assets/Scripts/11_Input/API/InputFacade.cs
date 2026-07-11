using UnityEngine;
using UniRx;
using System;

public interface IInputFacade
{
    void JumpReleasedSubscribe(Action OnJumpReleased);

    void JumpPressedSubscribe(Action OnJumpPressed);

    void JumpUnsubscribe();
    void MoveSubscribe(Action OnMove);
    void MoveUnsubscribe();

    void AllSubscribe(Action OnJumpReleased, Action OnJumpPressed, Action OnMove);

}

/// <summary>
/// Input?R???|?[?l???g?Q????????\?b?h???O??????J????Facade?B
/// ?????\?????B?????A?O???????A?N?Z?X????f???????????????B
/// </summary>
public class InputFacade : IInputFacade, IDisposable
{
    private InputService _inputService;

    private IDisposable _jumpReleasedDisposable;
    private IDisposable _jumpPressedDisposable;
    private IDisposable _moveDisposable;

    internal InputFacade(InputService inputService)
    {
        _inputService = inputService;
    }

    public void JumpReleasedSubscribe(Action OnJumpReleased)
    {
        _jumpReleasedDisposable?.Dispose();
        _jumpReleasedDisposable = _inputService.OnJumpReleased.Subscribe( _=>
        {
            OnJumpReleased?.Invoke();
        });
    }

    public void JumpPressedSubscribe(Action OnJumpPressed)
    {
        _jumpPressedDisposable?.Dispose();
        _jumpPressedDisposable = _inputService.OnJumpPressed.Subscribe( _=>
        {
            OnJumpPressed?.Invoke();
        });
    }

    public void JumpUnsubscribe()
    {
        _jumpReleasedDisposable?.Dispose();
        _jumpPressedDisposable?.Dispose();
        _jumpReleasedDisposable = null;
        _jumpPressedDisposable = null;
    }

    public void MoveSubscribe(Action OnMove)
    {
        _moveDisposable?.Dispose();
        _moveDisposable = _inputService.OnMove.Subscribe( _=>
        {
            OnMove?.Invoke();
        });
    }

    public void MoveUnsubscribe()
    {
        _moveDisposable?.Dispose();
        _moveDisposable = null;

    }

    public void AllSubscribe(Action OnJumpReleased, Action OnJumpPressed, Action OnMove)
    {
        JumpReleasedSubscribe(OnJumpReleased);
        JumpPressedSubscribe(OnJumpPressed);
        MoveSubscribe(OnMove);
    }

    public void AllUnsubscribe()
    {
        JumpUnsubscribe();
        MoveUnsubscribe();
    }

    public void Dispose()
    {
        AllUnsubscribe();
    }
}
