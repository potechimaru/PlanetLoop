using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// ローディングシーンで「Loading...」のようにドットが増えていくアニメーションを制御するクラス
/// </summary>
public class LoadingDotsAnimation : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI targetText;

    [Header("Base Text")]
    [SerializeField] private string baseText = "Loading";

    [Header("Interval (seconds)")]
    [SerializeField, Min(0.01f)] private float interval = 0.5f;

    [Header("Max Dots")]
    [SerializeField, Range(1, 5)] private int maxDots = 3;

    private CancellationTokenSource _cts;
    private bool _isPlaying;

    /// <summary>
    /// アニメーション開始
    /// </summary>
    public void Start()
    {
        if (_isPlaying) return;

        if (targetText == null)
        {
            Debug.LogError($"{nameof(LoadingDotsAnimation)}: targetText が未設定です。", this);
            return;
        }

        _isPlaying = true;
        _cts = new CancellationTokenSource();

        RunAsync(_cts.Token).Forget();
    }

    /// <summary>
    /// アニメーション停止
    /// </summary>
    public void Stop()
    {
        if (!_isPlaying) return;

        _isPlaying = false;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        // 停止時はベーステキストに戻す
        if (targetText != null)
        {
            targetText.text = baseText;
        }
    }

    private async UniTaskVoid RunAsync(CancellationToken token)
    {
        int dotCount = 1;

        try
        {
            while (!token.IsCancellationRequested)
            {
                targetText.text = baseText + new string('.', dotCount);

                dotCount++;
                if (dotCount > maxDots)
                    dotCount = 1;

                await UniTask.Delay(TimeSpan.FromSeconds(interval), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            // 正常終了
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
        }
    }

    private void OnDisable()
    {
        Stop();
    }

    private void OnDestroy()
    {
        Stop();
    }
}