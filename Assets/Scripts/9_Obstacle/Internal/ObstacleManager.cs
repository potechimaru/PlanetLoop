using System;
using System.Collections.Generic;
using UniRx;

/// <summary>
/// Obstacleを管理するクラス。プレイヤーが障害物に衝突した際のイベントを提供する。
/// </summary>
public class ObstacleManager : IDisposable
{
    private readonly IEnumerable<Obstacle> _obstacles;

    private readonly Subject<Unit> _onPlayerHitObstacle = new();
    public IObservable<Unit> OnPlayerHitObstacle => _onPlayerHitObstacle;

    private readonly CompositeDisposable _disposables = new();

    public ObstacleManager(IEnumerable<Obstacle> obstacles)
    {
        _obstacles = obstacles;

        RegisterObstacleSubscriptions();
    }

    private void RegisterObstacleSubscriptions()
    {
        foreach (var obstacle in _obstacles)
        {
            if (obstacle == null) continue;

            obstacle.OnPlayerHit
                .Subscribe(hitObstacle =>
                {
                    _onPlayerHitObstacle.OnNext(Unit.Default);
                })
                .AddTo(_disposables);
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();

        _onPlayerHitObstacle.OnCompleted();
        _onPlayerHitObstacle.Dispose();
    }
}