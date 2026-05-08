using System;
using UnityEngine;
using UniRx;

public interface IUIExternalFacade
{

    //Player
    IObservable<Vector3> OnNewOrbitAttached { get; }
    IObservable<Unit> OnLongJumped { get; }

    // PointObject
    IObservable<PointObjectType> OnPointCollected { get; }

    // Orbit
    IReadOnlyReactiveProperty<(int visitedCount, int allCount)> OnSplineCountChanged { get; }

    // Enemy
    IReadOnlyReactiveProperty<(int defeatEnemyCount, int allEnemyCount)> OnEnemyCountChanged { get; }

}
public class UIExternalFacade : IUIExternalFacade
{
    private readonly IPointObjectFacade _pointObjectFacade;
    private readonly IOrbitFacade _orbitFacade;
    private readonly IEnemyFacade _enemyFacade;
    private readonly IPlayerFacade _playerFacade;
    private readonly IGameStateFacade _gameStateFacade;

    public UIExternalFacade(IPointObjectFacade pointObjectFacade, IOrbitFacade orbitFacade, IEnemyFacade enemyFacade, IPlayerFacade playerFacade, IGameStateFacade gameStateFacade)
    {
        _pointObjectFacade = pointObjectFacade;
        _orbitFacade = orbitFacade;
        _enemyFacade = enemyFacade;
        _playerFacade = playerFacade;
        _gameStateFacade = gameStateFacade;
    }

    // Player
    public IObservable<Vector3> OnNewOrbitAttached => _playerFacade.OnNewOrbitAttached;
    public IObservable<Unit> OnLongJumped => _playerFacade.OnLongJumped;

    // PointObject
    public IObservable<PointObjectType> OnPointCollected => _pointObjectFacade.OnPointCollected;

    // Enemy
    public IReadOnlyReactiveProperty<(int defeatEnemyCount, int allEnemyCount)> OnEnemyCountChanged => _enemyFacade.OnEnemyCountChanged;

    // Orbit
    public IReadOnlyReactiveProperty<(int visitedCount, int allCount)> OnSplineCountChanged => _orbitFacade.OnSplineCountChanged;



}
