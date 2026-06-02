using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ClosedSplineLine))]
public class SplinePointPlacer : MonoBehaviour
{
    public enum PlacementMode
    {
        ByInterval,
        FullLoopEven
    }

    [Header("Point Prefabs")]
    [SerializeField] private GameObject pointLowPrefab;
    [SerializeField] private GameObject pointMediumPrefab;
    [SerializeField] private GameObject pointHighPrefab;
    [SerializeField] private GameObject pointVeryHighPrefab;

    [Header("Common")]
    [SerializeField] private List<PointPlacementEntry> pointEntries = new();
    [SerializeField] private float normalOffset = 0.1f;
    [SerializeField] private Transform spawnParent;

    [Header("Placement Mode")]
    [SerializeField] private PlacementMode mode = PlacementMode.FullLoopEven;

    [Header("By Interval Settings")]
    [SerializeField, Min(0f)] private float startDistance = 0f;
    [SerializeField, Min(0.01f)] private float pointInterval = 0.5f;

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

        float totalLen = _spline.GetTotalLength();
        if (totalLen <= 0f) return;

        List<GameObject> placementList = BuildPlacementList();

        if (placementList.Count == 0)
        {
            Debug.LogWarning("[SplinePointPlacer] îzíuÇ∑ÇÈPointPrefabÇ™Ç†ÇËÇ‹ÇπÇÒÅB");
            return;
        }

        if (mode == PlacementMode.ByInterval)
        {
            for (int i = 0; i < placementList.Count; i++)
            {
                float d = startDistance + pointInterval * i;
                CreatePoint(d, placementList[i], deactivateAfterBake);
            }
        }
        else
        {
            float step = totalLen / placementList.Count;

            for (int i = 0; i < placementList.Count; i++)
            {
                float d = step * i;
                CreatePoint(d, placementList[i], deactivateAfterBake);
            }
        }
    }
#endif

    private GameObject GetPrefab(PointObjectType type)
    {
        return type switch
        {
            PointObjectType.Low => pointLowPrefab,
            PointObjectType.Medium => pointMediumPrefab,
            PointObjectType.High => pointHighPrefab,
            PointObjectType.VeryHigh => pointVeryHighPrefab,
            _ => null
        };
    }

    private List<GameObject> BuildPlacementList()
    {
        var list = new List<GameObject>();

        foreach (var entry in pointEntries)
        {
            if (entry == null) continue;

            GameObject prefab = GetPrefab(entry.pointType);

            if (prefab == null)
            {
                Debug.LogWarning($"[SplinePointPlacer] Prefabñ¢ê›íË: {entry.pointType}");
                continue;
            }

            for (int i = 0; i < entry.count; i++)
            {
                list.Add(prefab);
            }
        }

        return list;
    }

    private void CreatePoint(float distance, GameObject prefab, bool deactivate)
    {
        if (prefab == null) return;

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

        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Bake Spline Point");
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
            Destroy(go);

        _spawned.Clear();
    }
}