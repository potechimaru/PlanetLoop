using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ClosedSplineLine))]
public class SplineObstaclePlacer : MonoBehaviour
{
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
            Debug.LogWarning("[SplineObstaclePlacer] obstaclePrefab Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
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