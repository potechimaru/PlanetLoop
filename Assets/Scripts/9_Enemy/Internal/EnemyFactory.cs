using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyFactory
{
    private readonly EnemyPool _enemyPool;

    public EnemyFactory(EnemyPool enemyPool)
    {
        _enemyPool = enemyPool;
    }

    public Enemy CreateRandom(Vector3 position)
    {
        if (_enemyPool == null)
        {
            Debug.LogError("[EnemyFactory] EnemyPool が null です。");
            return null;
        }

        IReadOnlyList<EnemyType> availableTypes =
            _enemyPool.GetAvailableEnemyTypes();

        if (availableTypes == null || availableTypes.Count == 0)
        {
            Debug.LogError("[EnemyFactory] 使用可能なEnemyTypeがありません。EnemyPoolのentriesを確認してください。");
            return null;
        }

        int index = Random.Range(0, availableTypes.Count);
        EnemyType enemyType = availableTypes[index];

        return Create(enemyType, position);
    }

    public Enemy Create(EnemyType enemyType, Vector3 position)
    {
        if (_enemyPool == null)
        {
            Debug.LogError("[EnemyFactory] EnemyPool が null です。");
            return null;
        }

        return _enemyPool.Rent(
            enemyType,
            position,
            Quaternion.identity
        );
    }

    public void Return(Enemy enemy)
    {
        if (enemy == null) return;
        _enemyPool.Return(enemy);
    }
}