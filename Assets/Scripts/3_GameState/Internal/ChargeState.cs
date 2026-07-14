using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

/// <summary>
/// âΩÇÃÇΩÇﬂÇ…Ç§Ç‹ÇÍÇΩÇÒÇæÇÎÇ§ÅH
/// </summary>
public class ChargeState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();

    private readonly IGameStateExternalFacade _gameStateExternalFacade;

    public ChargeState(IGameStateExternalFacade gameStateExternalFacade)
    {
        _gameStateExternalFacade = gameStateExternalFacade;
    }

    public async UniTask Enter()
    {
        await UniTask.CompletedTask;
    }
    public async UniTask Exit()
    {
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        await UniTask.CompletedTask;
    }
}
