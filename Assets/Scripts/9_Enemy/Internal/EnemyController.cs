using UnityEngine;
using UniRx;
using System;
using Cysharp.Threading.Tasks;

public sealed class EnemyController
{
    private readonly Transform _self;
    private readonly Transform _player;
    private readonly EnemyView _view;
    private readonly EnemyConfig _config;
    private readonly EnemyBulletFactory _bulletFactory;
    private readonly EnemyAttackType _enemyAttackType;

    public float TelegraphElapsed;
    public float CooldownElapsed;

    public readonly Vector3 Origin;
    public Vector3 WanderTarget;

    public EnemyController(
        Transform self,
        Transform player,
        EnemyView view,
        EnemyConfig config,
        EnemyBulletFactory bulletFactory,
        EnemyAttackType enemyAttackType)
    {
        _self = self;
        _player = player;
        _view = view;
        _config = config;
        _bulletFactory = bulletFactory;
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
    }

    public void HideTelegraph()
    {
        if (_view == null) return;
        _view.HideTelegraph();
    }

    public void FireBullet(Vector3 dirNormalized)
    {
        if (_view == null) return;
        _bulletFactory.Spawn(_enemyAttackType, _self.position, dirNormalized);
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