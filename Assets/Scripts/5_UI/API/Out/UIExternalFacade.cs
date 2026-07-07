using System;
using UnityEngine;
using UniRx;

public interface IUIExternalFacade
{


    //Player
    IObservable<Unit> OnNewOrbitAttached { get; }
    IObservable<Unit> OnLongJumped { get; }

    // PointObject
    IObservable<PointObjectType> OnPointCollected { get; }
    void ResetAllPoints();

    // Orbit
    IReadOnlyReactiveProperty<(int visitedCount, int allCount)> OnSplineCountChanged { get; }
    void ResetAllOrbits();

    // Enemy
    IReadOnlyReactiveProperty<(int defeatEnemyCount, int allEnemyCount)> OnEnemyCountChanged { get; }

    IObservable<Unit> OnEnemyDefeated { get; }

    // Audio
    void PlaySE(SEType sEType);

}

/// <summary>
/// UIコンポーネント群が外部のメソッドを呼び出すためのFacadeクラス。
/// このクラスを通じて、ゲームの状態やイベントにアクセスすることができる。
/// </summary>
public class UIExternalFacade : IUIExternalFacade
{
    private readonly IPointObjectFacade _pointObjectFacade;
    private readonly IOrbitFacade _orbitFacade;
    private readonly IEnemyFacade _enemyFacade;
    private readonly IPlayerFacade _playerFacade;

    private readonly GameSessionService _gameSessionService;

    private readonly AudioManager _audioManager;

    public UIExternalFacade(IPointObjectFacade pointObjectFacade, IOrbitFacade orbitFacade, IEnemyFacade enemyFacade, IPlayerFacade playerFacade ,GameSessionService gameSessionService, AudioManager audioManager)
    {
        _pointObjectFacade = pointObjectFacade;
        _orbitFacade = orbitFacade;
        _enemyFacade = enemyFacade;
        _playerFacade = playerFacade;
        _gameSessionService = gameSessionService;
        _audioManager = audioManager;
    }

    // Player
    public IObservable<Unit> OnNewOrbitAttached => _playerFacade.OnNewOrbitAttached;
    public IObservable<Unit> OnLongJumped => _playerFacade.OnLongJumped;

    // PointObject
    public IObservable<PointObjectType> OnPointCollected => _pointObjectFacade.OnPointCollected;

    public void ResetAllPoints()
    {
        _pointObjectFacade.ResetAllPoints();
    }

    // Enemy
    public IReadOnlyReactiveProperty<(int defeatEnemyCount, int allEnemyCount)> OnEnemyCountChanged => _enemyFacade.OnEnemyCountChanged;

    // Orbit
    public IReadOnlyReactiveProperty<(int visitedCount, int allCount)> OnSplineCountChanged => _orbitFacade.OnSplineCountChanged;

    public IObservable<Unit> OnEnemyDefeated
    => _enemyFacade.OnEnemyDefeated;

    public void ResetAllOrbits()
    {
        _orbitFacade.ResetAllOrbits();
    }

    // Audio
    public void PlaySE(SEType sEType)
    {
        _audioManager.PlaySE(sEType);
    }


}
