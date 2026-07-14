using System;
using UnityEngine;

/// <summary>
/// BGM‚Ìİ’è‚ğ•Û‚·‚éƒNƒ‰ƒX
/// </summary>
[Serializable]
public class BGMEntry
{
    public BGMType type;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Header("Playback Speed")]
    [Range(0.25f, 2f)]
    public float playbackSpeed = 1f;
}