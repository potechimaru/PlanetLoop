using System;
using System.Collections.Generic;
using UnityEngine;

public class PlanetOnClosedSpline : MonoBehaviour
{
    [Header("Target Spline")]
    [SerializeField] private ClosedSplineLine spline;

    [Header("Motion")]
    [SerializeField, Min(0.01f)] private float speed = 1.0f;          // units per second
    [SerializeField] private bool clockwise = false;
    [SerializeField] private float startOffset01 = 0f;                // 0..1
    [SerializeField] private bool useLocalPlaneXY = true;             // true: XY平面 / false: XZ平面

    [Header("Sampling")]
    [SerializeField, Range(16, 2048)] private int sampleCount = 256;  // 弧長テーブル用
    [SerializeField] private bool rebuildOnStart = true;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = false;

    private readonly List<Vector3> _samples = new();
    private readonly List<float> _cumLen = new();
    private float _totalLen;
    private float _distance; // 0..totalLen

    private void Start()
    {
        if (rebuildOnStart)
            RebuildTable();

        // 初期位置
        startOffset01 = Mathf.Repeat(startOffset01, 1f);
        _distance = startOffset01 * _totalLen;
        ApplyPosition(_distance);
    }

    private void Update()
    {
        if (_totalLen <= 0.0001f) return;

        float dir = clockwise ? -1f : 1f;
        _distance = Mathf.Repeat(_distance + dir * speed * Time.deltaTime, _totalLen);
        ApplyPosition(_distance);
    }

    /// <summary>
    /// 閉曲線の弧長テーブルを再構築（splineの形状を変えたら呼ぶ）
    /// </summary>
    [ContextMenu("Rebuild Table")]
    public void RebuildTable()
    {
        if (spline == null)
        {
            Debug.LogWarning("[PlanetOnClosedSpline] spline is null.");
            _totalLen = 0f;
            return;
        }

        var pts = spline.GetSampledWorldPoints(sampleCount, useLocalPlaneXY);
        if (pts == null || pts.Count < 3)
        {
            Debug.LogWarning("[PlanetOnClosedSpline] spline points are insufficient.");
            _totalLen = 0f;
            return;
        }

        _samples.Clear();
        _cumLen.Clear();

        _samples.AddRange(pts);

        _cumLen.Add(0f);
        float acc = 0f;

        for (int i = 1; i < _samples.Count; i++)
        {
            acc += Vector3.Distance(_samples[i - 1], _samples[i]);
            _cumLen.Add(acc);
        }

        _totalLen = acc;

        if (_totalLen <= 0.0001f)
            Debug.LogWarning("[PlanetOnClosedSpline] total length is too small.");

        // 距離を範囲内へ
        _distance = Mathf.Repeat(_distance, Mathf.Max(_totalLen, 0.0001f));
    }

    private void ApplyPosition(float distance)
    {
        transform.position = EvaluateByDistance(distance);
    }

    private Vector3 EvaluateByDistance(float distance)
    {
        // 端点を含む単調増加の累積長から、distance が入る区間を二分探索
        int hi = _cumLen.Count - 1;
        int lo = 0;

        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (_cumLen[mid] < distance) lo = mid + 1;
            else hi = mid;
        }

        int i = Mathf.Clamp(lo, 1, _cumLen.Count - 1);

        float l0 = _cumLen[i - 1];
        float l1 = _cumLen[i];
        float t = (Mathf.Abs(l1 - l0) < 1e-6f) ? 0f : Mathf.InverseLerp(l0, l1, distance);

        return Vector3.LerpUnclamped(_samples[i - 1], _samples[i], t);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos || _samples.Count < 2) return;

        Gizmos.color = Color.white;
        for (int i = 1; i < _samples.Count; i++)
            Gizmos.DrawLine(_samples[i - 1], _samples[i]);
    }
}
