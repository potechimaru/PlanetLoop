using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyView view;
    [SerializeField] private Transform player;

    [Header("Common Config")]
    [SerializeField] private EnemyConfig config = new();

    //[Header("Strategy Select")]
    //[SerializeField] private EnemyDetectType detectType = EnemyDetectType.Circle; // ‰º‚Å’è‹`
    //[SerializeField] private EnemyMoveType moveType = EnemyMoveType.Fixed;
    //[SerializeField] private EnemyAttackType attackType = EnemyAttackType.Single;

    [SerializeField] private EnemyType _enemyType = EnemyType.Enemy1;

    private EnemyController _ctx;
    private EnemyStateMachine _sm;

    // strategies
    private IDetectStrategy _detect;
    private IMoveStrategy _move;
    private IAttackStrategy _attack;

    private async void Awake()
    {
        if (view == null) view = GetComponentInChildren<EnemyView>();

        _ctx = new EnemyController(transform, player, view, config);

        //_detect = CreateDetect(detectType, _ctx);
        //_move = CreateMove(moveType, _ctx);
        //_attack = CreateAttack(attackType, _ctx);

        _detect = CreateDetectStrategy(_enemyType, _ctx);
        _move = CreateMoveStrategy(_enemyType, _ctx);
        _attack = CreateAttackStrategy(_enemyType, _ctx);


        _sm = new EnemyStateMachine();
        RegisterState();

        await _sm.ChangeStateAsync(EnemyStateKey.Idle);
    }

    protected virtual void RegisterState()
    {
        _sm.RegisterState(EnemyStateKey.Idle, new EnemyIdleState(_ctx, _detect, _move));
        _sm.RegisterState(EnemyStateKey.Telegraph, new EnemyTelegraphState(_ctx, _move, _attack));
        _sm.RegisterState(EnemyStateKey.Cooldown, new EnemyCooldownState(_ctx, _move, _attack));
    }

    private void Update()
    {
        _sm?.Tick();
    }

    private void OnDestroy()
    {
        _sm?.Dispose();
    }

    private static IDetectStrategy CreateDetectStrategy(EnemyType enemyType, EnemyController ctx)
    {
        return enemyType switch
        {
            EnemyType.Enemy1 => new CircleDetectStrategy(ctx),
            _ => new CircleDetectStrategy(ctx)
        };

    }

    private static IMoveStrategy CreateMoveStrategy(EnemyType enemyType, EnemyController ctx)
    {
        return enemyType switch
        {
            EnemyType.Enemy1 => new FixedMoveStrategy(),
            _ => new WanderInCircleMoveStrategy(ctx)
        };
    }

    private static IAttackStrategy CreateAttackStrategy(EnemyType enemyType, EnemyController ctx)
    {
        return enemyType switch
        {
            EnemyType.Enemy1 => new SingleShotAttackStrategy(ctx),
            _ => new SingleShotAttackStrategy(ctx)
        };
    }
}