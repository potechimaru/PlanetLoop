using System;
using UniRx;

public interface IGameModeSelectionReader
{
    GameModeType CurrentSelectedMode { get; }
    IObservable<GameModeType> OnSelectedModeChanged { get; }
}

public interface IGameModeSelectionWriter
{
    void SetSelectedMode(GameModeType gameModeType);
}

public class GameModeSelectionService :
    IGameModeSelectionReader,
    IGameModeSelectionWriter,
    IDisposable
{
    private readonly ReactiveProperty<GameModeType> _selectedMode =
        new(GameModeType.Endless);

    public GameModeType CurrentSelectedMode => _selectedMode.Value;

    public IObservable<GameModeType> OnSelectedModeChanged => _selectedMode;

    public void SetSelectedMode(GameModeType gameModeType)
    {
        if (_selectedMode.Value == gameModeType)
            return;

        _selectedMode.Value = gameModeType;
    }

    public void Dispose()
    {
        _selectedMode.Dispose();
    }
}