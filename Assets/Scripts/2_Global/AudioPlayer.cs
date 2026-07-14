using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各BGMやSEのAudioClipを管理するクラス。音量やピッチ、再生速度などの設定もここで行う。
/// インスペクタにて操作可能
/// </summary>
public class AudioPlayer : MonoBehaviour
{
    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioSource loopSeSource;

    [Header("BGM")]
    [SerializeField] private List<BGMEntry> bgmEntries = new();

    [Header("SE")]
    [SerializeField] private List<SEEntry> seEntries = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public BGMEntry GetBGMEntry(BGMType type)
    {
        foreach (var entry in bgmEntries)
        {
            if (entry != null && entry.type == type)
                return entry;
        }

        Debug.LogWarning($"[AudioPlayer] BGMEntry が見つかりません: {type}", this);
        return null;
    }

    public SEEntry GetSEEntry(SEType type)
    {
        foreach (var entry in seEntries)
        {
            if (entry != null && entry.type == type)
                return entry;
        }

        Debug.LogWarning($"[AudioPlayer] SEEntry が見つかりません: {type}", this);
        return null;
    }

    public AudioSource BGMSource => bgmSource;
    public AudioSource SESource => seSource;
    public AudioSource LoopSESource => loopSeSource;
}