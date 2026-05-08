using TMPro;
using UnityEngine;

public class LoadingProgressView : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("Format")]
    [SerializeField] private string format = "{0}%";

    private int _lastDisplayedValue = -1;

    /// <summary>
    /// 0.0〜1.0 の progress を受け取って % 表示する
    /// </summary>
    public void SetProgress(float progress)
    {
        if (progressText == null)
        {
            Debug.LogWarning($"{nameof(LoadingProgressView)}: progressText が未設定です。", this);
            return;
        }

        // 範囲補正
        progress = Mathf.Clamp01(progress);

        // %に変換（四捨五入）
        int percent = Mathf.RoundToInt(progress * 100f);

        // 無駄な更新を防ぐ
        if (percent == _lastDisplayedValue) return;

        _lastDisplayedValue = percent;

        progressText.text = string.Format(format, percent);
    }

    /// <summary>
    /// 即時初期化（0%表示など）
    /// </summary>
    public void ResetView()
    {
        _lastDisplayedValue = -1;
        SetProgress(0f);
    }
}