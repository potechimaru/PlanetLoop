using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

internal sealed class EnemyCooldownState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyContext _ctx;
    private readonly IMoveStrategy _move;
    private readonly IAttackStrategy _attack;

    internal EnemyCooldownState(EnemyContext ctx, IMoveStrategy move, IAttackStrategy attack)
    {
        _ctx = ctx;
        _move = move;
        _attack = attack;
    }

    public async UniTask Enter()
    {
        // š‚±‚±‚Å•K‚¸”­ŽË‚ðawait‚µ‚Ä’¼—ñ‰»
        await _attack.Fire();

        _ctx.CooldownElapsed = 0f;
    }

    public UniTask Exit() => UniTask.CompletedTask;

    public void Tick()
    {
        _move.Tick();

        _ctx.CooldownElapsed += Time.deltaTime;
        if (_ctx.CooldownElapsed >= _ctx.Config.CooldownTime)
            NextState.Execute(EnemyStateKey.Idle);
    }
}