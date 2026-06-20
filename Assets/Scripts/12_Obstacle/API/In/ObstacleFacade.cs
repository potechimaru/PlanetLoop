using System;
using UniRx;

public interface IObstacleFacade
{
    IObservable<Unit> OnPlayerHitObstacle { get; }
}

public class ObstacleFacade : IObstacleFacade
{
    private readonly ObstacleManager _obstacleManager;

    public ObstacleFacade(ObstacleManager obstacleManager)
    {
        _obstacleManager = obstacleManager;
    }

    public IObservable<Unit> OnPlayerHitObstacle =>
        _obstacleManager.OnPlayerHitObstacle;
}