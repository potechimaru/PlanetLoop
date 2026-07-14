using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 生存時間をカウントアップ表示するアニメーションを制御するクラス
/// </summary>
public class TimeCountUpAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float characterWidth = 40f;

    /// <summary>
    /// 0秒から指定の秒数までカウントアップするアニメーションを再生する
    /// </summary>
    /// <param name="targetSeconds">終端時間（どこまでカウントアップするか）</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask PlayAsync(float targetSeconds, CancellationToken cancellationToken = default)
    {
        if (_timeText == null) return;

        try
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                cancellationToken.ThrowIfCancellationRequested();

                elapsed += Time.unscaledDeltaTime;

                float t = Mathf.Clamp01(elapsed / duration);
                float current = Mathf.Lerp(0f, targetSeconds, t);

                SetTime(current);

                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            }

            SetTime(targetSeconds);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void SetTime(float seconds)
    {
        int totalSeconds = Mathf.FloorToInt(seconds);

        int hours = totalSeconds / 3600;
        int minutes = totalSeconds % 3600 / 60;
        int sec = totalSeconds % 60;

        string timeText = $"{hours:00}:{minutes:00}:{sec:00}";
        _timeText.text = $"<mspace={characterWidth}>{timeText}</mspace>";
    }
}