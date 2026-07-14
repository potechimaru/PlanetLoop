/// <summary>
/// Laserをプールに返却するためのインターフェース。
/// </summary>
public interface ILaserBeamReturner
{
    void Return(PooledLaserBeamObject item);
}