using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// EnemyBulletを生成するためのファクトリクラス。EnemyAttackTypeに応じて適切なBulletを生成する。
/// </summary>
public sealed class EnemyBulletFactory
{
    [Header("Pools")]
    private SingleBulletPool _singleBulletPool;
    private LargeSingleBulletPool _largeSingleBulletPool;
    private EnemyBulletManager _enemyBulletManager;
    private EnemySimulationRangeController _enemySimulationRangeController;

    public EnemyBulletFactory(SingleBulletPool singleBulletPool,LargeSingleBulletPool largeSingleBulletPool, EnemyBulletManager enemyBulletManager, EnemySimulationRangeController enemySimulationRangeController)
    {
        _singleBulletPool = singleBulletPool;
        _enemyBulletManager = enemyBulletManager;
        _enemySimulationRangeController = enemySimulationRangeController;
        _largeSingleBulletPool = largeSingleBulletPool;

    }

    /// <summary>
    /// 指定UIを出現
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

            case EnemyAttackType.LargeSingle:
                if (_singleBulletPool == null) return;
                _largeSingleBulletPool.Rent(anchoredPos, dirNormalized, _enemySimulationRangeController.GetActiveRadius()).Forget();
                break;

            case EnemyAttackType.LargeSpread:
                if (_enemyBulletManager == null) return;
                _largeSingleBulletPool.Rent(anchoredPos, dirNormalized, _enemySimulationRangeController.GetActiveRadius()).Forget();
                break;


            default:
                return;
        }
    }
}