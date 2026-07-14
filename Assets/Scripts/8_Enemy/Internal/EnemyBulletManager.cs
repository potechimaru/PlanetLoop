using System;
using System.Collections.Generic;
using UniRx;

/// <summary>
/// EnemyBulletを管理するクラス。プレイヤーがEnemyBulletに当たったときのイベントを通知する。
/// </summary>
public sealed class EnemyBulletManager : IDisposable
{
    private readonly Subject<Unit> _onPlayerHitByEnemyBullet = new();
    private readonly CompositeDisposable _disposables = new();

    private readonly HashSet<EnemyBullet> _registeredBullets = new();

    public IObservable<Unit> OnPlayerHitByEnemyBullet
        => _onPlayerHitByEnemyBullet;

    public void RegisterBullet(EnemyBullet bullet)
    {
        if (bullet == null) return;

        // Poolで同じ弾を敍利用する晝插戡二拇Subscribe防止
        if (!_registeredBullets.Add(bullet)) return;

        bullet.OnHitPlayer
            .Subscribe(_ =>
            {
                _onPlayerHitByEnemyBullet.OnNext(Unit.Default);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _onPlayerHitByEnemyBullet.Dispose();
        _registeredBullets.Clear();
    }
}