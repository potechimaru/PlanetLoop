using UnityEngine;

public sealed class CircleDetectStrategy : IDetectStrategy
{
    private readonly EnemyController _ctx;

    public CircleDetectStrategy(EnemyController ctx) => _ctx = ctx;

    public bool IsDetected()
    {
        if (_ctx.Self == null) return false;
        return Vector3.Distance(_ctx.Self.position, _ctx.Player.position) <= _ctx.DetectRadius;
    }
}