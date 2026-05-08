using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;

public class PlayerController : ITickable
{
    private readonly PlayerModel _model;
    private readonly PlayerView _view;
    private readonly PlayerSplineMover _mover;
    private readonly AttachEvent _attachEvent;
    private readonly IPlayerExternalFacade _playerExternalFacade;

    private PlayerStateMachine _playerStateMachine;

    private Subject<Unit> _onLongJumped = new Subject<Unit>();
    public IObservable<Unit> OnLongJumped => _onLongJumped;

    private Subject<Vector3> _onNewOrbitAttached = new Subject<Vector3>();
    public IObservable<Vector3> OnNewOrbitAttached => _onNewOrbitAttached;

    public PlayerController(
        PlayerView view,
        IPlayerExternalFacade playerExternalFacade)
    {
        _model = new PlayerModel();
        _view = view;
        _playerExternalFacade = playerExternalFacade;
        _attachEvent = new AttachEvent(_view, _model,_onLongJumped, _onNewOrbitAttached);

        _mover = new PlayerSplineMover(
            _view,
            _model,
            _attachEvent,
            _playerExternalFacade);

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
            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        });

        _playerExternalFacade.OnPlayerEnteredBlackHole.Subscribe(_ =>
        {
            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        });

        _playerExternalFacade.OnPlayerExitedOuterLimit.Subscribe(_ =>
        {
            _playerStateMachine.ChangeState(PlayerStateKey.GameOver);
        });

    }

    private void CancelChargeAndReturnMove()
    {
        _model.InitializeMoveSpeed();
        _model.InitializeJumpSpeed();
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
        _mover.SetPlayer(_view.Spline, 0f);
    }

    public void StartMove()
    {
        _view.HideJumpNormalGuide();
        _view.SetAuraColor(ChargeLevel.Normal);
        _mover.InitializeMove();
        _model.InitializeMoveSpeed();
    }

    public void TickMove()
    {
        _mover.Tick(Time.deltaTime);
    }

    public void StartJump()
    {
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
    }

    public void TickCharge()
    {
        _model.CurrentChargeDuaration += Time.deltaTime;
        _model.ApplyChargeJumpSpeed();
        _model.ApplyChargeMoveSpeed();

        _view.SetAuraColor(_model.CurrentChargeLevel);
        _view.ShowJumpNormalGuide(_mover.GetOuterNormal());
    }

    public void Dead()
    {
        _view.PlayDeadEffect().Forget();
    }
}