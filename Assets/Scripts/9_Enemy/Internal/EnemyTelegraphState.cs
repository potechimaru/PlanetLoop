using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

internal sealed class EnemyTelegraphState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyContext _ctx;
    private readonly IMoveStrategy _move;
    private readonly IAttackStrategy _attack;

    internal EnemyTelegraphState(EnemyContext ctx, IMoveStrategy move, IAttackStrategy attack)
    {
        _ctx = ctx;
        _move = move;
        _attack = attack;
    }

    public async UniTask Enter()
    {
        _ctx.TelegraphElapsed = 0f;
        await _attack.OnEnterTelegraph();
    }

    public UniTask Exit()
    {
        _ctx.View.HideTelegraph();
        return UniTask.CompletedTask;
    }

    public void Tick()
    {
        _move.Tick();

        _ctx.TelegraphElapsed += Time.deltaTime;
        _attack.TickTelegraph();

        if (_ctx.TelegraphElapsed >= _ctx.Config.TelegraphTime)
            NextState.Execute(EnemyStateKey.Cooldown);
    }
}