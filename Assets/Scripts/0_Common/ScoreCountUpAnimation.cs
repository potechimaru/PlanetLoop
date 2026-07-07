using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// スコアをだんだんカウントアップして表示するアニメーションを制御するクラス
/// </summary>
public class ScoreCountUpAnimation : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float duration = 1.0f;
    [SerializeField] private Ease ease = Ease.OutCubic;
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Format")]
    [SerializeField] private string prefix = "";
    [SerializeField] private string suffix = "";

    [Header("Monospace")]
    [SerializeField] private bool useMonospace = true;
    [SerializeField] private float monospaceWidth = 40f;

    private Tween _tween;

    private void Awake()
    {
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();
    }

    // アニメーション開始（デフォルトは0から）
    public async UniTask PlayAsync(int targetScore)
    {
        await PlayAsync(0, targetScore);
    }

    public async UniTask PlayAsync(int fromScore, int targetScore)
    {
        _tween?.Kill();

        int currentScore = fromScore;
        ApplyText(currentScore);

        try
        {
            _tween = DOTween.To(
                    () => currentScore,
                    value =>
                    {
                        currentScore = value;
                        ApplyText(currentScore);
                    },
                    targetScore,
                    duration
                )
                .SetEase(ease)
                .SetUpdate(useUnscaledTime);

            await _tween.AsyncWaitForCompletion();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        finally
        {
            _tween = null;

            if (this != null && scoreText != null)
            {
                ApplyText(targetScore);
            }
        }
    }

    public void Stop()
    {
        _tween?.Kill();
        _tween = null;
    }

    private void ApplyText(int score)
    {
        if (scoreText == null) return;

        string text = $"{prefix}{score}{suffix}";

        scoreText.text = useMonospace
            ? $"<mspace={monospaceWidth}>{text}</mspace>"
            : text;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
        _tween = null;
    }
}