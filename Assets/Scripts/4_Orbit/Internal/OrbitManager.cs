using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class OrbitManager : IDisposable
{
    private readonly IReadOnlyList<ClosedSplineLine> _lines;
    private readonly CompositeDisposable _disposables = new();

    private ClosedSplineLine _startSpline;

    private IEnumerable<ClosedSplineLine> TargetSplines
        => _lines.Where(line => !line.IsStartSpline);

    private int AllSplineCount => TargetSplines.Count();

    private int VisitedSplineCount
        => TargetSplines.Count(line => !line.IsNewOrbit);

    private readonly ReactiveProperty<(int visitedCount, int allCount)> _splineCount = new();

    public IReadOnlyReactiveProperty<(int visitedCount, int allCount)> SplineCount
        => _splineCount;

    public ClosedSplineLine StartSpline => _startSpline;

    public OrbitManager(IEnumerable<ClosedSplineLine> lines)
    {
        _lines = lines.ToList();

        SetupRandomStartSpline();
        NotifySplineCount();

        foreach (var line in _lines)
        {
            line.OnPlayerLanded
                .Subscribe(_ =>
                {
                    HandlePlayerLanded(line);
                })
                .AddTo(_disposables);
        }

        
    }

    private void SetupRandomStartSpline()
    {
        Debug.Log($"[OrbitManager] Created / SetupRandomStartSpline hash={GetHashCode()}");
        if (_lines == null || _lines.Count == 0)
        {
            Debug.LogWarning("[OrbitManager] No spline lines found.");
            return;
        }

        foreach (var line in _lines)
        {
            line.IsStartSpline = false;
        }

        int randomIndex =
            UnityEngine.Random.Range(0, _lines.Count);

        _startSpline = _lines[randomIndex];

        _startSpline.IsStartSpline = true;

        _startSpline.IsNewOrbit = false;

        _startSpline.ApplyStartSplineMaterial();

        //Debug.Log(
        //    $"[OrbitManager] Start spline selected : " +
        //    $"{_startSpline.SplineID}"
        //);

        Debug.Log(
        $"[OrbitManager] start name={_startSpline.name}, " +
        $"id={_startSpline.SplineID}, " +
        $"isStart={_startSpline.IsStartSpline}, " +
        $"instance={_startSpline.GetInstanceID()}"
    );
    }

    private void HandlePlayerLanded(ClosedSplineLine line)
    {
        NotifySplineCount();
    }

    private void NotifySplineCount()
    {
        _splineCount.Value =
            (VisitedSplineCount, AllSplineCount);
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

            if (!line.TrySweepHit(
                    from,
                    to,
                    radius,
                    out float distOnSpline,
                    out Vector3 pointOnSpline))
                continue;

            float sqr =
                (from - pointOnSpline).sqrMagnitude;

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

    public void Dispose()
    {
        _disposables.Dispose();
        _splineCount.Dispose();
    }
}