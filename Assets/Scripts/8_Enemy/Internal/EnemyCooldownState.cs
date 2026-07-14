using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// EnemyStateのCooldown状態を表すクラス。敵が攻撃後のクールダウン中の状態を管理する。
/// </summary>
internal sealed class EnemyCooldownState : IEnemyState
{
    public ReactiveCommand<EnemyStateKey> NextState { get; } = new();

    private readonly EnemyController _ctx;
    private readonly IMoveStrategy _move;
    private readonly IAttackStrategy _attack;

    internal EnemyCooldownState(EnemyController ctx, IMoveStrategy move, IAttackStrategy attack)
    {
        _ctx = ctx;
        _move = move;
        _attack = attack;
    }

    public async UniTask Enter()
    {
        await _attack.Fire();

        _ctx.CooldownElapsed = 0f;

        _ctx.RotateDecoration();
    }

    public UniTask Exit() => UniTask.CompletedTask;

    public void Tick()
    {
        _move.Tick();

        _ctx.CooldownElapsed += Time.deltaTime;
        if (_ctx.CooldownElapsed >= _ctx.CooldownTime)
            NextState.Execute(EnemyStateKey.Idle);
    }
}