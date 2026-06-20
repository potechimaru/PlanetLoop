using System;
using UniRx;
using UnityEngine;

public interface IEnemyFacade
{
    IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged { get; }

    void SetAllDetectionEnabled(bool isEnabled);

    IObservable<Unit> OnPlayerHitByEnemyBullet { get; }

    IObservable<Unit> OnPlayerHitByLaserBeam { get; }

    IObservable<Unit> OnEnemyDefeated { get; }

    IObservable<IEnemyContactHandle> OnPlayerTouchedEnemy { get; }

    void DefeatEnemy(IEnemyContactHandle enemyHandle);
}

public class EnemyFacade : IEnemyFacade
{
    private readonly EnemyManager _enemyManager;
    private readonly EnemyBulletManager _enemyBulletManager;
    private readonly LaserBeamManager _laserBeamManager;

    public EnemyFacade(
        EnemyManager enemyManager,
        EnemyBulletManager enemyBulletManager,
        LaserBeamManager laserBeamManager)
    {
        _enemyManager = enemyManager;
        _enemyBulletManager = enemyBulletManager;
        _laserBeamManager = laserBeamManager;
    }

    public IReadOnlyReactiveProperty<(int defeatedCount, int totalEnemyCount)> OnEnemyCountChanged
        => _enemyManager.EnemyCount;

    public void SetAllDetectionEnabled(bool isEnabled)
    {
        _enemyManager.SetAllDetectionEnabled(isEnabled);
    }

    public IObservable<Unit> OnPlayerHitByEnemyBullet
        => _enemyBulletManager.OnPlayerHitByEnemyBullet;

    public IObservable<Unit> OnPlayerHitByLaserBeam
        => _laserBeamManager.OnPlayerHitByLaser;

    public IObservable<Unit> OnEnemyDefeated
    => _enemyManager.OnEnemyDefeated;

    public IObservable<IEnemyContactHandle> OnPlayerTouchedEnemy
        => _enemyManager.OnPlayerTouchedEnemy;

    public void DefeatEnemy(IEnemyContactHandle enemyHandle)
    {
        _enemyManager.DefeatEnemy(enemyHandle);
    }
}