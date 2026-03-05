using Cysharp.Threading.Tasks;
using UniRx;

internal sealed class EnemyIdleState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyContext _ctx;
    private readonly IDetectStrategy _detect;
    private readonly IMoveStrategy _move;

    internal EnemyIdleState(EnemyContext ctx, IDetectStrategy detect, IMoveStrategy move)
    {
        _ctx = ctx;
        _detect = detect;
        _move = move;
    }

    public UniTask Enter()
    {
        _ctx.View.HideTelegraph();
        return UniTask.CompletedTask;
    }

    public UniTask Exit() => UniTask.CompletedTask;

    public void Tick()
    {
        _move.Tick();

        if (_detect.IsDetected())
            NextState.Execute(EnemyStateKey.Telegraph);
    }
}