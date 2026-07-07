using UnityEngine;

/// <summary>
/// 使ってないが、敵のMoveStrategyの一つとして円形に徘徊する戦略を実装したクラス
/// Splineの内側に入ったらダメじゃんと断念（入らないようにすればいいけど要らんなーと）
/// </summary>
public sealed class WanderInCircleMoveStrategy : IMoveStrategy
{
    private readonly EnemyController _ctx;

    public WanderInCircleMoveStrategy(EnemyController ctx)
    {
        _ctx = ctx;
        PickNewTarget();
    }

    public void Tick()
    {
        var pos = _ctx.Self.position;
        var next = Vector3.MoveTowards(pos, _ctx.WanderTarget, _ctx.MoveSpeed * Time.deltaTime);
        _ctx.Self.position = next;

        if ((next - _ctx.WanderTarget).sqrMagnitude < 0.01f)
            PickNewTarget();
    }

    private void PickNewTarget()
    {
        var r = _ctx.DetectRadius;
        var p = (Vector2)_ctx.Origin + Random.insideUnitCircle * (r * 0.9f);
        _ctx.WanderTarget = new Vector3(p.x, p.y, _ctx.Origin.z);
    }
}