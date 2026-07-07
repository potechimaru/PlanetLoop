using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyLaserFactory
{
    private readonly LaserBeamPool _laserBeamPool;

    public EnemyLaserFactory(LaserBeamPool laserBeamPool)
    {
        _laserBeamPool = laserBeamPool;
    }

    public async UniTask<LaserBeam> SpawnAsync(
        Vector3 startPos,
        Vector3 direction)
    {
        var laser = await _laserBeamPool.RentAsync(startPos, Quaternion.identity);

        laser.PlayAsync(
            startPos,
            direction,
            laser.GetCancellationTokenOnDestroy()
        ).Forget();

        return laser;
    }
}