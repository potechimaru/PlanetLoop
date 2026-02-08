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
            Debug.Log("RightClick");
            _model.Clockwise = !_model.Clockwise;
        });
        _playerExternalFacade.JumpSubscribe(() =>
        {
            Debug.Log("Jump");
            _playerStateMachine.ChangeState(PlayerStateKey.Jump);
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
    }

    public void TickMove()
    {
        _mover.Tick(Time.deltaTime);
    }

    public void StartJump()
    {
        _mover.Jump();
        var normal = _mover.GetOuterNormal();
        _view.StartJump(_view.transform.position, normal, _model.Jumpspeed);
    }

    public bool TickJump()
    {
        _view.UpdateJump(Time.deltaTime);
        return _mover.TickJumpAndCheckAttach(_view.transform.position);
    }

}
