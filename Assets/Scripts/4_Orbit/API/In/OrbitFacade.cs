using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public interface IOrbitFacade
{
    bool TryFindTouchedSpline(
    Vector3 from,
    Vector3 to,
    float radius,
    ClosedSplineLine exclude,
    out ClosedSplineLine touchedSpline,
    out float hitDistanceOnSpline,
    out Vector3 hitPointOnSpline);

    IReadOnlyReactiveProperty<(int visitedCount, int allCount)> OnSplineCountChanged { get; }

}
public class OrbitFacade : IOrbitFacade
{
    private readonly OrbitManager _orbitManager;
    public OrbitFacade(OrbitManager orbitManager)
    {
        _orbitManager = orbitManager;
    }

    public bool TryFindTouchedSpline(
    Vector3 from,
    Vector3 to,
    float radius,
    ClosedSplineLine exclude,
    out ClosedSplineLine touchedSpline,
    out float hitDistanceOnSpline,
    out Vector3 hitPointOnSpline)
    {
        return _orbitManager.TryFindTouchedSpline(
            from,
            to,
            radius,
            exclude,
            out touchedSpline,
            out hitDistanceOnSpline,
            out hitPointOnSpline);
    }

    public IReadOnlyReactiveProperty<(int visitedCount, int allCount)> OnSplineCountChanged => _orbitManager.SplineCount;



}
