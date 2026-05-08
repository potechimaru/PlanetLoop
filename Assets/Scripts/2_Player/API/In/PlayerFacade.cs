using System;
using UniRx;
using UnityEngine;

public interface IPlayerFacade
{
    void SetPlayer();
    void StartMove();

    IObservable<Vector3> OnNewOrbitAttached { get; }
    IObservable<Unit> OnLongJumped { get; }

    void RegisterInputSubscriptions();
    void RegisterPlayerSubscriptions();

}

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

    public IObservable<Vector3> OnNewOrbitAttached => _playerController.OnNewOrbitAttached;

    public IObservable<Unit> OnLongJumped => _playerController.OnLongJumped;


}
