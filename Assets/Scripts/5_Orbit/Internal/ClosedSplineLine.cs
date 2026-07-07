using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class ClosedSplineLine : MonoBehaviour
{
    /* =====================================
     * 基本設定
     * ===================================== */

    [Header("Spline ID")]
    [SerializeField, Min(1)]
    private int splineID = 1;

    private bool _isStartSpline;
    public bool IsStartSpline
    {
        get => _isStartSpline;
        set => _isStartSpline = value;
    }

    [SerializeField] private bool _canBeStartSpline = true;

    public bool CanBeStartSpline => _canBeStartSpline;

    [Header("Spline Control Points (Local Space)")]
    [SerializeField]
    private List<Vector2> controlPoints = new()
    {
        new Vector2( 1f,  0f),
        new Vector2( 0f,  1f),
        new Vector2(-1f,  0f),
        new Vector2( 0f, -1f),
    };

    [Header("Auto Shape")]
    [SerializeField] private bool useAutoGenerateShape = false;
    [SerializeField] private AutoShapeType autoShapeType = AutoShapeType.Circle;
    [SerializeField, Min(3)] private int autoControlPointCount = 8;
    [SerializeField, Min(0.01f)] private float circleRadius = 3f;
    [SerializeField] private Vector2 ellipseRadius = new Vector2(4f, 2f);

    [Header("Rendering")]
    [SerializeField, Range(8, 256)]
    private int resolution = 64;

    [SerializeField]
    private bool updateInEditor = true;

    /* =====================================
     * ポイント周回設定
     * ===================================== */

    [Header("Point Rotation")]
    [SerializeField] private bool rotatePoints = false;

    public bool IsPointRotationEnabled => rotatePoints;

    [Tooltip("距離 / 秒")]
    [SerializeField] private float rotationSpeed = 1f;

    [Header("Line Materials")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material landingMaterial;
    [SerializeField] private Material startSplineMaterial;

    private float _distanceOffset = 0f;

    /* =====================================
     * 内部
     * ===================================== */

    private LineRenderer _lineRenderer;

    private readonly List<Vector3> _positions = new();
    private readonly List<float> _cumLen = new();
    private float _totalLength;

    private Vector3 _localCenter;

    private bool _isNewOrbit = true;
    public bool IsNewOrbit
    {
        get => _isNewOrbit;
        set
        {
            if (_isNewOrbit == value) return;

            bool wasNewOrbit = _isNewOrbit;
            _isNewOrbit = value;

            if (wasNewOrbit && !_isNewOrbit)
            {
                _onPlayerLanded.OnNext(Unit.Default);
            }
        }
    }

    [SerializeField] private Transform _spawnParent;

    private Subject<Unit> _onPlayerLanded = new();
    public IObservable<Unit> OnPlayerLanded => _onPlayerLanded;

    private enum AutoShapeType
    {
        Circle,
        Ellipse
    }

    /* =====================================
     * 初期化
     * ===================================== */

    private void OnEnable()
    {
        EnsureRenderer();
        if (Application.isPlaying && normalMaterial == null)
        {
            normalMaterial = _lineRenderer.material;
        }
        Rebuild();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying && updateInEditor)
        {
            Rebuild();
        }

        if (Application.isPlaying && rotatePoints)
        {
            RotateChildPoints();
        }
    }
#else
    private void Update()
    {
        if (rotatePoints)
        {
            RotateChildPoints();
        }
    }
#endif

    private void EnsureRenderer()
    {
        if (_lineRenderer == null)
            _lineRenderer = GetComponent<LineRenderer>();

        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = false;
    }

    public void ApplyStartSplineMaterial()
    {
        EnsureRenderer();

        if (startSplineMaterial == null)
            return;

        _lineRenderer.material = startSplineMaterial;
    }

    public void ResetOrbitState()
    {
        IsStartSpline = false;
        IsNewOrbit = true;
        SetPointRotationEnabled(false);

        EnsureRenderer();

        if (normalMaterial != null)
        {
            _lineRenderer.material = normalMaterial;
        }
    }

    /* =====================================
     * 自動形状生成
     * ===================================== */

    private void GenerateAutoControlPoints()
    {
        int count = Mathf.Max(3, autoControlPointCount);

        if (controlPoints == null)
            controlPoints = new List<Vector2>();
        else
            controlPoints.Clear();

        for (int i = 0; i < count; i++)
        {
            float angle = (Mathf.PI * 2f * i) / count;
            float x;
            float y;

            switch (autoShapeType)
            {
                case AutoShapeType.Circle:
                    x = Mathf.Cos(angle) * circleRadius;
                    y = Mathf.Sin(angle) * circleRadius;
                    break;

                case AutoShapeType.Ellipse:
                    x = Mathf.Cos(angle) * ellipseRadius.x;
                    y = Mathf.Sin(angle) * ellipseRadius.y;
                    break;

                default:
                    x = Mathf.Cos(angle) * circleRadius;
                    y = Mathf.Sin(angle) * circleRadius;
                    break;
            }

            controlPoints.Add(new Vector2(x, y));
        }
    }

    /* =====================================
     * スプライン再構築
     * ===================================== */

    public void Rebuild()
    {
        if (useAutoGenerateShape)
        {
            GenerateAutoControlPoints();
        }

        if (controlPoints == null || controlPoints.Count < 3)
            return;

        EnsureRenderer();

        _positions.Clear();
        _cumLen.Clear();

        int count = controlPoints.Count;
        int segmentResolution = Mathf.Max(1, resolution / count);

        float acc = 0f;

        for (int i = 0; i < count; i++)
        {
            Vector2 p0 = controlPoints[(i - 1 + count) % count];
            Vector2 p1 = controlPoints[i];
            Vector2 p2 = controlPoints[(i + 1) % count];
            Vector2 p3 = controlPoints[(i + 2) % count];

            for (int j = 0; j < segmentResolution; j++)
            {
                float t = j / (float)segmentResolution;

                Vector2 local2D = CatmullRom(p0, p1, p2, p3, t);
                Vector3 local3D = new(local2D.x, local2D.y, 0f);

                if (_positions.Count > 0)
                {
                    acc += Vector3.Distance(
                        _positions[_positions.Count - 1],
                        local3D);
                }

                _positions.Add(local3D);
                _cumLen.Add(acc);
            }
        }

        if (_positions.Count > 0)
        {
            acc += Vector3.Distance(_positions[^1], _positions[0]);
            _positions.Add(_positions[0]);
            _cumLen.Add(acc);
        }

        _totalLength = acc;

        _localCenter = Vector3.zero;
        int n = Mathf.Max(1, _positions.Count - 1);
        for (int i = 0; i < n; i++)
            _localCenter += _positions[i];
        _localCenter /= n;

        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());
    }

    /* =====================================
     * ポイント周回処理
     * ===================================== */

    private void RotateChildPoints()
    {
        if (_totalLength <= 0f)
            return;

        _distanceOffset += rotationSpeed * Time.deltaTime;
        _distanceOffset = Mathf.Repeat(_distanceOffset, _totalLength);

        Transform parent = _spawnParent != null ? _spawnParent : transform;

        int childCount = parent.childCount;
        if (childCount == 0)
            return;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);

            var data = child.GetComponent<SplinePointData>();
            if (data == null)
                continue;

            float d = data.BaseDistance + _distanceOffset;

            Vector3 pos = EvaluateByDistance(d);
            Vector3 normal = EvaluateNormalByDistance(d);

            child.position = pos;
            child.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        }
    }

    public void SetPointRotationEnabled(bool enabled)
    {
        rotatePoints = enabled;
    }

    /* =====================================
     * 距離API
     * ===================================== */

    public float GetTotalLength()
    {
        return _totalLength;
    }

    public Vector3 EvaluateByDistance(float distance)
    {
        if (_positions.Count < 2)
            return transform.position;

        distance = Mathf.Repeat(distance, _totalLength);

        int lo = 0;
        int hi = _cumLen.Count - 1;

        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (_cumLen[mid] < distance)
                lo = mid + 1;
            else
                hi = mid;
        }

        int i = Mathf.Clamp(lo, 1, _cumLen.Count - 1);

        float l0 = _cumLen[i - 1];
        float l1 = _cumLen[i];

        float t = Mathf.Abs(l1 - l0) < 1e-6f
            ? 0f
            : Mathf.InverseLerp(l0, l1, distance);

        Vector3 local =
            Vector3.LerpUnclamped(_positions[i - 1], _positions[i], t);

        return transform.TransformPoint(local);
    }

    private Vector3 EvaluateLocalByDistance(float distance)
    {
        if (_positions.Count < 2)
            return Vector3.zero;

        distance = Mathf.Repeat(distance, _totalLength);

        int lo = 0;
        int hi = _cumLen.Count - 1;

        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;
            if (_cumLen[mid] < distance)
                lo = mid + 1;
            else
                hi = mid;
        }

        int i = Mathf.Clamp(lo, 1, _cumLen.Count - 1);

        float l0 = _cumLen[i - 1];
        float l1 = _cumLen[i];

        float t = Mathf.Abs(l1 - l0) < 1e-6f
            ? 0f
            : Mathf.InverseLerp(l0, l1, distance);

        return Vector3.LerpUnclamped(
            _positions[i - 1],
            _positions[i],
            t);
    }

    public Vector3 EvaluateNormalByDistance(float distance)
    {
        if (_positions.Count < 2 || _totalLength <= 1e-6f)
            return transform.up;

        float eps = Mathf.Max(0.001f, _totalLength * 0.001f);

        Vector3 lp0 = EvaluateLocalByDistance(distance - eps);
        Vector3 lp1 = EvaluateLocalByDistance(distance + eps);

        Vector3 tangent = (lp1 - lp0).normalized;

        Vector3 localNormal =
            new Vector3(-tangent.y, tangent.x, 0f).normalized;

        Vector3 worldNormal =
            transform.TransformDirection(localNormal).normalized;

        Vector3 worldPos =
            transform.TransformPoint(EvaluateLocalByDistance(distance));

        Vector3 worldCenter =
            transform.TransformPoint(_localCenter);

        Vector3 toOutside =
            (worldPos - worldCenter).normalized;

        if (Vector3.Dot(worldNormal, toOutside) < 0f)
            worldNormal = -worldNormal;

        return worldNormal;
    }

    public float FindNearestDistance(Vector3 worldPos)
    {
        if (_positions.Count < 2)
            return 0f;

        float minSqrDist = float.MaxValue;
        float nearestDistance = 0f;

        for (int i = 0; i < _positions.Count - 1; i++)
        {
            Vector3 a = transform.TransformPoint(_positions[i]);
            Vector3 b = transform.TransformPoint(_positions[i + 1]);

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

                float segLen = Mathf.Sqrt(abSqr);
                float baseLen = _cumLen[i];

                nearestDistance = baseLen + segLen * t;
            }
        }

        return nearestDistance;
    }

    /* =====================================
     * Catmull-Rom
     * ===================================== */

    private Vector2 CatmullRom(
        Vector2 p0,
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    public void FlashLandingMaterial()
    {
        if (IsStartSpline) return; // スタート地点はフラッシュさせない
        if (!Application.isPlaying) return;
        EnsureRenderer();

        if (landingMaterial == null) return;

        if (normalMaterial == null)
            normalMaterial = _lineRenderer.material;

        _lineRenderer.material = landingMaterial;
    }

#if UNITY_EDITOR
    [ContextMenu("Rebuild Spline")]
    public void EditorRebuild()
    {
        Rebuild();
    }

    [ContextMenu("Generate Circle Control Points")]
    private void EditorGenerateCircle()
    {
        autoShapeType = AutoShapeType.Circle;
        GenerateAutoControlPoints();
        Rebuild();
    }

    [ContextMenu("Generate Ellipse Control Points")]
    private void EditorGenerateEllipse()
    {
        autoShapeType = AutoShapeType.Ellipse;
        GenerateAutoControlPoints();
        Rebuild();
    }
#endif

    public int SplineID => splineID;

    public bool TrySweepHit(
        Vector3 fromWorld,
        Vector3 toWorld,
        float radius,
        out float hitDistanceOnSpline,
        out Vector3 hitPointOnSpline)
    {
        hitDistanceOnSpline = 0f;
        hitPointOnSpline = Vector3.zero;

        if (_positions.Count < 2)
            return false;

        float radiusSqr = radius * radius;

        bool found = false;
        float bestMoveT = float.MaxValue;
        float bestSplineT = 0f;
        int bestSegIndex = -1;
        Vector3 bestPointOnSpline = Vector3.zero;

        for (int i = 0; i < _positions.Count - 1; i++)
        {
            Vector3 a = transform.TransformPoint(_positions[i]);
            Vector3 b = transform.TransformPoint(_positions[i + 1]);

            ClosestPtSegmentSegment(
                fromWorld,
                toWorld,
                a,
                b,
                out float moveT,
                out float splineT,
                out Vector3 c1,
                out Vector3 c2);

            float sqrDist = (c1 - c2).sqrMagnitude;
            if (sqrDist > radiusSqr)
                continue;

            if (!found || moveT < bestMoveT)
            {
                found = true;
                bestMoveT = moveT;
                bestSplineT = splineT;
                bestSegIndex = i;
                bestPointOnSpline = c2;
            }
        }

        if (!found)
            return false;

        Vector3 segLocalA = _positions[bestSegIndex];
        Vector3 segLocalB = _positions[bestSegIndex + 1];
        float segLen = Vector3.Distance(segLocalA, segLocalB);

        hitDistanceOnSpline = _cumLen[bestSegIndex] + segLen * bestSplineT;
        hitPointOnSpline = bestPointOnSpline;
        return true;
    }

    private static void ClosestPtSegmentSegment(
        Vector3 p1,
        Vector3 q1,
        Vector3 p2,
        Vector3 q2,
        out float s,
        out float t,
        out Vector3 c1,
        out Vector3 c2)
    {
        const float EPS = 1e-6f;

        Vector3 d1 = q1 - p1;
        Vector3 d2 = q2 - p2;
        Vector3 r = p1 - p2;

        float a = Vector3.Dot(d1, d1);
        float e = Vector3.Dot(d2, d2);
        float f = Vector3.Dot(d2, r);

        if (a <= EPS && e <= EPS)
        {
            s = 0f;
            t = 0f;
            c1 = p1;
            c2 = p2;
            return;
        }

        if (a <= EPS)
        {
            s = 0f;
            t = Mathf.Clamp01(f / e);
        }
        else
        {
            float c = Vector3.Dot(d1, r);

            if (e <= EPS)
            {
                t = 0f;
                s = Mathf.Clamp01(-c / a);
            }
            else
            {
                float b = Vector3.Dot(d1, d2);
                float denom = a * e - b * b;

                if (denom != 0f)
                    s = Mathf.Clamp01((b * f - c * e) / denom);
                else
                    s = 0f;

                float tNom = b * s + f;

                if (tNom < 0f)
                {
                    t = 0f;
                    s = Mathf.Clamp01(-c / a);
                }
                else if (tNom > e)
                {
                    t = 1f;
                    s = Mathf.Clamp01((b - c) / a);
                }
                else
                {
                    t = tNom / e;
                }
            }
        }

        c1 = p1 + d1 * s;
        c2 = p2 + d2 * t;
    }
}