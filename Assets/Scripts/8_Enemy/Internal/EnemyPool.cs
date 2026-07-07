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

    private void BuildPools()
    {
        foreach (var entry in entries)
        {
            if (entry == null) continue;
            if (entry.prefab == null) continue;

            if (_pools.ContainsKey(entry.enemyType))
            {
                Debug.LogWarning($"[EnemyPool] èdï°ÇµÇΩEnemyTypeÇ≈Ç∑: {entry.enemyType}");
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

    public Enemy Rent(EnemyType enemyType, Vector3 position, Quaternion rotation)
    {
        if (!_pools.TryGetValue(enemyType, out var queue))
        {
            Debug.LogError($"[EnemyPool] PoolÇ™ë∂ç›ÇµÇ‹ÇπÇÒ: {enemyType}");
            return null;
        }

        Enemy enemy = queue.Count > 0
            ? queue.Dequeue()
            : CreateNew(enemyType);

        enemy.transform.SetParent(_poolRoots[enemyType]);
        enemy.transform.SetPositionAndRotation(position, rotation);

        enemy.gameObject.SetActive(true);
        //Debug.Log($"[EnemyPool] PlayerTransform: {_playerTransform != null}");
        enemy.InitializeForSpawn(_playerTransform);
        //Debug.Log($"[EnemyPool] Rent: {enemyType} at {position}");

        return enemy;
    }

    public void Return(Enemy enemy)
    {
        if (enemy == null) return;

        EnemyType enemyType = enemy.EnemyType;

        if (!_pools.TryGetValue(enemyType, out var queue))
        {
            Debug.LogError($"[EnemyPool] ReturnêÊÇÃPoolÇ™ë∂ç›ÇµÇ‹ÇπÇÒ: {enemyType}");
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
            Debug.LogError($"[EnemyPool] PrefabÇ™ë∂ç›ÇµÇ‹ÇπÇÒ: {enemyType}");
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