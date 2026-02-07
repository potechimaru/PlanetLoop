using UnityEngine;
using VContainer.Unity;

public class PlayerController : ITickable
{
    private PlayerModel _model;
    private PlayerView _view;

    private PlayerSplineMover _mover;

    private IPlayerExternalFacade _playerExternalFacade;

    public PlayerController(PlayerView view, IPlayerExternalFacade playerExternalFacade)
    {
        _model = new PlayerModel();
        _view = view;

        _playerExternalFacade = playerExternalFacade;

        _mover = new PlayerSplineMover(_view, _model);
    }

    public void Tick()
    {
        _mover.Tick(Time.deltaTime);
        _view.Tick();
    }

    public void StartMove()
    {
        _playerExternalFacade.MoveSubscribe();
        _playerExternalFacade.JumpSubscribe(_mover.Jump);

    }


}
