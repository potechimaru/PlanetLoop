using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

internal sealed class EnemyTelegraphState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyController _ctx;
    private readonly IMoveStrategy _move;
    private readonly IAttackStrategy _attack;

    internal EnemyTelegraphState(EnemyController ctx, IMoveStrategy move, IAttackStrategy attack)
    {
        _ctx = ctx;
        _move = move;
        _attack = attack;
    }

    public async UniTask Enter()
    {
        _ctx.TelegraphElapsed = 0f;
        _ctx.StopRotateDecoration();
        await _attack.OnEnterTelegraph();
    }

    public UniTask Exit()
    {
        _ctx.HideTelegraph();
        return UniTask.CompletedTask;
    }

    public void Tick()
    {
        _move.Tick();

        _ctx.TelegraphElapsed += Time.deltaTime;
        _attack.TickTelegraph();

        if (_ctx.TelegraphElapsed >= _ctx.TelegraphTime)
            NextState.Execute(EnemyStateKey.Cooldown);
    }
}