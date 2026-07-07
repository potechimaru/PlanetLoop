using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public sealed class EnemyController
{
    private readonly Transform _self;
    private readonly Transform _player;
    private readonly EnemyView _view;
    private readonly EnemyConfig _config;
    private readonly EnemyBulletFactory _bulletFactory;
    private readonly EnemyLaserFactory _laserFactory;
    private readonly EnemyAttackType _enemyAttackType;

    public float TelegraphElapsed;
    public float CooldownElapsed;

    public readonly Vector3 Origin;
    public Vector3 WanderTarget;

    public bool IsDetectionEnabled { get; private set; } = true;

    public EnemyController(
        Transform self,
        Transform player,
        EnemyView view,
        EnemyConfig config,
        EnemyBulletFactory bulletFactory,
        EnemyLaserFactory laserFactory,
        EnemyAttackType enemyAttackType)
    {
        _self = self;
        _player = player;
        _view = view;
        _config = config;
        _bulletFactory = bulletFactory;
        _laserFactory = laserFactory;
        _enemyAttackType = enemyAttackType;

        Origin = self.position;
        WanderTarget = Origin;
    }

    public Vector3 DirToPlayerNormalized()
    {
        if (_player == null) return Vector3.right;
        var d = (_player.position - _self.position);
        d.z = 0f;
        return d.sqrMagnitude > 0.0001f ? d.normalized : Vector3.right;
    }

    public void StopRotateDecoration()
    {
        if (_view == null) return;
        _view.StopRotateDecoration();
    }

    public void RotateDecoration()
    {
        if (_view == null) return;
        _view.RotateDecoration();
    }

    public void ShowTelegraph(Vector3 dirNormalized)
    {
        if (_view == null) return;
        _view.ShowTelegraph(dirNormalized);
        //_enemyExternalFacade.PlaySE(SEType.EnemyLockOn);
    }

    public void HideTelegraph()
    {
        if (_view == null) return;
        _view.HideTelegraph();
    }

    public void ShowTelegraphs(IReadOnlyList<Vector3> directions)
    {
        if (_view == null) return;
        _view.ShowTelegraphs(directions);
        //_enemyExternalFacade.PlaySE(SEType.EnemyLockOn);
    }

    public void HideTelegraphs()
    {
        if (_view == null) return;
        _view.HideTelegraphs();
    }

    public void SetDetectionEnabled(bool enabled)
    {
        IsDetectionEnabled = enabled;
    }

    public void FireBullet(Vector3 dirNormalized)
    {
        if (_view == null) return;
        _bulletFactory.Spawn(_enemyAttackType, _self.position, dirNormalized);
    }

    public void FireLaser(Vector3 dirNormalized)
    {
        if (_view == null) return;
        _laserFactory.SpawnAsync(_self.position, dirNormalized).Forget();
    }

    public async UniTask PlayDisappearAnimationAsync()
    {
        await _view.PlayDisappearAnimationAsync();
    }

    public async UniTask PlayDisappearParticleAsync()
    {
        await _view.PlayDisappearParticleAsync();
    }

    public Transform Self => _self;
    public Transform Player => _player;
    public float DetectRadius => _config.DetectRadius;
    public float TelegraphTime => _config.TelegraphTime;
    public float CooldownTime => _config.CooldownTime;
    public float MoveSpeed => _config.MoveSpeed;
}