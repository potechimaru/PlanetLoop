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
/// Inputコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
public class InputFacade : IInputFacade
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
        _jumpReleasedDisposable = _inputService.OnJumpReleased.Subscribe( _=>
        {
            //Debug.Log("InputFacade Jump");
            OnJumpReleased?.Invoke();
        });
    }

    public void JumpPressedSubscribe(Action OnJumpPressed)
    {
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


}
