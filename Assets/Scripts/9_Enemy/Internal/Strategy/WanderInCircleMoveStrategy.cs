using UnityEngine;
public sealed class WanderInCircleMoveStrategy : IMoveStrategy
{
    private readonly EnemyContext _ctx;

    public WanderInCircleMoveStrategy(EnemyContext ctx)
    {
        _ctx = ctx;
        PickNewTarget();
    }

    public void Tick()
    {
        var pos = _ctx.Self.position;
        var next = Vector3.MoveTowards(pos, _ctx.WanderTarget, _ctx.Config.MoveSpeed * Time.deltaTime);
        _ctx.Self.position = next;

        if ((next - _ctx.WanderTarget).sqrMagnitude < 0.01f)
            PickNewTarget();
    }

    private void PickNewTarget()
    {
        var r = _ctx.Config.DetectRadius;
        var p = (Vector2)_ctx.Origin + Random.insideUnitCircle * (r * 0.9f);
        _ctx.WanderTarget = new Vector3(p.x, p.y, _ctx.Origin.z);
    }
}