using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

internal sealed class EnemyTelegraphState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyController _ctx;
    private readonly IMoveStrategy _move;
    private readonly IAttackStrategy _attack;
    private readonly IEnemyExternalFacade _enemyExternalFacade;

    internal EnemyTelegraphState(EnemyController ctx, IMoveStrategy move, IAttackStrategy attack, IEnemyExternalFacade enemyExternalFacade)
    {
        _ctx = ctx;
        _move = move;
        _attack = attack;
        _enemyExternalFacade = enemyExternalFacade;
    }

    public async UniTask Enter()
    {
        _ctx.TelegraphElapsed = 0f;
        _ctx.StopRotateDecoration();
        //_enemyExternalFacade.PlaySE(SEType.EnemyLockOn);
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