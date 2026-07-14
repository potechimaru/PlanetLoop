using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

[Serializable]
public class EnemyPrefabEntry
{
    public EnemyType enemyType;
    public Enemy prefab;
    public int initialPoolSize = 5;
}

/// <summary>
/// Enemyのプールを管理するクラス。Enemyの生成と再利用を効率的に行うためのオブジェクトプールを提供する。
/// </summary>
public class EnemyPool : MonoBehaviour
{
    [SerializeField] private List<EnemyPrefabEntry> entries = new();

    private readonly Dictionary<EnemyType, Queue<Enemy>> _pools = new();
    private readonly Dictionary<EnemyType, Enemy> _prefabs = new();
    private readonly Dictionary<EnemyType, Transform> _poolRoots = new();

    private Transform _playerTransform;

    private IObjectResolver _resolver;

    [Inject]
    public void Construct(IObjectResolver resolver, Transform playerTransform)
    {
        _resolver = resolver;
        _playerTransform = playerTransform;
    }

    private void Awake()
    {
        BuildPools();
    }

    /// <summary>
    /// Enemyの種類の数分だけプールを作成する。各EnemyTypeに対して、
    /// 初期プールサイズ分のEnemyインスタンスを生成し、非アクティブ状態でキューに格納する。
    /// </summary>
    private void BuildPools()
    {
        foreach (var entry in entries)
        {
            if (entry == null) continue;
            if (entry.prefab == null) continue;

            if (_pools.ContainsKey(entry.enemyType))
            {
                Debug.LogWarning($"[EnemyPool] 重複したEnemyTypeです: {entry.enemyType}");
                continue;
            }

            var root = new GameObject($"{entry.enemyType}Pool").transform;
            root.SetParent(transform);

            _prefabs.Add(entry.enemyType, entry.prefab);
            _poolRoots.Add(entry.enemyType, root);
            _pools.Add(entry.enemyType, new Queue<Enemy>());

            for (int i = 0; i < entry.initialPoolSize; i++)
            {
                Enemy enemy = CreateNew(entry.enemyType);
                enemy.gameObject.SetActive(false);
                _pools[entry.enemyType].Enqueue(enemy);
            }
        }
    }

    /// <summary>
    /// Enemyをプールから取得する。プールに利用可能なEnemyが存在する場合はそれを返し、存在しない場合は新たに生成する。
    /// </summary>
    /// <param name="enemyType">Enemyの種類</param>
    /// <param name="position">Enemyを出現させる位置</param>
    /// <param name="rotation">Enemyの回転量</param>
    /// <returns></returns>
    public Enemy Rent(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        if (!_pools.TryGetValue(enemyType, out var queue))
        {
            Debug.LogError($"[EnemyPool] Poolが存在しません: {enemyType}");
            return null;
        }

        Enemy enemy = queue.Count > 0
            ? queue.Dequeue()
            : CreateNew(enemyType);

        enemy.transform.SetParent(_poolRoots[enemyType]);
        enemy.transform.SetPositionAndRotation(position, rotation);

        enemy.gameObject.SetActive(true);
        enemy.InitializeForSpawn(_playerTransform);

        return enemy;
    }

    /// <summary>
    /// Enemyをプールに返却する。返却されたEnemyは非アクティブ状態にされ、プールのキューに戻される。
    /// </summary>
    /// <param name="enemy">返すEnemy</param>
    public void Return(Enemy enemy)
    {
        if (enemy == null) return;

        EnemyType enemyType = enemy.EnemyType;

        if (!_pools.TryGetValue(enemyType, out var queue))
        {
            Debug.LogError($"[EnemyPool] Return先のPoolが存在しません: {enemyType}");
            enemy.gameObject.SetActive(false);
            return;
        }

        enemy.gameObject.SetActive(false);
        enemy.transform.SetParent(_poolRoots[enemyType]);

        queue.Enqueue(enemy);
    }

    public IReadOnlyList<EnemyType> GetAvailableEnemyTypes()
    {
        return new List<EnemyType>(_pools.Keys);
    }

    private Enemy CreateNew(EnemyType enemyType)
    {
        if (!_prefabs.TryGetValue(enemyType, out var prefab))
        {
            Debug.LogError($"[EnemyPool] Prefabが存在しません: {enemyType}");
            return null;
        }

        Transform parent = _poolRoots[enemyType];

        Enemy enemy = Instantiate(prefab, parent);

        if (_resolver != null)
        {
            _resolver.Inject(enemy);
        }

        enemy.gameObject.SetActive(false);

        return enemy;
    }
}