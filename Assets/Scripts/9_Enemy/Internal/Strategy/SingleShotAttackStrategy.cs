using Cysharp.Threading.Tasks;

public sealed class SingleShotAttackStrategy : IAttackStrategy
{
    private readonly EnemyController _ctx;

    public SingleShotAttackStrategy(EnemyController ctx) => _ctx = ctx;

    public UniTask OnEnterTelegraph()
    {
        // 必要ならここでSEや予告開始演出
        return UniTask.CompletedTask;
    }

    public void TickTelegraph()
    {
        var dir = _ctx.DirToPlayerNormalized();
        _ctx.ShowTelegraph(dir);
    }

    public UniTask Fire()
    {
        _ctx.HideTelegraph();
        var dir = _ctx.DirToPlayerNormalized();
        _ctx.FireBullet(dir);
        return UniTask.CompletedTask;
    }
}