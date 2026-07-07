using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioManager
{
    private readonly AudioPlayer _audioPlayer;
    private readonly SaveDataService _saveDataService;

    private float _bgmVolume = 1f;
    private float _seVolume = 1f;

    private float BGMVolumeScale => _bgmVolume * 2f;
    private float SEVolumeScale => _seVolume * 2f;

    private BGMType? _currentBgm;
    private SEType? _currentLoopSe;

    private CancellationTokenSource _loopSeStopCts;

    public AudioManager(
    AudioPlayer audioPlayer,
    SaveDataService saveDataService)
    {
        _audioPlayer = audioPlayer;
        _saveDataService = saveDataService;

        _bgmVolume = _saveDataService.GetBGMVolume();
        _seVolume = _saveDataService.GetSEVolume();
    }

    #region Volume

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = Mathf.Clamp01(volume);

        _saveDataService.SetBGMVolume(_bgmVolume);

        ApplyBGMVolume();
    }

    public void SetSEVolume(float volume)
    {
        _seVolume = Mathf.Clamp01(volume);

        _saveDataService.SetSEVolume(_seVolume);

        ApplyLoopSEVolume();
    }

    public float GetBGMVolume()
    {
        return _bgmVolume;
    }

    public float GetSEVolume()
    {
        return _seVolume;
    }

    private void ApplyBGMVolume()
    {
        if (_audioPlayer == null)
            return;

        var source = _audioPlayer.BGMSource;
        if (source == null)
            return;

        if (_currentBgm == null)
            return;

        var entry = _audioPlayer.GetBGMEntry(_currentBgm.Value);
        if (entry == null)
            return;

        source.volume = entry.volume * BGMVolumeScale;
    }

    private void ApplyLoopSEVolume()
    {
        if (_audioPlayer == null)
            return;

        var source = _audioPlayer.LoopSESource;
        if (source == null)
            return;

        if (_currentLoopSe == null)
            return;

        var entry = _audioPlayer.GetSEEntry(_currentLoopSe.Value);
        if (entry == null)
            return;

        source.volume = entry.volume * SEVolumeScale;
    }

    #endregion

    #region BGM

    public void PlayBGM(BGMType type)
    {
        if (_audioPlayer == null)
            return;

        if (_currentBgm == type)
            return;

        var entry = _audioPlayer.GetBGMEntry(type);

        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[AudioManager] BGM clip Ç™ñ¢ê›íËÇ≈Ç∑: {type}");
            return;
        }

        var source = _audioPlayer.BGMSource;

        if (source == null)
        {
            Debug.LogError("[AudioManager] BGMSource Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        source.clip = entry.clip;
        source.volume = entry.volume * BGMVolumeScale;
        source.pitch = entry.playbackSpeed;
        source.loop = true;
        source.Play();

        _currentBgm = type;
    }

    public void StopBGM()
    {
        if (_audioPlayer == null)
            return;

        var source = _audioPlayer.BGMSource;

        if (source == null)
            return;

        source.Stop();
        source.clip = null;
        source.pitch = 1f;

        _currentBgm = null;
    }

    #endregion

    #region Loop SE

    public void StartLoopSE(SEType type)
    {
        if (_audioPlayer == null)
            return;

        if (_currentLoopSe == type)
            return;

        CancelLoopSeStopTask();

        var entry = _audioPlayer.GetSEEntry(type);

        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[AudioManager] SE clip Ç™ñ¢ê›íËÇ≈Ç∑: {type}");
            return;
        }

        var source = _audioPlayer.LoopSESource;

        if (source == null)
        {
            Debug.LogError("[AudioManager] LoopSESource Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        source.Stop();

        float semitone =
            entry.pitchSemitone +
            UnityEngine.Random.Range(
                -entry.pitchRandomSemitone,
                 entry.pitchRandomSemitone);

        float pitch = Mathf.Pow(2f, semitone / 12f);

        source.clip = entry.clip;
        source.volume = entry.volume * SEVolumeScale;
        source.pitch = pitch * entry.playbackSpeed;
        source.loop = true;
        source.Play();

        _currentLoopSe = type;
    }

    public void StopLoopSE()
    {
        if (_audioPlayer == null)
            return;

        var source = _audioPlayer.LoopSESource;

        if (source == null)
            return;

        if (!source.isPlaying)
        {
            ClearLoopSESource(source);
            return;
        }

        source.loop = false;

        CancelLoopSeStopTask();
        _loopSeStopCts = new CancellationTokenSource();

        WaitUntilLoopSEFinishedAsync(
            source,
            _loopSeStopCts.Token).Forget();
    }

    private async UniTaskVoid WaitUntilLoopSEFinishedAsync(
        AudioSource source,
        CancellationToken token)
    {
        try
        {
            await UniTask.WaitWhile(
                () => source != null && source.isPlaying,
                cancellationToken: token
            );

            if (token.IsCancellationRequested)
                return;

            ClearLoopSESource(source);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[AudioManager] LoopSE stop error: {e}");
        }
    }

    private void ClearLoopSESource(AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.clip = null;
        source.loop = false;
        source.pitch = 1f;

        _currentLoopSe = null;
    }

    private void CancelLoopSeStopTask()
    {
        if (_loopSeStopCts == null)
            return;

        _loopSeStopCts.Cancel();
        _loopSeStopCts.Dispose();
        _loopSeStopCts = null;
    }

    #endregion

    #region OneShot SE

    public void PlaySE(SEType type)
    {
        if (_audioPlayer == null)
            return;

        var entry = _audioPlayer.GetSEEntry(type);

        if (entry == null || entry.clip == null)
        {
            Debug.LogWarning($"[AudioManager] SE clip Ç™ñ¢ê›íËÇ≈Ç∑: {type}");
            return;
        }

        var source = _audioPlayer.SESource;

        if (source == null)
        {
            Debug.LogError("[AudioManager] SESource Ç™ñ¢ê›íËÇ≈Ç∑ÅB");
            return;
        }

        float originalPitch = source.pitch;

        float semitone =
            entry.pitchSemitone +
            UnityEngine.Random.Range(
                -entry.pitchRandomSemitone,
                 entry.pitchRandomSemitone);

        float pitch = Mathf.Pow(2f, semitone / 12f);

        source.pitch = pitch * entry.playbackSpeed;

        source.PlayOneShot(
            entry.clip,
            entry.volume * SEVolumeScale);

        source.pitch = originalPitch;
    }

    #endregion
}