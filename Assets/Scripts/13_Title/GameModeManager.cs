using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface IGameModeManager
{
    GameModeType CurrentSelectedMode { get; }
    IReadOnlyList<GameModeDummyEntry> Entries { get; }

    IObservable<GameModeType> OnSelectedModeChanged { get; }

    void RotateRight();
    void RotateLeft();

    IReadOnlyList<ClosedSplineLine> GetCurrentSplineAssignments(IReadOnlyList<ClosedSplineLine> slotSplines);

    bool JudgeComminSoonGameMode(GameModeType gameModeType);
}

public class GameModeManager : IGameModeManager, IDisposable
{
    private readonly List<GameModeDummyEntry> _entries;
    private readonly ReactiveProperty<GameModeType> _selectedMode;
    private int _selectedIndex;
    private bool _disposed;

    public IReadOnlyList<GameModeDummyEntry> Entries => _entries;
    public GameModeType CurrentSelectedMode => _selectedMode.Value;

    public IObservable<GameModeType> OnSelectedModeChanged => _selectedMode;

    public GameModeManager(IEnumerable<GameModeDummyEntry> entries)
    {
        if (entries == null)
        {
            throw new ArgumentNullException(nameof(entries));
        }

        _entries = entries
            .Where(x => x != null)
            .OrderBy(x => x.Order)
            .ToList();

        if (_entries.Count == 0)
        {
            throw new InvalidOperationException($"{nameof(GameModeManager)}: GameModeDummyEntry ??1???????????B");
        }

        _selectedIndex = 0;

        _selectedMode = new ReactiveProperty<GameModeType>(_entries[_selectedIndex].GameMode);
    }

    public void RotateRight()
    {
        if (_entries.Count <= 1)
            return;

        _selectedIndex = (_selectedIndex + 1) % _entries.Count;

        _selectedMode.Value = _entries[_selectedIndex].GameMode;
    }

    public void RotateLeft()
    {
        if (_entries.Count <= 1)
            return;

        _selectedIndex = (_selectedIndex - 1 + _entries.Count) % _entries.Count;

        _selectedMode.Value = _entries[_selectedIndex].GameMode;
    }

    public IReadOnlyList<ClosedSplineLine> GetCurrentSplineAssignments(IReadOnlyList<ClosedSplineLine> slotSplines)
    {
        if (slotSplines == null)
        {
            throw new ArgumentNullException(nameof(slotSplines));
        }

        if (slotSplines.Count != _entries.Count)
        {
            throw new InvalidOperationException(
                $"{nameof(GameModeManager)}: slotSplines??({slotSplines.Count}) ?? GameModeDummyEntry??({_entries.Count}) ????v??????????B");
        }

        var result = new ClosedSplineLine[_entries.Count];

        for (int entryIndex = 0; entryIndex < _entries.Count; entryIndex++)
        {
            int slotIndex = (entryIndex - _selectedIndex + _entries.Count) % _entries.Count;
            result[entryIndex] = slotSplines[slotIndex];
        }

        return result;
    }

    public bool JudgeComminSoonGameMode(GameModeType gameModeType)
    {
        if (gameModeType == GameModeType.Endless)
        {
            return false;
        }
        return true;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _selectedMode.Dispose();
    }
}