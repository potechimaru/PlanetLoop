/// <summary>
/// EnemyBulletをプールに返却するためのインターフェース。
/// </summary>
public interface IBulletReturner
{
    void Return(PooledBulletObject bullet);
}