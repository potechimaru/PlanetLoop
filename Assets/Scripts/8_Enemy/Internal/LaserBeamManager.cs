using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// Enemy5のレーザービームを管理するクラス。レーザービームの登録とプレイヤーへのヒットイベントを管理する。
/// </summary>
public sealed class LaserBeamManager : IDisposable
{
    private readonly Subject<Unit> _onPlayerHitByLaser = new();
    private readonly CompositeDisposable _disposables = new();

    private readonly HashSet<LaserBeam> _registeredLasers = new();

    public IObservable<Unit> OnPlayerHitByLaser
        => _onPlayerHitByLaser;

    public void RegisterLaser(LaserBeam laser)
    {
        if (laser == null) return;

        // Pool再利用時の二重Subscribe防止
        if (!_registeredLasers.Add(laser)) return;

        laser.OnHitPlayer
            .Subscribe(_ =>
            {
                _onPlayerHitByLaser.OnNext(Unit.Default);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _onPlayerHitByLaser.Dispose();
        _registeredLasers.Clear();
    }
}