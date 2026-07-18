using System;
using System.Collections.Generic;
using UniRx;

/// <summary>
/// EnemyBullet���Ǘ�����N���X�B�v���C���[��EnemyBullet�ɓ��������Ƃ��̃C�x���g��ʒm����B
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