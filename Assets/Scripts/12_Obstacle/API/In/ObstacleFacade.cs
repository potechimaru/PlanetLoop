using System;
using UniRx;

public interface IObstacleFacade
{
    IObservable<Unit> OnPlayerHitObstacle { get; }
}

/// <summary>
/// Obstacleコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
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