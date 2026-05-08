using UnityEngine;

public class SaveDataService
{
    private const string KEY_PREFIX = "HighScore_";

    public int GetHighScore(GameModeType mode)
    {
        return PlayerPrefs.GetInt(KEY_PREFIX + mode, 0);
    }

    public void SetHighScore(GameModeType mode, int score)
    {
        int current = GetHighScore(mode);

        if (score > current)
        {
            PlayerPrefs.SetInt(KEY_PREFIX + mode, score);
            PlayerPrefs.Save();
        }
    }

    public void SetLastSelectedMode(GameModeType mode)
    {
        PlayerPrefs.SetInt("LastMode", (int)mode);
        PlayerPrefs.Save();
    }

    public GameModeType GetLastSelectedMode()
    {
        return (GameModeType)PlayerPrefs.GetInt("LastMode", 0);
    }
}