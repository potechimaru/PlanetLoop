using System.Collections.Generic;
using UnityEngine;

internal class PlayerSplineMover
{
    private readonly PlayerView _view;
    private readonly PlayerModel _model;

    private readonly List<Vector3> _samples = new();
    private readonly List<float> _cumLen = new();

    private float _totalLen;
    private float _distance;

    private Vector3 _center;   // ★ 追加：閉曲線の中心

    private const int SampleCount = 256;

    internal PlayerSplineMover(PlayerView view, PlayerModel model)
    {
        _view = view;
        _model = model;

        RebuildTable();
        _distance = 0f;
        ApplyPosition();
    }

    public void Tick(float deltaTime)
    {
        if (_totalLen <= 0.0001f) return;
        if (_model.IsGameOver) return;
        if (_model.IsJumping) return;

        float dir = _model.Clockwise ? -1f : 1f;
        _distance = Mathf.Repeat(
            _distance + dir * _model.MoveSpeed * deltaTime,
            _totalLen
        );

        ApplyPosition();
    }

    public void RebuildTable()
    {
        var spline = _view.Spline;
        if (spline == null)
        {
            _totalLen = 0f;
            return;
        }

        var pts = spline.GetSampledWorldPoints(SampleCount, _view.UseLocalPlaneXY);
        if (pts == null || pts.Count < 3)
        {
            _totalLen = 0f;
            return;
        }

        _samples.Clear();
        _cumLen.Clear();
        _samples.AddRange(pts);

        // --- 累積長 ---
        float acc = 0f;
        _cumLen.Add(0f);

        for (int i = 1; i < _samples.Count; i++)
        {
            acc += Vector3.Distance(_samples[i - 1], _samples[i]);
            _cumLen.Add(acc);
        }

        _totalLen = acc;
        _distance = Mathf.Repeat(_distance, Mathf.Max(_totalLen, 0.0001f));

        // 中心点（重心）を計算
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
        float t = Mathf.Abs(l1 - l0) < 1e-6f ? 0f : Mathf.InverseLerp(l0, l1, distance);

        return Vector3.LerpUnclamped(_samples[i - 1], _samples[i], t);
    }

    public void Jump()
    {
        if (_model.IsJumping || _model.IsGameOver)
            return;

        _model.IsJumping = true;

        Vector3 normal = GetOuterNormal();

        _view.StartJump(
            _view.transform.position,
            normal,
            _model.MoveSpeed,
            _model.Jumpspeed
        );
    }

    /// <summary>
    /// 常に「閉曲線の外側」を向く法線
    /// </summary>
    private Vector3 GetOuterNormal()
    {
        const float epsilon = 0.01f;

        float d0 = Mathf.Repeat(_distance - epsilon, _totalLen);
        float d1 = Mathf.Repeat(_distance + epsilon, _totalLen);

        Vector3 p0 = EvaluateByDistance(d0);
        Vector3 p1 = EvaluateByDistance(d1);

        Vector3 tangent = (p1 - p0).normalized;

        Vector3 normal;

        if (_view.UseLocalPlaneXY)
        {
            normal = new Vector3(-tangent.y, tangent.x, 0f).normalized;
        }
        else
        {
            normal = new Vector3(-tangent.z, 0f, tangent.x).normalized;
        }

        // 外側判定
        Vector3 toCenter = (_center - _view.transform.position).normalized;

        // 内向きなら反転
        if (Vector3.Dot(normal, toCenter) > 0f)
            normal = -normal;

        return normal;
    }
}
