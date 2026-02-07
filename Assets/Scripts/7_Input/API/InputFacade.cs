using UnityEngine;
using UniRx;
using System;

public interface IInputFacade
{
    void JumpSubscribe(Action OnJump);
    void JumpUnsubscribe();
    void MoveSubscribe();
    void MoveUnsubscribe();

    void AllSubscribe(Action OnJump);

}

public class InputFacade : IInputFacade
{
    private InputService _inputService;

    private IDisposable _jumpDisposable;
    private IDisposable _moveDisposable;

    internal InputFacade(InputService inputService)
    {
        _inputService = inputService;
    }

    public void JumpSubscribe(Action OnJump)
    {
        _jumpDisposable = _inputService.OnJump.Subscribe( _=>
        {
            Debug.Log("InputFacade Jump");
            OnJump?.Invoke();
        });
    }

    public void JumpUnsubscribe()
    {
        _jumpDisposable?.Dispose();
        _jumpDisposable = null;
    }

    public void MoveSubscribe()
    {
        _moveDisposable = _inputService.OnVector2Input.Subscribe( dir =>
        {
            Debug.Log($"InputFacade Move: {dir}");
        });
    }

    public void MoveUnsubscribe()
    {
        _moveDisposable?.Dispose();
        _moveDisposable = null;

    }

    public void AllSubscribe(Action OnJump)
    {
        JumpSubscribe(OnJump);
        MoveSubscribe();
    }

    public void AllUnsubscribe()
    {
        JumpUnsubscribe();
        MoveUnsubscribe();
    }


}
