using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

/// <summary>
/// Enemy5のレーザービームをプールするクラス。レーザービームの生成と再利用を効率的に管理する。
/// </summary>
public sealed class LaserBeamPool : MonoBehaviour, ILaserBeamReturner
{
    [Header("Prefab / Parent")]
    [SerializeField] private LaserBeam prefab;
    [SerializeField] private Transform defaultParent;

    [Header("Pooling")]
    [SerializeField, Min(0)] private int prewarmCount = 3;
    [SerializeField] private bool setInactiveOnReturn = true;

    [Inject] private LaserBeamManager _laserBeamManager;

    private readonly Stack<PooledLaserBeamObject> _pool = new();
    private readonly HashSet<PooledLaserBeamObject> _rented = new();

    private void Awake()
    {
        Prewarm();
    }

    private void Prewarm()
    {
        for (int i = 0; i < prewarmCount; i++)
        {
            var item = CreateNew(defaultParent);
            Return(item);
        }
    }

    private PooledLaserBeamObject CreateNew(Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError($"{nameof(LaserBeamPool)}: prefab が未設定です。", this);
            return null;
        }

        var laser = Instantiate(prefab, parent);

        var pooled = laser.GetComponent<PooledLaserBeamObject>();
        if (pooled == null)
            pooled = laser.gameObject.AddComponent<PooledLaserBeamObject>();

        pooled.Bind(this);

        if (setInactiveOnReturn)
            laser.gameObject.SetActive(false);

        return pooled;
    }

    public async UniTask<LaserBeam> RentAsync(
        Vector3 position,
        Quaternion rotation)
    {
        PooledLaserBeamObject item =
            _pool.Count > 0 ? _pool.Pop() : CreateNew(defaultParent);

        if (item == null) return null;

        _rented.Add(item);

        Transform tr = item.transform;

        if (defaultParent != null && tr.parent != defaultParent)
            tr.SetParent(defaultParent, true);

        tr.position = position;
        tr.rotation = rotation;

        item.gameObject.SetActive(true);

        var laser = item.GetComponent<LaserBeam>();

        _laserBeamManager.RegisterLaser(laser);

        await UniTask.CompletedTask;

        return laser;
    }

    public void Return(PooledLaserBeamObject item)
    {
        if (item == null) return;

        _rented.Remove(item);

        var laser = item.GetComponent<LaserBeam>();
        if (laser != null)
            laser.Hide();

        if (setInactiveOnReturn)
            item.gameObject.SetActive(false);

        if (defaultParent != null)
            item.transform.SetParent(defaultParent, true);

        _pool.Push(item);
    }
}