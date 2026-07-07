using System;
using UniRx;
using UnityEngine;

public interface IPlayerFacade
{
    void SetPlayer();
    void StartMove();

    IObservable<Unit> OnNewOrbitAttached { get; }
    IObservable<Unit> OnLongJumped { get; }
    IObservable<Unit> OnPlayerDead { get; }

    void RegisterInputSubscriptions();
    void RegisterPlayerSubscriptions();



}

/// <summary>
/// Playerコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
public class PlayerFacade : IPlayerFacade
{
    private readonly PlayerStateMachine _playerStateMachine;
    private readonly PlayerController _playerController;

    public PlayerFacade(PlayerStateMachine playerStateMachine, PlayerController playerController)
    {
        _playerStateMachine = playerStateMachine;
        _playerController = playerController;

    }

    public void SetPlayer()
    {
        _playerController.SetPlayer();
    }

    public void StartMove()
    {
        _playerStateMachine.StartMove();
    }

    public void RegisterInputSubscriptions()
    {
        _playerController.RegisterInputSubscriptions();
    }

    public void RegisterPlayerSubscriptions()
    {
        _playerController.RegisterPlayerSubscriptions();
    }

    public IObservable<Unit> OnNewOrbitAttached => _playerController.OnNewOrbitAttached;

    public IObservable<Unit> OnLongJumped => _playerController.OnLongJumped;

    public IObservable<Unit> OnPlayerDead => _playerController.OnPlayerDead;


}
