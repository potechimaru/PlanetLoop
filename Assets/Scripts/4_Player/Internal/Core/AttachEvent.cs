using System;
using UniRx;
using UnityEngine;

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

    internal void OnSplineAttached(
        ClosedSplineLine currentSpline,
        float distance,
        Vector3 playerWorldPos)
    {

        if (!currentSpline.IsStartSpline)
        {
            CheckNewOrbitAttached(currentSpline);
        }

        _playerView.PlaySplineAttachFx(currentSpline, distance, playerWorldPos);

        currentSpline.IsNewOrbit = false;
    }

    internal void CheckLongJumped(float jumpDistance)
    {
        if (jumpDistance >= _playerModel.LongJumpDistanceThreshold)
        {
            _onLongJumped.OnNext(Unit.Default);
        }
    }

    private void CheckNewOrbitAttached(
        ClosedSplineLine currentSpline)
    {
        if (!currentSpline.IsNewOrbit) return;

        Debug.Log(
            $"[CheckNewOrbitAttached] attached name={currentSpline.name}, " +
            $"id={currentSpline.SplineID}, " +
            $"isStart={currentSpline.IsStartSpline}, " +
            $"instance={currentSpline.GetInstanceID()}"
        );

        currentSpline.FlashLandingMaterial();
        _onNewOrbitAttached.OnNext(Unit.Default);
    }
}