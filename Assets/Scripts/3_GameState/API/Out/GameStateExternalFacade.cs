using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public interface IGameStateExternalFacade
{
    void RegisterInputSubscriptions();
    void RegisterPlayerSubscriptions();
    void StartMove();
    void SetPlayer();

    IObservable<Unit> OnPlayerDead { get; }

}

public class GameStateExternalFacade: IGameStateExternalFacade
{
    private readonly IPlayerFacade _playerFacade;

    public GameStateExternalFacade(IPlayerFacade playerFacade)
    {
        _playerFacade = playerFacade;
    }

    public void RegisterInputSubscriptions()
    {
        _playerFacade.RegisterInputSubscriptions();
    }

    public void RegisterPlayerSubscriptions()
    {
        _playerFacade.RegisterPlayerSubscriptions();
    }
    public void StartMove()
    {
        _playerFacade.StartMove();
    }

    public void SetPlayer()
    {
        _playerFacade.SetPlayer();
    }

    public IObservable<Unit> OnPlayerDead => _playerFacade.OnPlayerDead;
}
