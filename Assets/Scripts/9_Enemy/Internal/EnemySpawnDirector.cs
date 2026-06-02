using Cysharp.Threading.Tasks;
using System;
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
        SpawnInitialEnemies();
        SpawnLoopAsync(_destroyToken).Forget();
    }

    private void SpawnInitialEnemies()
    {
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnOne();
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
                    SpawnOne();
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

    private void SpawnOne()
    {
        if (spawnPointGroup == null)
        {
            Debug.LogWarning("[EnemySpawnDirector] spawnPointGroup ‚ª–¢Ý’è‚Å‚·B");
            return;
        }

        if (!spawnPointGroup.TryGetRandomFreeSpawnPoint(out var spawnPoint))
            return;

        var enemy = _enemyManager.SpawnRandomEnemy(spawnPoint.Position);

        if (enemy == null)
        {
            spawnPoint.Release();
            return;
        }

        enemy.SetSpawnPoint(spawnPoint);
    }
}