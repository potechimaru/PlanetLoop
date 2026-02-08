using UnityEngine;
using UniRx;
using System;

public interface IInputFacade
{
    void JumpSubscribe(Action OnJump);
    void JumpUnsubscribe();
    void MoveSubscribe(Action OnMove);
    void MoveUnsubscribe();

    void AllSubscribe(Action OnJump, Action OnMove);

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
            //Debug.Log("InputFacade Jump");
            OnJump?.Invoke();
        });
    }

    public void JumpUnsubscribe()
    {
        _jumpDisposable?.Dispose();
        _jumpDisposable = null;
    }

    public void MoveSubscribe(Action OnMove)
    {
        _moveDisposable = _inputService.OnMove.Subscribe( _=>
        {
            //Debug.Log($"InputFacade Move: {dir}");
            OnMove?.Invoke();
        });
    }

    public void MoveUnsubscribe()
    {
        _moveDisposable?.Dispose();
        _moveDisposable = null;

    }

    public void AllSubscribe(Action OnJump, Action OnMove)
    {
        JumpSubscribe(OnJump);
        MoveSubscribe(OnMove);
    }

    public void AllUnsubscribe()
    {
        JumpUnsubscribe();
        MoveUnsubscribe();
    }


}
