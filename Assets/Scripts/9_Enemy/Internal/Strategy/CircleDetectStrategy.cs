using UnityEngine;

public sealed class CircleDetectStrategy : IDetectStrategy
{
    private readonly EnemyContext _ctx;

    public CircleDetectStrategy(EnemyContext ctx) => _ctx = ctx;

    public bool IsDetected()
    {
        if (_ctx.Player == null) return false;
        return Vector3.Distance(_ctx.Self.position, _ctx.Player.position) <= _ctx.Config.DetectRadius;
    }
}