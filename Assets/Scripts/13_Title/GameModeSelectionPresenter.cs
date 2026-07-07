using System;
using UniRx;
using VContainer.Unity;

public class GameModeSelectionPresenter : IStartable, IDisposable
{
    private readonly IGameModeManager _gameModeManager;
    private readonly IGameModeSelectionWriter _selectionWriter;

    private readonly CompositeDisposable _disposables = new();

    public GameModeSelectionPresenter(
        IGameModeManager gameModeManager,
        IGameModeSelectionWriter selectionWriter)
    {
        _gameModeManager = gameModeManager;
        _selectionWriter = selectionWriter;
    }

    public void Start()
    {
        // ‰Šú’l‚ðRoot‘¤‚Ö”½‰f
        _selectionWriter.SetSelectedMode(_gameModeManager.CurrentSelectedMode);

        // •ÏXŽž‚àRoot‘¤‚Ö”½‰f
        _gameModeManager.OnSelectedModeChanged
            .Subscribe(mode =>
            {
                _selectionWriter.SetSelectedMode(mode);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}