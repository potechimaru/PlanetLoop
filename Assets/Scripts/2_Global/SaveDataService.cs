using UnityEngine;

public class SaveDataService
{
    private const string KEY_PREFIX = "HighScore_Endless";
    private const string VISITED_SPLINE_COUNT_KEY_PREFIX = "MaxVisitedSplineCount_Endless";
    private const string DEFEATED_ENEMY_COUNT_KEY_PREFIX = "MaxDefeatedEnemyCount_Endless";
    private const string FIRST_PLAY_KEY = "IsFirstPlay_Endless";

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SE_VOLUME_KEY = "SEVolume";

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
            Debug.Log($"[Save] New high score for {mode}: {score}");
        }
    }

    public int GetMaxVisitedSplineCount(GameModeType mode)
    {
        return PlayerPrefs.GetInt(
            VISITED_SPLINE_COUNT_KEY_PREFIX + mode,
            0);
    }

    public void SetMaxVisitedSplineCount(
        GameModeType mode,
        int visitedSplineCount)
    {
        int current = GetMaxVisitedSplineCount(mode);

        if (visitedSplineCount <= current)
            return;

        PlayerPrefs.SetInt(
            VISITED_SPLINE_COUNT_KEY_PREFIX + mode,
            visitedSplineCount);

        PlayerPrefs.Save();

        Debug.Log(
            $"[Save] New max visited spline count for {mode}: " +
            $"{visitedSplineCount}");
    }

    public int GetMaxDefeatedEnemyCount(GameModeType mode)
    {
        return PlayerPrefs.GetInt(
            DEFEATED_ENEMY_COUNT_KEY_PREFIX + mode,
            0);
    }

    public void SetMaxDefeatedEnemyCount(
        GameModeType mode,
        int defeatedEnemyCount)
    {
        int current = GetMaxDefeatedEnemyCount(mode);

        if (defeatedEnemyCount <= current)
            return;

        PlayerPrefs.SetInt(
            DEFEATED_ENEMY_COUNT_KEY_PREFIX + mode,
            defeatedEnemyCount);

        PlayerPrefs.Save();

        Debug.Log(
            $"[Save] New max defeated enemy count for {mode}: " +
            $"{defeatedEnemyCount}");
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

    public bool IsFirstPlay()
    {
        return !PlayerPrefs.HasKey(FIRST_PLAY_KEY);
    }

    public void MarkTutorialShown()
    {
        PlayerPrefs.SetInt(FIRST_PLAY_KEY, 1);
        PlayerPrefs.Save();
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat(BGM_VOLUME_KEY, 0.5f);
    }

    public void SetBGMVolume(float volume)
    {
        PlayerPrefs.SetFloat(
            BGM_VOLUME_KEY,
            Mathf.Clamp01(volume));

        PlayerPrefs.Save();
    }

    public float GetSEVolume()
    {
        return PlayerPrefs.GetFloat(SE_VOLUME_KEY, 0.5f);
    }

    public void SetSEVolume(float volume)
    {
        PlayerPrefs.SetFloat(
            SE_VOLUME_KEY,
            Mathf.Clamp01(volume));

        PlayerPrefs.Save();
    }
}