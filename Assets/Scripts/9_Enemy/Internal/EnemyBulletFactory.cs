using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class EnemyBulletFactory
{
    [Header("Pools")]
    private SingleBulletPool _singleBulletPool;
    private EnemyBulletManager _enemyBulletManager;

    public EnemyBulletFactory(SingleBulletPool singleBulletPool, EnemyBulletManager enemyBulletManager)
    {
        _singleBulletPool = singleBulletPool;
        _enemyBulletManager = enemyBulletManager;


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
                _singleBulletPool.Rent(anchoredPos, dirNormalized).Forget();
                break;

            case EnemyAttackType.Spread:
                if (_enemyBulletManager == null) return;
                _singleBulletPool.Rent(anchoredPos, dirNormalized).Forget();
                break;

            default:
                return;
        }
    }
}