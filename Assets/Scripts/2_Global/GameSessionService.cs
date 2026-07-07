using UnityEngine;

public class GameSessionService
{
    public GameModeType CurrentMode { get; private set; }
    public int CurrentScore { get; private set; }

    public LastResult LastResult { get; private set; }

    public void StartGame(GameModeType mode)
    {
        CurrentMode = mode;
        CurrentScore = 0;
    }

    public void EndGame(int finalScore)
    {
        CurrentScore = finalScore;

        LastResult = new LastResult
        {
            Mode = CurrentMode,
            Score = CurrentScore
        };

        Debug.Log($"Game ended. Mode: {CurrentMode}, Score: {CurrentScore}");
    }
}

public class LastResult
{
    public GameModeType Mode;
    public int Score;
}