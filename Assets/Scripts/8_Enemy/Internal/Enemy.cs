using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer;

public class Enemy : MonoBehaviour, IEnemyContactHandle
{
    [Header("Refs")]
    [SerializeField] private EnemyView view;

    [Header("Common Config")]
    [SerializeField] private EnemyConfig config = new();

    [SerializeField] private EnemyType _enemyType = EnemyType.Enemy1;
    public EnemyType EnemyType => _enemyType;

    [Inject] private IEnemyExternalFacade _enemyExternalFacade;
    [Inject] private EnemyBulletFactory _bulletFactory;
    [Inject] private EnemyLaserFactory _laserFactory;
    

    private Transform _playerTransform;

    protected EnemyController _enemyController;
    protected EnemyStateMachine _sm;

    protected IDetectStrategy _detect;
    protected IMoveStrategy _move;
    protected IAttackStrategy _attack;

    private readonly CompositeDisposable _disposables = new();
    private bool _isDead;

    private readonly Subject<Unit> _onPlayerTouched = new();
    public IObservable<Unit> OnPlayerTouched => _onPlayerTouched;

    private EnemySpawnPoint _spawnPoint;

    private bool _isSimulationEnabled = true;

    public void InitializeForSpawn(Transform playerTransform)
    {
        _isDead = false;

        _playerTransform = playerTransform;

        if (view == null)
            view = GetComponentInChildren<EnemyView>();

        var attackType = GetAttackType(_enemyType);

        _enemyController = new EnemyController(
            transform,
            _playerTransform,
            view,
            config,
            _bulletFactory,
            _laserFactory,
            attackType);

        var strategies = EnemyStrategyFactory.Create(_enemyType, _enemyController);

        _detect = strategies.Detect;
        _move = strategies.Move;
        _attack = strategies.Attack;

        _sm = new EnemyStateMachine();
        RegisterState();

        _sm.ChangeStateAsync(EnemyStateKey.Idle).Forget();

        _enemyController?.HideTelegraph();
    }

    protected virtual void RegisterState()
    {
        _sm.RegisterState(
            EnemyStateKey.Idle,
            new EnemyIdleState(_enemyController, _detect, _move));

        _sm.RegisterState(
            EnemyStateKey.Telegraph,
            new EnemyTelegraphState(_enemyController, _move, _attack, _enemyExternalFacade));

        _sm.RegisterState(
            EnemyStateKey.Cooldown,
            new EnemyCooldownState(_enemyController, _move, _attack));
    }

    public void SetSimulationEnabled(bool enabled)
    {
        if (_isSimulationEnabled == enabled)
            return;

        _isSimulationEnabled = enabled;

        _enemyController?.SetRenderingEnabled(enabled);

        if (!enabled)
        {
            _enemyController?.HideTelegraph();
            _enemyController?.StopRotateDecoration();
        }
        else
        {
            _enemyController?.RotateDecoration();
        }
    }

    private void Update()
    {
        if (_isDead) return;
        if (!_isSimulationEnabled) return;

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

        _spawnPoint?.Release();
        _spawnPoint = null;

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDead) return;
        if (!collision.CompareTag("Player")) return;

        _onPlayerTouched.OnNext(Unit.Default);
    }

    public void SetSpawnPoint(EnemySpawnPoint spawnPoint)
    {
        _spawnPoint = spawnPoint;
    }

    public void SetDetectionEnabled(bool enabled)
    {
        _enemyController?.SetDetectionEnabled(enabled);
    }

    public void Defeat()
    {
        DisableEnemy().Forget();
    }

    private void OnDestroy()
    {
        _sm?.Dispose();
        _disposables.Dispose();
        _onPlayerTouched.Dispose();
    }

    private static EnemyAttackType GetAttackType(EnemyType enemyType)
    {
        return enemyType switch
        {
            EnemyType.Enemy1 => EnemyAttackType.Single,
            EnemyType.Enemy2 => EnemyAttackType.Spread,
            EnemyType.Enemy3 => EnemyAttackType.LargeSingle,
            EnemyType.Enemy4 => EnemyAttackType.LargeSpread,
            EnemyType.Enemy5 => EnemyAttackType.Laser,
            _ => EnemyAttackType.Single
        };
    }
}