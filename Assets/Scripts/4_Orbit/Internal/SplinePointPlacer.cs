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

    [Header("Common")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private float normalOffset = 0.1f;

    [SerializeField] private Transform spawnParent;

    [Header("Placement Mode")]
    [SerializeField] private PlacementMode mode = PlacementMode.ByInterval;

    [Header("By Interval Settings")]
    [SerializeField, Min(0f)]
    private float startDistance = 0f;

    [SerializeField, Min(0.01f)]
    private float pointInterval = 0.5f;

    [SerializeField, Min(1)]
    private int intervalPointCount = 12;

    [Header("Full Loop Settings")]
    [SerializeField, Min(1)]
    private int loopPointCount = 12;

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
#endif

#if UNITY_EDITOR
    private void BakeInternal(bool deactivateAfterBake)
    {
        ClearImmediate();

        float totalLen = _spline.GetTotalLength();
        if (totalLen <= 0f)
            return;

        if (mode == PlacementMode.ByInterval)
        {
            for (int i = 0; i < intervalPointCount; i++)
            {
                float d = startDistance + pointInterval * i;
                CreatePoint(d, deactivateAfterBake);
            }
        }
        else // FullLoopEven
        {
            float step = totalLen / loopPointCount;

            for (int i = 0; i < loopPointCount; i++)
            {
                float d = step * i;
                CreatePoint(d, deactivateAfterBake);
            }
        }
    }
#endif

    private void CreatePoint(float distance, bool deactivate)
    {
        float totalLen = _spline.GetTotalLength();
        distance = Mathf.Repeat(distance, totalLen);

        Vector3 pos = _spline.EvaluateByDistance(distance);
        Vector3 normal = _spline.EvaluateNormalByDistance(distance);

        pos += normal * normalOffset;

        var parent = (spawnParent != null) ? spawnParent : transform;

#if UNITY_EDITOR
        GameObject go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(
            pointPrefab,
            parent);

        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Bake Spline Point");
#else
        GameObject go = Instantiate(pointPrefab, parent);
#endif

        // 位置・回転はワールドで合わせる（親がどこでもOK）
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
        var parent = (spawnParent != null) ? spawnParent : transform;

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