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

    void SetAllDetectionEnabled(bool isEnabled);

    int GetScore();

    IObservable<Unit> OnPlayerDead { get; }

    void EndGame();

}

public class GameStateExternalFacade: IGameStateExternalFacade
{
    private readonly IPlayerFacade _playerFacade;
    private readonly IEnemyFacade _enemyFacade;
    private readonly IUIFacade _UIFacade;

    private readonly GameSessionService _gameSessionService;
    private readonly SaveDataService _saveDataService;

    public GameStateExternalFacade(IPlayerFacade playerFacade, IEnemyFacade enemyFacade, IUIFacade UIFacade, GameSessionService gameSessionService, SaveDataService saveDataService)
    {
        _playerFacade = playerFacade;
        _enemyFacade = enemyFacade;
        _UIFacade = UIFacade;
        _gameSessionService = gameSessionService;
        _saveDataService = saveDataService;
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

    public void SetAllDetectionEnabled(bool isEnabled)
    {
        _enemyFacade.SetAllDetectionEnabled(isEnabled);
    }

    public int GetScore()
    {
        return _UIFacade.GetScore();
    }

    public void EndGame()
    {
        _gameSessionService.EndGame(_UIFacade.GetScore());
        _saveDataService.SetHighScore(_gameSessionService.LastResult.Mode, _gameSessionService.LastResult.Score);
    }

    public IObservable<Unit> OnPlayerDead => _playerFacade.OnPlayerDead;
}
