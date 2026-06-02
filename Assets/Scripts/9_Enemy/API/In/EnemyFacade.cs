using System;
using UniRx;
using UnityEngine;

public interface IEnemyFacade
{
    IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged { get; }

    void SetAllDetectionEnabled(bool isEnabled);

    IObservable<Unit> OnPlayerHitByEnemyBullet { get; }

    IObservable<Unit> OnEnemyDefeated { get; }
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

    public void SetAllDetectionEnabled(bool isEnabled)
    {
        _enemyManager.SetAllDetectionEnabled(isEnabled);
    }

    public IObservable<Unit> OnPlayerHitByEnemyBullet
        => _enemyBulletManager.OnPlayerHitByEnemyBullet;

    public IObservable<Unit> OnEnemyDefeated
    => _enemyManager.OnEnemyDefeated;
}