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

    void StartTimer();
    void StopTimer();

    void SetAllDetectionEnabled(bool isEnabled);

    int GetScore();

    int GetVisitedSplineCount();
    int GetDefeatedEnemyCount();    

    float GetElapsedTime();

    IObservable<Unit> OnPlayerDead { get; }

    void EndGame();

    void StartPointObjectListening();

}

/// <summary>
/// GameStateコンポーネント群が外部のメソッドを呼び出すためのFacadeクラス。
/// このクラスを通じて、ゲームの状態やイベントにアクセスすることができる。
/// </summary>
public class GameStateExternalFacade: IGameStateExternalFacade
{
    private readonly IPlayerFacade _playerFacade;
    private readonly IEnemyFacade _enemyFacade;
    private readonly IUIFacade _UIFacade;
    private readonly IPointObjectFacade _pointObjectFacade;

    private readonly GameSessionService _gameSessionService;
    private readonly SaveDataService _saveDataService;
    private readonly UnityroomRankingService _unityroomRankingService;

    public GameStateExternalFacade(IPlayerFacade playerFacade, IEnemyFacade enemyFacade, IUIFacade UIFacade, IPointObjectFacade pointObjectFacade, GameSessionService gameSessionService, SaveDataService saveDataService, UnityroomRankingService unityroomRankingService)
    {
        _playerFacade = playerFacade;
        _enemyFacade = enemyFacade;
        _UIFacade = UIFacade;
        _pointObjectFacade = pointObjectFacade;
        _gameSessionService = gameSessionService;
        _saveDataService = saveDataService;
        _unityroomRankingService = unityroomRankingService;
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

    public void StartTimer()
    {
        _UIFacade.StartTimer();
    }

    public void StopTimer()
    {
        _UIFacade.StopTimer();
    }

    public void SetAllDetectionEnabled(bool isEnabled)
    {
        _enemyFacade.SetAllDetectionEnabled(isEnabled);
    }

    public int GetScore()
    {
        return _UIFacade.GetScore();
    }

    public int GetVisitedSplineCount()
    {
        return _UIFacade.GetVisitedSplineCount();
    }

    public int GetDefeatedEnemyCount()
    {
        return _UIFacade.GetDefeatEnemyCount();
    }

    public float GetElapsedTime()
    {
        return _UIFacade.GetElapsedTime();
    }

    public void EndGame()
    {
        int score = _UIFacade.GetScore();
        int visitedSplineCount = _UIFacade.GetVisitedSplineCount();
        int defeatedEnemyCount = _UIFacade.GetDefeatEnemyCount();

        _gameSessionService.EndGame(score);

        GameModeType mode = _gameSessionService.LastResult.Mode;

        // ローカル保存
        _saveDataService.SetHighScore(
            mode,
            score);

        _saveDataService.SetMaxVisitedSplineCount(
            mode,
            visitedSplineCount);

        _saveDataService.SetMaxDefeatedEnemyCount(
            mode,
            defeatedEnemyCount);

        // unityroomへ送信
        _unityroomRankingService.SendResults(
            score,
            visitedSplineCount,
            defeatedEnemyCount);
    }

    public void StartPointObjectListening()
    {
        _pointObjectFacade.StartPointObjectListening();
    }

    public IObservable<Unit> OnPlayerDead => _playerFacade.OnPlayerDead;
}
