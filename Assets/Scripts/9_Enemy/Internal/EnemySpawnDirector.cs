using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VContainer;

public class EnemySpawnDirector : MonoBehaviour
{
    [SerializeField] private EnemySpawnPointGroup spawnPointGroup;

    [Header("Initial Spawn")]
    [SerializeField, Min(0)] private int initialSpawnCount = 5;

    [Header("Interval Spawn")]
    [SerializeField, Min(0.1f)] private float spawnInterval = 5f;
    [SerializeField, Min(1)] private int spawnCountPerInterval = 1;

    [Header("Spawn Type Unlock")]
    [SerializeField, Min(1f)] private float unlockIntervalSeconds = 60f;

    [SerializeField]
    private List<EnemyType> unlockOrder = new()
    {
        EnemyType.Enemy2,
        EnemyType.Enemy3,
        EnemyType.Enemy4,
        EnemyType.Enemy5
    };

    private readonly List<EnemyType> _currentSpawnTypes = new()
    {
        EnemyType.Enemy1,
    };

    private EnemyManager _enemyManager;
    private CancellationToken _destroyToken;

    [Inject]
    public void Construct(EnemyManager enemyManager)
    {
        _enemyManager = enemyManager;
    }

    private void Awake()
    {
        _destroyToken = this.GetCancellationTokenOnDestroy();
    }

    private void Start()
    {
        SetCurrentSpawnTypes(0);

        SpawnInitialEnemies();

        SpawnLoopAsync(_destroyToken).Forget();
        UnlockEnemyTypesLoopAsync(_destroyToken).Forget();
    }

    private void SetCurrentSpawnTypes(int startIndex)
    {
        _currentSpawnTypes.Clear();

        var allTypes = GetSpawnProgressionTypes();

        if (allTypes.Count == 0)
            return;

        int firstIndex = Mathf.Clamp(startIndex, 0, allTypes.Count - 1);
        int secondIndex = Mathf.Clamp(startIndex + 1, 0, allTypes.Count - 1);

        _currentSpawnTypes.Add(allTypes[firstIndex]);

        if (secondIndex != firstIndex)
            _currentSpawnTypes.Add(allTypes[secondIndex]);
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnOneFromTypes(_currentSpawnTypes);
        }
    }

    //private List<EnemyType> GetInitialSpawnTypes()
    //{
    //    var initialTypes = new List<EnemyType>();

    //    foreach (EnemyType type in Enum.GetValues(typeof(EnemyType)))
    //    {
    //        if (!unlockOrder.Contains(type))
    //        {
    //            initialTypes.Add(type);
    //        }
    //    }

    //    return initialTypes;
    //}

    private async UniTaskVoid UnlockEnemyTypesLoopAsync(CancellationToken token)
    {
        try
        {
            for (int i = 0; i < unlockOrder.Count; i++)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(unlockIntervalSeconds),
                    cancellationToken: token
                );

                SetCurrentSpawnTypes(i + 1);
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async UniTaskVoid SpawnLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Delay(
                    TimeSpan.FromSeconds(spawnInterval),
                    cancellationToken: token
                );

                for (int i = 0; i < spawnCountPerInterval; i++)
                {
                    SpawnOneFromTypes(_currentSpawnTypes);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void SpawnOneFromTypes(IReadOnlyList<EnemyType> enemyTypes)
    {
        if (spawnPointGroup == null)
        {
            Debug.LogWarning("[EnemySpawnDirector] spawnPointGroup Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        if (enemyTypes == null || enemyTypes.Count == 0)
        {
            Debug.LogWarning("[EnemySpawnDirector] enemyTypes Ç™ãÛÇ≈Ç∑ÅB");
            return;
        }

        if (!spawnPointGroup.TryGetRandomFreeSpawnPoint(out var spawnPoint))
            return;

        var enemy = _enemyManager.SpawnRandomEnemyFromTypes(
            spawnPoint.Position,
            enemyTypes
        );

        if (enemy == null)
        {
            spawnPoint.Release();
            return;
        }

        enemy.SetSpawnPoint(spawnPoint);
    }

    private List<EnemyType> GetSpawnProgressionTypes()
    {
        var types = new List<EnemyType>
    {
        EnemyType.Enemy1
    };

        foreach (var type in unlockOrder)
        {
            if (!types.Contains(type))
                types.Add(type);
        }

        return types;
    }
}