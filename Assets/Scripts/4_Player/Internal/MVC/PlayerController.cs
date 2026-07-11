using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class PlayerController : ITickable, IDisposable
{
    private readonly PlayerModel _model;
    private readonly PlayerView _view;
    private readonly PlayerSplineMover _mover;
    private readonly AttachEvent _attachEvent;
    private readonly IPlayerExternalFacade _playerExternalFacade;
    private readonly PlayerSpawnOverlapResolver _spawnOverlapResolver;

    private PlayerStateMachine _playerStateMachine;

    private readonly CompositeDisposable _playerSubscriptions = new();

    private Subject<Unit> _onLongJumped = new Subject<Unit>();
    public IObservable<Unit> OnLongJumped => _onLongJumped;

    private Subject<Unit> _onNewOrbitAttached = new Subject<Unit>();
    public IObservable<Unit> OnNewOrbitAttached => _onNewOrbitAttached;

    private Subject<Unit> _onPlayerDead = new Subject<Unit>();
    public IObservable<Unit> OnPlayerDead => _onPlayerDead;

    private ChargeLevel _previousChargeLevel = ChargeLevel.Normal;

    public PlayerController(
    PlayerView view,
    IPlayerExternalFacade playerExternalFacade,
    PlayerSpawnOverlapResolver spawnOverlapResolver)
    {
        _model = new PlayerModel();
        _view = view;
        _playerExternalFacade = playerExternalFacade;
        _spawnOverlapResolver = spawnOverlapResolver;

        _attachEvent = new AttachEvent(_view, _model, _onLongJumped, _onNewOrbitAttached);

        _mover = new PlayerSplineMover(
            _view,
            _model,
            _attachEvent,
            _playerExternalFacade,
            _spawnOverlapResolver);
    }

    public void RegisterInputSubscriptions()
    {
        _playerExternalFacade.MoveSubscribe(() =>
        {
            if (_playerStateMachine.CurrentState is GameOverState) return;

            if (_playerStateMachine.CurrentState is ChargeState)
            {
                CancelChargeAndReturnMove();
                return;
            }

            _model.Clockwise = !_model.Clockwise;
            //_view.FlipRotateUI();
        });

        _playerExternalFacade.JumpReleasedSubscribe(() =>
        {
            if (_playerStateMachine.CurrentState is MoveState) return;
            if (_playerStateMachine.CurrentState is JumpState) return;
            if (_playerStateMachine.CurrentState is GameOverState) return;

            _playerStateMachine.ChangeState(PlayerStateKey.Jump);
        });

        _playerExternalFacade.JumpPressedSubscribe(() =>
        {
            if (_playerStateMachine.CurrentState is JumpState) return;
            if (_playerStateMachine.CurrentState is GameOverState) return;

            _playerStateMachine.ChangeState(PlayerStateKey.Charge);
        });
    }

    public void RegisterPlayerSubscriptions()
    {
        _playerExternalFacade.OnPlayerHitByEnemyBullet.Subscribe(_ =>
        {
            if (TryGuardEnemyBullet())
                return;

            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        }).AddTo(_playerSubscriptions);

        _playerExternalFacade.OnPlayerHitByLaserBeam.Subscribe(_ =>
        {
            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        }).AddTo(_playerSubscriptions);

        _playerExternalFacade.OnPlayerHitObstacle.Subscribe(_ =>
        {
            if (TryGuardObstacle())
                return;

            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        }).AddTo(_playerSubscriptions);

        _playerExternalFacade.OnPlayerEnteredBlackHole.Subscribe(_ =>
        {
            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        }).AddTo(_playerSubscriptions);

        _playerExternalFacade.OnPlayerExitedOuterLimit.Subscribe(_ =>
        {
            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        }).AddTo(_playerSubscriptions);

        _playerExternalFacade.OnPlayerTouchedEnemy.Subscribe(enemyHandle =>
        {
            _playerExternalFacade.DefeatEnemy(enemyHandle);
        }).AddTo(_playerSubscriptions);
    }

    private void CancelChargeAndReturnMove()
    {
        _model.GuardCount = 0;

        _playerExternalFacade.StopLoopSE();

        _model.InitializeMoveSpeed();
        _model.InitializeJumpSpeed();

        _view.SetAuraColor(ChargeLevel.Normal);

        _playerStateMachine.ChangeState(PlayerStateKey.Move);
    }

    public void SetPlayerStateMachine(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
    }

    public void Tick()
    {
        _playerStateMachine?.Tick();
    }

    public void SetPlayer()
    {
        _mover.SetPlayer(_playerExternalFacade.StartSpline, 0f);
    }

    public void StartMove()
    {
        _view.HideJumpNormalGuide();
        _view.SetAuraColor(ChargeLevel.Normal);

        _mover.InitializeMove();
        _model.InitializeMoveSpeed();

        _model.GuardCount = 0;
    }

    public void TickMove()
    {
        _mover.Tick(Time.deltaTime);
    }

    public void StartJump()
    {
        RefreshGuardCount();

        _playerExternalFacade.StopLoopSE();

        _view.HideJumpNormalGuide();
        _mover.StartJump();
    }
    public bool TickJump()
    {
        return _mover.TickJump(Time.deltaTime);
    }

    public void StartCharge()
    {
        _model.CurrentChargeDuaration = 0f;
        _model.GuardCount = 0;

        _previousChargeLevel = ChargeLevel.Normal;
        _playerExternalFacade.StopLoopSE();
    }

    public void TickCharge()
    {
        _model.CurrentChargeDuaration += Time.deltaTime;
        _model.ApplyChargeJumpSpeed();
        _model.ApplyChargeMoveSpeed();

        ChargeLevel currentLevel = _model.CurrentChargeLevel;

        if (currentLevel != _previousChargeLevel)
        {
            switch (currentLevel)
            {
                case ChargeLevel.Charge1:
                    _playerExternalFacade.StopLoopSE();
                    _playerExternalFacade.StartLoopSE(SEType.Charge1);
                    break;

                case ChargeLevel.Charge2:
                    _playerExternalFacade.StopLoopSE();
                    _playerExternalFacade.StartLoopSE(SEType.Charge2);
                    break;

                case ChargeLevel.Normal:
                default:
                    _playerExternalFacade.StopLoopSE();
                    break;
            }

            _previousChargeLevel = currentLevel;
        }

        _view.SetAuraColor(currentLevel);
        _view.ShowJumpNormalGuide(_mover.GetOuterNormal());
    }

    private void RefreshGuardCount()
    {
        switch (_model.CurrentChargeLevel)
        {
            case ChargeLevel.Charge1:
                _model.GuardCount = 1;
                break;

            case ChargeLevel.Charge2:
                _model.GuardCount = 2;
                break;

            case ChargeLevel.Normal:
            default:
                _model.GuardCount = 0;
                break;
        }
    }

    private bool TryGuardEnemyBullet()
    {
        if (_model.GuardCount <= 0)
            return false;

        _model.GuardCount--;

        switch (_model.GuardCount)
        {
            case 1:
                _view.SetAuraColor(ChargeLevel.Charge1);
                break;

            case 0:
                _view.SetAuraColor(ChargeLevel.Normal);

                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        return true;
    }

    private bool TryGuardObstacle()
    {
        if (_model.GuardCount <= 0)
            return false;

        _model.GuardCount--;

        switch (_model.GuardCount)
        {
            case 1:
                _view.SetAuraColor(ChargeLevel.Charge1);
                _model.SetCharge1JumpSpeed();
                _model.SetCharge1MoveSpeed();
                break;

            case 0:
                _view.SetAuraColor(ChargeLevel.Normal);
                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        return true;
    }

    private bool TryGuardEnemyContact()
    {
        if (_model.GuardCount <= 0)
            return false;

        _model.GuardCount--;

        switch (_model.GuardCount)
        {
            case 1:
                _view.SetAuraColor(ChargeLevel.Charge1);
                _model.SetCharge1JumpSpeed();
                _model.SetCharge1MoveSpeed();
                break;

            case 0:
                _view.SetAuraColor(ChargeLevel.Normal);
                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        return true;
    }

    public void Dead()
    {
        _onPlayerDead.OnNext(Unit.Default);
        _playerExternalFacade.PlaySE(SEType.PlayerDead);
        _view.PlayDeadEffect().Forget();
        _view.HideJumpNormalGuide();
        _playerExternalFacade.StopLoopSE();
    }

    public void Dispose()
    {
        _playerSubscriptions.Dispose();
        _onLongJumped.Dispose();
        _onNewOrbitAttached.Dispose();
        _onPlayerDead.Dispose();
    }
}