using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditorInternal;
using UnityEngine;
using VContainer.Unity;

public class PlayerController : ITickable
{
    private readonly PlayerModel _model;
    private readonly PlayerView _view;
    private readonly PlayerSplineMover _mover;

    private readonly IPlayerExternalFacade _playerExternalFacade;
    private PlayerStateMachine _playerStateMachine;

    public PlayerController(
    PlayerView view,
    IPlayerExternalFacade playerExternalFacade)
    {
        _model = new PlayerModel();
        _view = view;
        _playerExternalFacade = playerExternalFacade;

        _mover = new PlayerSplineMover(
            _view,
            _model,
            playerExternalFacade.TryFindTouchedSpline);

        _playerExternalFacade.MoveSubscribe(() =>
        {
            // ジャンプチャージ中は反転キャンセル
            if (_playerStateMachine.CurrentState is ChargeState)
            {
                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                _playerStateMachine.ChangeState(PlayerStateKey.Move);
            }
            // 通常は移動方向反転
            else
            {
                _model.Clockwise = !_model.Clockwise;
            }
        });
        _playerExternalFacade.JumpReleasedSubscribe(() =>
        {
            if (_playerStateMachine.CurrentState is MoveState) return;
            //Debug.Log("JumpReleased");
            _playerStateMachine.ChangeState(PlayerStateKey.Jump);
        });
        _playerExternalFacade.JumpPressedSubscribe(() =>
        {
            //Debug.Log("JumpPressed");
            _playerStateMachine.ChangeState(PlayerStateKey.Charge);
        });
    }

    public void SetPlayerStateMachine(PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
    }

    public void Tick()
    {
        _playerStateMachine?.Tick();
    }

    public void StartMove()
    {
        _mover.Initialize();
        _model.InitializeMoveSpeed();
    }

    public void TickMove()
    {
        _mover.Tick(Time.deltaTime);
    }

    public void StartJump()
    {
        _mover.Jump();
        var normal = _mover.GetOuterNormal();
        _view.StartJump(_view.transform.position, normal, _model.CurrentJumpspeed);
    }

    public bool TickJump()
    {
        _view.UpdateJump(Time.deltaTime);
        return _mover.TickJumpAndCheckAttach(_view.transform.position);
    }

    public void StartCharge()
    {
        _model.CurrentChargeDuaration = 0f;

    }

    public void TickCharge()
    {
        Debug.Log(_model.CurrentMoveSpeed);
        _model.CurrentChargeDuaration += Time.deltaTime;
        _model.ApplyChargeJumpSpeed();
        _model.ApplyChargeMoveSpeed();
    }

}
