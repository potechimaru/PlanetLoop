using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ClosedSplineLine : MonoBehaviour
{
    [Header("Spline ID")]
    [SerializeField, Min(1)] private int splineID = 1;

    [Header("Spline Control Points (Local Space)")]
    [SerializeField]
    private List<Vector2> controlPoints = new()
    {
        new Vector2( 1f,  0f),
        new Vector2( 0f,  1f),
        new Vector2(-1f,  0f),
        new Vector2( 0f, -1f),
    };

    [Header("Rendering")]
    [SerializeField, Range(8, 256)]
    private int resolution = 64;

    [Tooltip("Editor上で制御点編集時に毎フレーム更新するか")]
    [SerializeField]
    private bool updateEveryFrameInEditor = true;

    [Header("Gizmos")]
    [SerializeField]
    private bool drawGizmos = true;

    [SerializeField]
    private float gizmoPointRadius = 0.05f;

    private LineRenderer _lineRenderer;

    // 判定用サンプル（ワールド座標）
    private readonly List<Vector3> _collisionSamples = new();
    public IReadOnlyList<Vector3> CollisionSamples => _collisionSamples;



    // 再利用バッファ（GC削減）
    private readonly List<Vector3> _positions = new();

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.loop = true;
        _lineRenderer.useWorldSpace = false;

        UpdateLine(); // 初回のみ生成
    }

#if UNITY_EDITOR
    private void Update()
    {
        // Editor でのみ制御点編集に追従
        if (!Application.isPlaying && updateEveryFrameInEditor)
        {
            UpdateLine();
        }
    }
#endif

    /// <summary>
    /// スプライン形状を LineRenderer に反映
    /// </summary>
    private void UpdateLine()
    {
        if (controlPoints == null || controlPoints.Count < 3)
            return;

        _positions.Clear();

        int count = controlPoints.Count;
        int segmentResolution = Mathf.Max(1, resolution / count);

        for (int i = 0; i < count; i++)
        {
            Vector2 p0 = controlPoints[(i - 1 + count) % count];
            Vector2 p1 = controlPoints[i];
            Vector2 p2 = controlPoints[(i + 1) % count];
            Vector2 p3 = controlPoints[(i + 2) % count];

            for (int j = 0; j < segmentResolution; j++)
            {
                float t = j / (float)segmentResolution;
                Vector2 p = CatmullRom(p0, p1, p2, p3, t);
                _positions.Add(p);
            }
        }

        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());

        _collisionSamples.Clear();

        foreach (var localPos in _positions)
        {
            _collisionSamples.Add(transform.TransformPoint(localPos));
        }

        // 閉曲線を明示的に閉じる
        if (_collisionSamples.Count > 0)
            _collisionSamples.Add(_collisionSamples[0]);

    }

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

    /// <summary>
    /// 判定・移動用：ワールド座標のサンプル点列を取得
    /// </summary>
    public List<Vector3> GetSampledWorldPoints(int sampleCount, bool useLocalPlaneXY)
    {
        if (controlPoints == null || controlPoints.Count < 3 || sampleCount < 8)
            return new List<Vector3>();

        int count = controlPoints.Count;
        int segRes = Mathf.Max(1, sampleCount / count);

        var list = new List<Vector3>(count * segRes + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 p0 = controlPoints[(i - 1 + count) % count];
            Vector2 p1 = controlPoints[i];
            Vector2 p2 = controlPoints[(i + 1) % count];
            Vector2 p3 = controlPoints[(i + 2) % count];

            for (int j = 0; j < segRes; j++)
            {
                float t = j / (float)segRes;
                Vector2 p = CatmullRom(p0, p1, p2, p3, t);

                Vector3 local = useLocalPlaneXY
                    ? new Vector3(p.x, p.y, 0f)
                    : new Vector3(p.x, 0f, p.y);

                list.Add(transform.TransformPoint(local));
            }
        }

        // 閉曲線を明示的に閉じる
        if (list.Count > 0)
            list.Add(list[0]);

        return list;
    }

    public int SplineID => splineID;

    private void OnDrawGizmos()
    {
        if (!drawGizmos || controlPoints == null || controlPoints.Count < 3)
            return;

        Gizmos.matrix = transform.localToWorldMatrix;

        int count = controlPoints.Count;

        // 制御点
        Gizmos.color = Color.yellow;
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(controlPoints[i], gizmoPointRadius);
        }

        // 補助線
        Gizmos.color = Color.gray;
        for (int i = 0; i < count; i++)
        {
            Vector2 a = controlPoints[i];
            Vector2 b = controlPoints[(i + 1) % count];
            Gizmos.DrawLine(a, b);
        }

        // スプライン表示
        Gizmos.color = Color.cyan;

        Vector2 prev = controlPoints[0];
        int segmentResolution = Mathf.Max(1, resolution / count);

        for (int i = 0; i < count; i++)
        {
            Vector2 p0 = controlPoints[(i - 1 + count) % count];
            Vector2 p1 = controlPoints[i];
            Vector2 p2 = controlPoints[(i + 1) % count];
            Vector2 p3 = controlPoints[(i + 2) % count];

            for (int j = 1; j <= segmentResolution; j++)
            {
                float t = j / (float)segmentResolution;
                Vector2 cur = CatmullRom(p0, p1, p2, p3, t);
                Gizmos.DrawLine(prev, cur);
                prev = cur;
            }
        }
    }
}
