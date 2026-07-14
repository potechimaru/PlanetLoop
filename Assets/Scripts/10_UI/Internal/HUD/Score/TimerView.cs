using TMPro;
using UnityEngine;

/// <summary>
/// プレイヤーの生存時間を表示するViewクラス。デジタル時計形式。
/// </summary>
public class TimerView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private float characterWidth = 40f;

    public void SetTime(float elapsedSeconds)
    {
        if (_timerText == null) return;

        int totalSeconds = Mathf.FloorToInt(elapsedSeconds);

        int hours = totalSeconds / 3600;
        int minutes = totalSeconds % 3600 / 60;
        int seconds = totalSeconds % 60;

        string timeText = $"{hours:00}:{minutes:00}:{seconds:00}";
        _timerText.text = $"<mspace={characterWidth}>{timeText}</mspace>";
    }

    public void ResetTime()
    {
        SetTime(0f);
    }
}