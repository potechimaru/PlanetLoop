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

    //private readonly List<Vector3> _samples = new();
    //private readonly List<float> _cumLen = new();

    private float _totalLen;
    private float _distance;
    //private Vector3 _center;

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
            _distance + dir * _model.CurrentMoveSpeed * deltaTime,
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
            _model.CurrentJumpspeed
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

        _distance = _currentSpline.FindNearestDistance(playerWorldPos);

        _totalLen = _currentSpline.GetTotalLength();

        _attachFrom = _view.transform.position;
        _attachTo = _currentSpline.EvaluateByDistance(_distance);

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

        _totalLen = _currentSpline.GetTotalLength();
    }


    private void ApplyPosition()
    {
        Vector3 pos = _currentSpline.EvaluateByDistance(_distance);
        _view.SetPosition(pos);
    }



    /* =========================
     * äÙâΩ
     * ========================= */

    public Vector3 GetOuterNormal()
    {
        return _currentSpline.EvaluateNormalByDistance(_distance);
    }


    //private float FindNearestDistance(Vector3 worldPos)
    //{
    //    float minSqrDist = float.MaxValue;
    //    float nearestDistance = 0f;

    //    for (int i = 0; i < _samples.Count - 1; i++)
    //    {
    //        Vector3 a = _samples[i];
    //        Vector3 b = _samples[i + 1];

    //        Vector3 ab = b - a;
    //        float abSqr = ab.sqrMagnitude;
    //        if (abSqr < 1e-6f)
    //            continue;

    //        float t = Vector3.Dot(worldPos - a, ab) / abSqr;
    //        t = Mathf.Clamp01(t);

    //        float sqrDist = (worldPos - (a + ab * t)).sqrMagnitude;

    //        if (sqrDist < minSqrDist)
    //        {
    //            minSqrDist = sqrDist;
    //            nearestDistance = _cumLen[i] + Mathf.Sqrt(abSqr) * t;
    //        }
    //    }

    //    return nearestDistance;
    //}
}
