using System;
using UnityEngine;

/// <summary>
/// SE（効果音）の設定を保持するクラス。音量、ピッチ、再生速度などの設定をインスペクタで操作可能
/// </summary>
[Serializable]
public class SEEntry
{
    public SEType type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Pitch")]

    [Tooltip("0=元の音、-12=1オクターブ下、+12=1オクターブ上")]
    [Range(-24, 24)]
    public int pitchSemitone = 0;

    [Tooltip("ピッチのランダム幅（半音）")]
    [Range(0f, 4f)]
    public float pitchRandomSemitone = 0f;

    [Header("Playback Speed")]

    [Tooltip("1=通常、0.5=半分、2=2倍")]
    [Range(0.25f, 5f)]
    public float playbackSpeed = 1f;
}