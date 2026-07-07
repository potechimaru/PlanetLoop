using Cysharp.Threading.Tasks;

/// <summary>
/// Enemy1、3で汎用的に使用されるAttackStrategy。プレイヤーの方向に一発だけ弾を撃つ。
/// </summary>
public sealed class SingleShotAttackStrategy : IAttackStrategy
{
    private readonly EnemyController _ctx;

    public SingleShotAttackStrategy(EnemyController ctx) => _ctx = ctx;

    public UniTask OnEnterTelegraph()
    {
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