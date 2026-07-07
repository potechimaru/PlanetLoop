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

    ClosedSplineLine StartSpline { get; }

    void ResetAllOrbits();

}

/// <summary>
/// Orbitコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
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

    public ClosedSplineLine StartSpline => _orbitManager.StartSpline;

    public void ResetAllOrbits()
    {
        _orbitManager.ResetAllOrbits();
    }



}
