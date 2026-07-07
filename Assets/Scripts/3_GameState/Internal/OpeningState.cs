using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;

public class OpeningState : IGameState
{
    public ReactiveCommand<GameStateKey> NextState { get; } = new();
    private readonly IGameStateExternalFacade _gameStateExternalFacade;
    private readonly GameUIManager _gameUIManager;
    private readonly SaveDataService _saveDataService;
    private readonly AudioManager _audioManager;

    public OpeningState(IGameStateExternalFacade gameStateExternalFacade, GameUIManager gameUIManager, SaveDataService saveDataService, AudioManager audioManager)
    {
        _gameStateExternalFacade = gameStateExternalFacade;
        _gameUIManager = gameUIManager;
        _saveDataService = saveDataService;
        _audioManager = audioManager;
    }

    public async UniTask Enter()
    {
        _gameStateExternalFacade.RegisterPlayerSubscriptions();
        _gameStateExternalFacade.SetPlayer();
        _gameStateExternalFacade.SetAllDetectionEnabled(false);
        Time.timeScale = 0f;
        await _gameUIManager.GameOpening();
        //if(_saveDataService.IsFirstPlay())
        //{
            _audioManager.PlayBGM(BGMType.Game);
            await _gameUIManager.ShowTutorial();
            await _gameUIManager.WaitUntilTutorialClosed();
            _saveDataService.MarkTutorialShown();
        //}
        //else 
        //{
        //    _gameStateExternalFacade.PlayBGM(BGMType.Game);
        //}
            await _gameUIManager.ShowPreGame();
    }
    public async UniTask Exit()
    {
        Time.timeScale = 1f;
        _gameStateExternalFacade.SetAllDetectionEnabled(true);
        //Debug.Log("Exiting Opening State");
        await UniTask.CompletedTask;
    }
    public async UniTask Tick()
    {
        // Logic for the opening state
        await UniTask.CompletedTask;
    }
}
