using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class OrbitManager : IDisposable
{
    private readonly IReadOnlyList<ClosedSplineLine> _lines;
    private readonly CompositeDisposable _disposables = new();

    private int AllSplineCount => _lines.Count;
    private int VisitedSplineCount => _lines.Count(line => !line.IsNewOrbit);

    private readonly ReactiveProperty<(int visitedCount, int allCount)> _splineCount = new();
    public IReadOnlyReactiveProperty<(int visitedCount, int allCount)> SplineCount => _splineCount;

    public OrbitManager(IEnumerable<ClosedSplineLine> lines)
    {
        _lines = lines.ToList();

        foreach (var line in _lines)
        {
            line.OnPlayerLanded
                .Subscribe(_ =>
                {
                    HandlePlayerLanded(line);
                })
                .AddTo(_disposables);
        }

        NotifySplineCount();
    }

    private void HandlePlayerLanded(ClosedSplineLine line)
    {
        //Debug.Log($"Player landed on spline {line.SplineID}");
        //Debug.Log($"Visited {VisitedSplineCount} / {AllSplineCount}");

        NotifySplineCount();
    }

    private void NotifySplineCount()
    {
        Debug.Log($"NotifySplineCount called. Visited: {VisitedSplineCount}, All: {AllSplineCount}");
        _splineCount.Value = (VisitedSplineCount, AllSplineCount);
    }

    public bool TryFindTouchedSpline(
    Vector3 from,
    Vector3 to,
    float radius,
    ClosedSplineLine exclude,
    out ClosedSplineLine result,
    out float hitDistanceOnSpline,
    out Vector3 hitPointOnSpline)
    {
        result = null;
        hitDistanceOnSpline = 0f;
        hitPointOnSpline = Vector3.zero;

        bool found = false;
        float bestMoveSqr = float.MaxValue;

        foreach (var line in _lines)
        {
            if (line == exclude)
                continue;

            if (!line.TrySweepHit(from, to, radius, out float distOnSpline, out Vector3 pointOnSpline))
                continue;

            float sqr = (from - pointOnSpline).sqrMagnitude;
            if (!found || sqr < bestMoveSqr)
            {
                found = true;
                bestMoveSqr = sqr;
                result = line;
                hitDistanceOnSpline = distOnSpline;
                hitPointOnSpline = pointOnSpline;
            }
        }

        return found;
    }


    private bool IsTouchingSpline(
        ClosedSplineLine spline,
        Vector3 worldPos,
        float radius)
    {
        float totalLen = spline.GetTotalLength();

        const int sampleCount = 128;
        float step = totalLen / sampleCount;

        for (int i = 0; i < sampleCount; i++)
        {
            float d = step * i;
            Vector3 p = spline.EvaluateByDistance(d);

            if ((p - worldPos).sqrMagnitude <= radius * radius)
                return true;
        }

        return false;
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _splineCount.Dispose();
    }
}