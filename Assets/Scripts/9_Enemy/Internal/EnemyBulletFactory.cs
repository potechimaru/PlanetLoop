using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class EnemyBulletFactory
{
    [Header("Pools")]
    private SingleBulletPool _singleBulletPool;

    public EnemyBulletFactory(SingleBulletPool singleBulletPool)
    {
        _singleBulletPool = singleBulletPool;

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

            default:
                return;
        }
    }
}