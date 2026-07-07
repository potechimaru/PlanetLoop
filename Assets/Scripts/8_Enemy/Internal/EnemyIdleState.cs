using Cysharp.Threading.Tasks;
using UniRx;

internal sealed class EnemyIdleState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyController _ctx;
    private readonly IDetectStrategy _detect;
    private readonly IMoveStrategy _move;

    internal EnemyIdleState(EnemyController ctx, IDetectStrategy detect, IMoveStrategy move)
    {
        _ctx = ctx;
        _detect = detect;
        _move = move;
    }

    public UniTask Enter()
    {
        _ctx.HideTelegraph();
        _ctx.RotateDecoration();
        return UniTask.CompletedTask;
    }

    public UniTask Exit() => UniTask.CompletedTask;

    public void Tick()
    {
        _move.Tick();

        if (!_ctx.IsDetectionEnabled)
            return;

        if (_detect.IsDetected())
            NextState.Execute(EnemyStateKey.Telegraph);
    }
}