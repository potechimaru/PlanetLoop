using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class EnemyBulletFactory
{
    [Header("Pools")]
    private SingleBulletPool _singleBulletPool;
    private EnemyBulletManager _enemyBulletManager;
    private EnemySimulationRangeController _enemySimulationRangeController;

    public EnemyBulletFactory(SingleBulletPool singleBulletPool, EnemyBulletManager enemyBulletManager, EnemySimulationRangeController enemySimulationRangeController)
    {
        _singleBulletPool = singleBulletPool;
        _enemyBulletManager = enemyBulletManager;
        _enemySimulationRangeController = enemySimulationRangeController;

    }

    /// <summary>
    /// éwíËUIÇèoåª
    /// </summary>

    public void Spawn(EnemyAttackType type, Vector2 anchoredPos, Vector3 dirNormalized)
    {
        switch (type)
        {
            case EnemyAttackType.Single:
                if (_singleBulletPool == null) return;
                _singleBulletPool.Rent(anchoredPos, dirNormalized, _enemySimulationRangeController.GetActiveRadius()).Forget();
                break;

            case EnemyAttackType.Spread:
                if (_enemyBulletManager == null) return;
                _singleBulletPool.Rent(anchoredPos, dirNormalized, _enemySimulationRangeController.GetActiveRadius()).Forget();
                break;

            default:
                return;
        }
    }
}