using UnityEngine;

public interface IGameStateFacade
{
    void RegisterRequestChangeState(GameStateKey key);
}

public class GameStateFacade : IGameStateFacade
{
    private readonly IGameStateChangeRequester _requestHub;

    public GameStateFacade(IGameStateChangeRequester requestHub)
    {
        _requestHub = requestHub;
    }

    public void RegisterRequestChangeState(GameStateKey key)
    {
        _requestHub.Request(key);
    }

}
