using System;
using System.Collections.Generic;
using UnityEngine;

internal delegate bool TryFindTouchedSplineFunc(
    Vector3 pos,
    float radius,
    ClosedSplineLine exclude,
    out ClosedSplineLine result
);

internal class PlayerSplineMover
{
    private const float AttachRadius = 0.1f;

    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    private readonly TryFindTouchedSplineFunc _tryFindTouchedSpline;

    private ClosedSplineLine _currentSpline;

    private readonly List<Vector3> _samples = new();
    private readonly List<float> _cumLen = new();

    private float _totalLen;
    private float _distance;
    private Vector3 _center;

    private bool _isAttaching;
    private float _attachT;
    private Vector3 _attachFrom;
    private Vector3 _attachTo;
    private const float AttachDuration = 0.03f;

    internal PlayerSplineMover(
        PlayerView view,
        PlayerModel model,
        TryFindTouchedSplineFunc tryFindTouchedSpline)
    {
        _view = view;
        _model = model;
        _tryFindTouchedSpline = tryFindTouchedSpline;

        _currentSpline = _view.Spline;
    }

    public void Initialize()
    {
        RebuildTable();

        if (_totalLen <= 0.0001f)
        {
            Debug.LogError(
                "[PlayerSplineMover] Initialize failed: spline samples are empty");
        }
    }

    /* =========================
     * í èÌé¸âÒ
     * ========================= */

    public void Tick(float deltaTime)
    {
        if (_model.IsGameOver) return;
        if (_totalLen <= 0.0001f) return;

        // Åö ãzíÖíÜÇÕLerpÇ≈à íuÇäÒÇπÇÈ
        if (_isAttaching)
        {
            _attachT += deltaTime / AttachDuration;
            float t = Mathf.SmoothStep(0f, 1f, _attachT);

            Vector3 pos = Vector3.Lerp(_attachFrom, _attachTo, t);
            _view.SetPosition(pos);

            if (_attachT >= 1f)
            {
                _isAttaching = false;
            }
            return;
        }

        float dir = _model.Clockwise ? -1f : 1f;
        _distance = Mathf.Repeat(
            _distance + dir * _model.MoveSpeed * deltaTime,
            _totalLen
        );

        ApplyPosition();
    }


    /* =========================
     * Jump
     * ========================= */

    public void Jump()
    {
        if (_model.IsGameOver)
            return;

        Vector3 normal = GetOuterNormal();

        _view.StartJump(
            _view.transform.position,
            normal,
            _model.Jumpspeed
        );
    }

    public bool TickJumpAndCheckAttach(Vector3 playerWorldPos)
    {
        //Debug.Log("TickJump");


        //  OrbitManagerÇ…ëºÇÃOrbitÇ…êGÇÍÇΩÇ©ñ‚Ç¢çáÇÌÇπÇÈ
        if (_tryFindTouchedSpline(
            playerWorldPos,
            AttachRadius,
            _currentSpline,
            out var touchedSpline))
        {
            //Debug.Log("AttachToSpline");
            //Debug.Log("TryFindTouchedSpline : true");
            AttachToSpline(touchedSpline, playerWorldPos);
            return true;
        }
        return false;

    }

    /* =========================
     * çƒãzíÖ
     * ========================= */

    private void AttachToSpline(
    ClosedSplineLine newSpline,
    Vector3 playerWorldPos)
    {
        _view.SetSpline(newSpline);
        _currentSpline = newSpline;

        RebuildTable();

        _distance = FindNearestDistance(playerWorldPos);

        // Åö ãzíÖÉAÉjÉÅÅ[ÉVÉáÉìèÄîı
        _attachFrom = _view.transform.position;
        _attachTo = EvaluateByDistance(_distance);

        _attachT = 0f;
        _isAttaching = true;
    }

    /* =========================
     * Spline Table
     * ========================= */

    private void RebuildTable()
    {
        if (_currentSpline == null)
        {
            _totalLen = 0f;
            return;
        }

        _samples.Clear();
        _cumLen.Clear();

        _samples.AddRange(_currentSpline.CollisionSamples);

        if (_samples.Count < 3)
        {
            _totalLen = 0f;
            return;
        }

        float acc = 0f;
        _cumLen.Add(0f);

        for (int i = 1; i < _samples.Count; i++)
        {
            acc += Vector3.Distance(_samples[i - 1], _samples[i]);
            _cumLen.Add(acc);
        }

        _totalLen = acc;
        _distance = Mathf.Repeat(_distance, Mathf.Max(_totalLen, 0.0001f));

        _center = Vector3.zero;
        foreach (var p in _samples)
            _center += p;
        _center /= _samples.Count;
    }

    private void ApplyPosition()
    {
        _view.SetPosition(EvaluateByDistance(_distance));
    }

    private Vector3 EvaluateByDistance(float distance)
    {
        int lo = 0;
        int hi = _cumLen.Count - 1;

        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (_cumLen[mid] < distance) lo = mid + 1;
            else hi = mid;
        }

        int i = Mathf.Clamp(lo, 1, _cumLen.Count - 1);

        float l0 = _cumLen[i - 1];
        float l1 = _cumLen[i];
        float t = Mathf.Abs(l1 - l0) < 1e-6f
            ? 0f
            : Mathf.InverseLerp(l0, l1, distance);

        return Vector3.LerpUnclamped(_samples[i - 1], _samples[i], t);
    }

    /* =========================
     * äÙâΩ
     * ========================= */

    public Vector3 GetOuterNormal()
    {
        const float epsilon = 0.01f;

        float d0 = Mathf.Repeat(_distance - epsilon, _totalLen);
        float d1 = Mathf.Repeat(_distance + epsilon, _totalLen);

        Vector3 p0 = EvaluateByDistance(d0);
        Vector3 p1 = EvaluateByDistance(d1);

        Vector3 tangent = (p1 - p0).normalized;
        Vector3 normal;

        if (_view.UseLocalPlaneXY)
            normal = new Vector3(-tangent.y, tangent.x, 0f);
        else
            normal = new Vector3(-tangent.z, 0f, tangent.x);

        Vector3 toCenter = (_center - _view.transform.position).normalized;
        if (Vector3.Dot(normal, toCenter) > 0f)
            normal = -normal;

        return normal.normalized;
    }

    private float FindNearestDistance(Vector3 worldPos)
    {
        float minSqrDist = float.MaxValue;
        float nearestDistance = 0f;

        for (int i = 0; i < _samples.Count - 1; i++)
        {
            Vector3 a = _samples[i];
            Vector3 b = _samples[i + 1];

            Vector3 ab = b - a;
            float abSqr = ab.sqrMagnitude;
            if (abSqr < 1e-6f)
                continue;

            float t = Vector3.Dot(worldPos - a, ab) / abSqr;
            t = Mathf.Clamp01(t);

            float sqrDist = (worldPos - (a + ab * t)).sqrMagnitude;

            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;
                nearestDistance = _cumLen[i] + Mathf.Sqrt(abSqr) * t;
            }
        }

        return nearestDistance;
    }
}
