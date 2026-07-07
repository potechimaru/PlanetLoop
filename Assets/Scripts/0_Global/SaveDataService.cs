using UnityEngine;

public class SaveDataService
{
    private const string KEY_PREFIX = "HighScore_";
    private const string FIRST_PLAY_KEY = "IsFirstPlay";

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