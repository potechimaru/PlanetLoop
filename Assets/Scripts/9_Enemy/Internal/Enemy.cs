using Cysharp.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyViewCommon view;
    [SerializeField] private Transform player;

    [Header("Common Config")]
    [SerializeField] private EnemyCommonConfig config = new();

    [Header("Strategy Select")]
    [SerializeField] private EnemyMoveType moveType = EnemyMoveType.Fixed;
    [SerializeField] private EnemyAttackType attackType = EnemyAttackType.Single; // ‰º‚Å’è‹`

    private EnemyContext _ctx;
    private EnemyStateMachine _sm;

    // strategies
    private IDetectStrategy _detect;
    private IMoveStrategy _move;
    private IAttackStrategy _attack;

    private async void Awake()
    {
        if (view == null) view = GetComponentInChildren<EnemyViewCommon>();

        _ctx = new EnemyContext(transform, player, view, config);

        _detect = new CircleDetectStrategy(_ctx);
        _move = CreateMove(moveType, _ctx);
        _attack = CreateAttack(attackType, _ctx);

        _sm = new EnemyStateMachine();
        _sm.RegisterState(EnemyStateKey.Idle, new EnemyIdleState(_ctx, _detect, _move));
        _sm.RegisterState(EnemyStateKey.Telegraph, new EnemyTelegraphState(_ctx, _move, _attack));
        _sm.RegisterState(EnemyStateKey.Cooldown, new EnemyCooldownState(_ctx, _move, _attack));

        await _sm.ChangeStateAsync(EnemyStateKey.Idle);
    }

    private void Update()
    {
        _sm?.Tick();
    }

    private void OnDestroy()
    {
        _sm?.Dispose();
    }

    private static IMoveStrategy CreateMove(EnemyMoveType t, EnemyContext ctx)
    {
        return t switch
        {
            EnemyMoveType.WanderInDetectCircle => new WanderInCircleMoveStrategy(ctx),
            _ => new FixedMoveStrategy()
        };
    }

    private static IAttackStrategy CreateAttack(EnemyAttackType t, EnemyContext ctx)
    {
        return t switch
        {
            EnemyAttackType.Single => new SingleShotAttackStrategy(ctx),
            _ => new SingleShotAttackStrategy(ctx)
        };
    }
}