using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInAnimation : MonoBehaviour
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

    public void Play()
    {
        _tween?.Kill();

        // ‘S‘Ì‚ð“§–¾‚É
        _canvasGroup.alpha = 0f;

        _tween = _canvasGroup
            .DOFade(1f, duration)
            .SetEase(ease)
            .SetDelay(delay);
    }

    public void ResetState()
    {
        _tween?.Kill();
        _canvasGroup.alpha = 0f;
    }

    public void FadeOut()
    {
        _tween?.Kill();

        _tween = _canvasGroup
            .DOFade(0f, duration)
            .SetEase(ease);
    }
}
