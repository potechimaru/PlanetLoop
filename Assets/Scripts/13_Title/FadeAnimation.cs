using UnityEngine;
using DG.Tweening;

/// <summary>
/// UIのCanvasGroupを使ったフェードイン・フェードアウトアニメーションを制御するクラス。
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class FadeAnimation : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private CanvasGroup _canvasGroup;
    private Tween _tween;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 指定したαまでフェードイン（現在値 → target）
    /// </summary>
    public void FadeIn(float targetAlpha = 1f)
    {
        _tween?.Kill();

        _tween = _canvasGroup
            .DOFade(targetAlpha, duration)
            .SetEase(ease)
            .SetDelay(delay);
    }

    /// <summary>
    /// 指定したαまでフェードアウト（現在値 → target）
    /// </summary>
    public void FadeOut(float targetAlpha = 0f)
    {
        _tween?.Kill();

        _tween = _canvasGroup
            .DOFade(targetAlpha, duration)
            .SetEase(ease)
            .SetDelay(delay);
    }

    /// <summary>
    /// 0 → targetAlpha にしたいとき用（従来Play相当）
    /// </summary>
    public void FadeInFromZero(float targetAlpha = 1f)
    {
        _tween?.Kill();

        _canvasGroup.alpha = 0f;

        _tween = _canvasGroup
            .DOFade(targetAlpha, duration)
            .SetEase(ease)
            .SetDelay(delay);
    }

    public void ResetState(float alpha = 0f)
    {
        _tween?.Kill();
        _canvasGroup.alpha = alpha;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}