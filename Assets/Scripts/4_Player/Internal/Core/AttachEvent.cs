using System;
using UniRx;
using UnityEngine;

/// <summary>
/// PlayerControllerの拡張。PlayerがSplineにアタッチされたときのイベントを管理するクラス
/// </summary>
public class AttachEvent
{
    private readonly PlayerView _playerView;
    private readonly PlayerModel _playerModel;

    private readonly Subject<Unit> _onLongJumped;
    private readonly Subject<Unit> _onNewOrbitAttached;


    internal AttachEvent(
        PlayerView playerView,
        PlayerModel playerModel,
        Subject<Unit> onLongJumped,
        Subject<Unit> onNewOrbitAttached)
    {
        _playerModel = playerModel;
        _playerView = playerView;
        _onLongJumped = onLongJumped;
        _onNewOrbitAttached = onNewOrbitAttached;

    }

    /// <summary>
    /// SplineにPlayerが飛び乗った時の処理。乗ったことの通知を飛ばしたり、エフェクトを再生する。
    /// また、新しいSplineであれば、Splineが新しいことを通知する。
    /// </summary>
    /// <param name="currentSpline">飛び乗ったSpline</param>
    /// <param name="distance">Splineの始点から何メートル地点にいるか</param>
    /// <param name="playerWorldPos">Splineに飛び乗った地点でのPlayerの位置</param>
    internal void OnSplineAttached(
        ClosedSplineLine currentSpline,
        float distance,
        Vector3 playerWorldPos)
    {

        // 新しいSplineに飛び乗った場合、Splineが新しいことを通知し、専用のエフェクトを再生する
        if (!currentSpline.IsStartSpline)
        {
            CheckNewOrbitAttached(currentSpline);
        }

        // PlayerがSplineに飛び乗った時のエフェクトを再生する
        _playerView.PlaySplineAttachFx(currentSpline, distance, playerWorldPos);

        currentSpline.IsNewOrbit = false;
    }

    /// <summary>
    /// ジャンプ距離が長い場合に、LongJumpしたことを通知する
    /// </summary>
    /// <param name="jumpDistance"></param>
    internal void CheckLongJumped(float jumpDistance)
    {
        if (jumpDistance >= _playerModel.LongJumpDistanceThreshold)
        {
            _onLongJumped.OnNext(Unit.Default);
        }
    }

    /// <summary>
    /// 新しいSplineに飛び乗った場合、Splineが新しいことを通知し、専用のエフェクトを再生する
    /// </summary>
    /// <param name="currentSpline"></param>
    private void CheckNewOrbitAttached(
        ClosedSplineLine currentSpline)
    {
        if (!currentSpline.IsNewOrbit) return;

        // 新しいSplineに飛び乗った場合、Splineが新しいことを通知し、専用のエフェクトを再生する
        currentSpline.FlashLandingMaterial();
        _onNewOrbitAttached.OnNext(Unit.Default);
    }
}