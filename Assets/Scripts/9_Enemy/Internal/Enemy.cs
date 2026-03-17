using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer;

public class Enemy : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyView view;
    [SerializeField] private Transform player;

    [Header("Common Config")]
    [SerializeField] private EnemyConfig config = new();

    [SerializeField] private EnemyType _enemyType = EnemyType.Enemy1;

    [Inject] private EnemyBulletFactory _bulletFactory;

    protected EnemyController _enemyController;
    protected EnemyStateMachine _sm;

    protected IDetectStrategy _detect;
    protected IMoveStrategy _move;
    protected IAttackStrategy _attack;

    private readonly CompositeDisposable _disposables = new();
    private bool _isDead;

    private Subject<Unit> _onDead = new();
    public IObservable<Unit> OnPlayerHit => _onDead;

    async void Start()
    {

        if (view == null)
            view = GetComponentInChildren<EnemyView>();

        var attackType = GetAttackType(_enemyType);

        _enemyController = new EnemyController(
            transform,
            player,
            view,
            config,
            _bulletFactory,
            attackType);

        var strategies = EnemyStrategyFactory.Create(_enemyType, _enemyController);

        _detect = strategies.Detect;
        _move = strategies.Move;
        _attack = strategies.Attack;

        _sm = new EnemyStateMachine();
        RegisterState();

        try
        {
            await _sm.ChangeStateAsync(EnemyStateKey.Idle);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    protected virtual void RegisterState()
    {
        _sm.RegisterState(
            EnemyStateKey.Idle,
            new EnemyIdleState(_enemyController, _detect, _move));

        _sm.RegisterState(
            EnemyStateKey.Telegraph,
            new EnemyTelegraphState(_enemyController, _move, _attack));

        _sm.RegisterState(
            EnemyStateKey.Cooldown,
            new EnemyCooldownState(_enemyController, _move, _attack));
    }

    void Update()
    {
        if (_isDead) return;
        _sm?.Tick();
    }

    public async UniTask DisableEnemy()
    {
        if (_isDead) return;
        _isDead = true;

        _sm?.Dispose();
        _sm = null;

        _enemyController?.StopRotateDecoration();
        _enemyController?.HideTelegraph();


        var controller = _enemyController;
        if (controller == null) return;

        await UniTask.WhenAll(
            controller.PlayDisappearAnimationAsync(),
            controller.PlayDisappearParticleAsync()
        );

        _enemyController = null;

        _disposables.Dispose();

        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _onDead.OnNext(Unit.Default);
        }
    }

    void OnDestroy()
    {
        _sm?.Dispose();
        _disposables.Dispose();
    }

    private static EnemyAttackType GetAttackType(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Enemy1 => EnemyAttackType.Single,
            _ => EnemyAttackType.Single
        };
    }
}