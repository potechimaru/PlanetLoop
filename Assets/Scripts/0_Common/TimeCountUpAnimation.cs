using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// 生存時間をだんだんカウントアップして表示するアニメーションを制御するクラス
/// </summary>
public class TimeCountUpAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float characterWidth = 40f;

    public async UniTask PlayAsync(float targetSeconds)
    {
        if (_timeText == null) return;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float current = Mathf.Lerp(0f, targetSeconds, t);

            SetTime(current);

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        SetTime(targetSeconds);
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