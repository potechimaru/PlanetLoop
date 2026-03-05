using Cysharp.Threading.Tasks;

public sealed class SingleShotAttackStrategy : IAttackStrategy
{
    private readonly EnemyContext _ctx;

    public SingleShotAttackStrategy(EnemyContext ctx) => _ctx = ctx;

    public UniTask OnEnterTelegraph()
    {
        // 必要ならここでSEや予告開始演出
        return UniTask.CompletedTask;
    }

    public void TickTelegraph()
    {
        var dir = _ctx.DirToPlayerNormalized();
        _ctx.View.ShowTelegraph(dir);
    }

    public UniTask Fire()
    {
        _ctx.View.HideTelegraph();
        var dir = _ctx.DirToPlayerNormalized();
        _ctx.View.FireBullet(dir);
        return UniTask.CompletedTask;
    }
}