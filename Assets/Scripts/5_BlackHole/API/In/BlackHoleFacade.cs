using System;
using UniRx;
using UnityEngine;

public interface IBlackHoleFacade
{
    Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt);

    IObservable<Unit> OnPlayerEnteredBlackHole { get; }
    IObservable<Unit> OnPlayerExitedOuterLimit { get; }
}

public class BlackHoleFacade : IBlackHoleFacade
{
    private readonly BlackHoleGravity _blackHoleGravity;
    private readonly BlackHoleDetector _blackHoleDetector;

    public BlackHoleFacade(
        BlackHoleGravity blackHoleGravity,
        BlackHoleDetector blackHoleDetector)
    {
        _blackHoleGravity = blackHoleGravity;
        _blackHoleDetector = blackHoleDetector;
    }

    public IObservable<Unit> OnPlayerEnteredBlackHole
        => _blackHoleDetector.OnPlayerEnteredBlackHole;

    public IObservable<Unit> OnPlayerExitedOuterLimit
        => _blackHoleDetector.OnPlayerExitedOuterLimit;

    public Vector3 BendDirection(Vector3 worldPos, Vector3 dir, float dt)
    {
        return _blackHoleGravity.BendDirection(worldPos, dir, dt);
    }
}