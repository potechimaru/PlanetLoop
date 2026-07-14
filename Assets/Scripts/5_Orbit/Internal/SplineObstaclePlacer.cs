using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ObstacleをSpline上に配置するためのコンポーネント。Splineの形状に沿って障害物を配置する機能を提供する。
/// </summary>
[RequireComponent(typeof(ClosedSplineLine))]
public class SplineObstaclePlacer : MonoBehaviour
{
    /// <summary>
    /// ByInterval: 指定した間隔でObstacleを配置するモード
    /// FullLoopEven: Spline全体を均等に分割してObstacleを等間隔に配置するモード
    /// </summary>
    public enum PlacementMode
    {
        ByInterval,
        FullLoopEven
    }

    [Header("Obstacle Prefab")]
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Common")]
    [SerializeField, Min(0)] private int obstacleCount = 1;
    [SerializeField] private float normalOffset = 0.1f;
    [SerializeField] private Transform spawnParent;

    [Header("Placement Mode")]
    [SerializeField] private PlacementMode mode = PlacementMode.FullLoopEven;

    [Header("By Interval Settings")]
    [SerializeField, Min(0f)] private float startDistance = 0f;
    [SerializeField, Min(0.01f)] private float obstacleInterval = 0.5f;

    private ClosedSplineLine _spline;
    private readonly List<GameObject> _spawned = new();

    private void Awake()
    {
        _spline = GetComponent<ClosedSplineLine>();
    }

#if UNITY_EDITOR
    /// <summary>
    /// エディタ上でSplineに沿ってObstacleを配置する。
    /// </summary>
    /// <param name="deactivateAfterBake"></param>
    public void Bake(bool deactivateAfterBake)
    {
        if (_spline == null)
            _spline = GetComponent<ClosedSplineLine>();

        _spline.EditorRebuild();
        BakeInternal(deactivateAfterBake);
    }

    private void BakeInternal(bool deactivateAfterBake)
    {
        ClearImmediate();

        if (obstaclePrefab == null)
        {
            Debug.LogWarning("[SplineObstaclePlacer] obstaclePrefab が未設定です。");
            return;
        }

        if (obstacleCount <= 0)
            return;

        float totalLen = _spline.GetTotalLength();
        if (totalLen <= 0f) return;

        if (mode == PlacementMode.ByInterval)
        {
            for (int i = 0; i < obstacleCount; i++)
            {
                float d = startDistance + obstacleInterval * i;
                CreateObstacle(d, obstaclePrefab, deactivateAfterBake);
            }
        }
        else
        {
            float step = totalLen / obstacleCount;

            for (int i = 0; i < obstacleCount; i++)
            {
                float d = step * i;
                CreateObstacle(d, obstaclePrefab, deactivateAfterBake);
            }
        }
    }
#endif

    private void CreateObstacle(float distance, GameObject prefab, bool deactivate)
    {
        float totalLen = _spline.GetTotalLength();
        distance = Mathf.Repeat(distance, totalLen);

        Vector3 pos = _spline.EvaluateByDistance(distance);
        Vector3 normal = _spline.EvaluateNormalByDistance(distance);

        pos += normal * normalOffset;

        var parent = spawnParent != null ? spawnParent : transform;

#if UNITY_EDITOR
        GameObject go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(
            prefab,
            parent
        );

        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Bake Spline Obstacle");
#else
        GameObject go = Instantiate(prefab, parent);
#endif

        go.transform.position = pos;
        go.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);

        var data = go.GetComponent<SplinePointData>();
        if (data == null)
            data = go.AddComponent<SplinePointData>();

        data.BaseDistance = distance;

        if (deactivate)
            go.SetActive(false);

        _spawned.Add(go);
    }

#if UNITY_EDITOR
    /// <summary>
    /// ObstacleをSplineから削除する。エディタ上での操作用。
    /// </summary>
    public void ClearImmediate()
    {
        var parent = spawnParent != null ? spawnParent : transform;

        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i).gameObject;
            UnityEditor.Undo.DestroyObjectImmediate(child);
        }

        _spawned.Clear();
    }
#endif

    public void ClearRuntime()
    {
        foreach (var go in _spawned)
        {
            if (go != null)
                Destroy(go);
        }

        _spawned.Clear();
    }
}