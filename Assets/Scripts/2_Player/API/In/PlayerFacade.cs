using UnityEngine;

public interface IPlayerFacade
{
    void StartMove();

}

public class PlayerFacade : IPlayerFacade
{
    private readonly PlayerStateMachine _playerStateMachine;
    private readonly PlayerController _playerController;

    public PlayerFacade(PlayerStateMachine playerStateMachine, PlayerController playerController)
    {
        _playerStateMachine = playerStateMachine;
        _playerController = playerController;

    }

    public void StartMove()
    {
        _playerStateMachine.StartMove();
    }
}
