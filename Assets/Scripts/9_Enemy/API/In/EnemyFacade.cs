using System;
using UniRx;

public interface IEnemyFacade
{
    IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged { get; }

    IObservable<Unit> OnPlayerHitByEnemyBullet { get; }
}

public class EnemyFacade : IEnemyFacade
{
    private readonly EnemyManager _enemyManager;
    private readonly EnemyBulletManager _enemyBulletManager;

    public EnemyFacade(
        EnemyManager enemyManager,
        EnemyBulletManager enemyBulletManager)
    {
        _enemyManager = enemyManager;
        _enemyBulletManager = enemyBulletManager;
    }

    public IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged
        => _enemyManager.EnemyCount;

    public IObservable<Unit> OnPlayerHitByEnemyBullet
        => _enemyBulletManager.OnPlayerHitByEnemyBullet;
}