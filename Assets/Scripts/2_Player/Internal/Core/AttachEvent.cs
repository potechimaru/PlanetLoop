using System;
using UniRx;
using UnityEngine;

public class AttachEvent
{
    private readonly PlayerView _playerView;
    private readonly PlayerModel _playerModel;

    private readonly Subject<Unit> _onLongJumped;
    private readonly Subject<Vector3> _onNewOrbitAttached;


    internal AttachEvent(
        PlayerView playerView,
        PlayerModel playerModel,
        Subject<Unit> onLongJumped,
        Subject<Vector3> onNewOrbitAttached)
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
        Debug.Log(
            $"[AttachEvent] attached name={currentSpline.name}, " +
            $"id={currentSpline.SplineID}, " +
            $"isStart={currentSpline.IsStartSpline}, " +
            $"instance={currentSpline.GetInstanceID()}"
        );

        if (!currentSpline.IsStartSpline)
        {
            Debug.Log($"IsStartSpline : {currentSpline.IsStartSpline}");
            CheckNewOrbitAttached(currentSpline, playerWorldPos);
        }

        _playerView.PlaySplineAttachFx(currentSpline, distance, playerWorldPos);

        currentSpline.IsNewOrbit = false;
    }

    internal void CheckLongJumped(float jumpDistance)
    {
        if (jumpDistance >= _playerModel.LongJumpDistanceThreshold)
        {
            //Debug.Log($"Long Jumped! Distance: {jumpDistance}");
            _onLongJumped.OnNext(Unit.Default);
        }
    }

    private void CheckNewOrbitAttached(
        ClosedSplineLine currentSpline,
        Vector3 playerWorldPos)
    {
        if (!currentSpline.IsNewOrbit) return;

        Debug.Log(
            $"[CheckNewOrbitAttached] attached name={currentSpline.name}, " +
            $"id={currentSpline.SplineID}, " +
            $"isStart={currentSpline.IsStartSpline}, " +
            $"instance={currentSpline.GetInstanceID()}"
        );

        currentSpline.FlashLandingMaterial();
        _onNewOrbitAttached.OnNext(playerWorldPos);
        //Debug.Log("New Orbit Attached!");
    }
}