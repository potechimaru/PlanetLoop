using System;
using UniRx;
using UnityEngine;

public interface IPlayerFacade
{
    void StartMove();

    IObservable<Vector3> OnNewOrbitAttached { get; }
    IObservable<Unit> OnLongJumped { get; }

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

    public void StartMove()
    {
        _playerStateMachine.StartMove();
    }

    public IObservable<Vector3> OnNewOrbitAttached => _playerController.OnNewOrbitAttached;

    public IObservable<Unit> OnLongJumped => _playerController.OnLongJumped;


}
