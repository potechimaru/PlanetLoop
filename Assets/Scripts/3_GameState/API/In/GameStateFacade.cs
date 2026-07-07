using UnityEngine;

public interface IGameStateFacade
{
    void RegisterRequestChangeState(GameStateKey key);
}

/// <summary>
/// GameStateコンポーネント群の内部メソッドを外部に公開するFacade。
/// 内部構造を隠蔽し、外部からのアクセスを簡素化する役割を持つ。
/// </summary>
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
