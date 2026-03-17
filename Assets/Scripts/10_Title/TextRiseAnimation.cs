using UnityEngine;
using DG.Tweening;

public class TextRiseAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform text;

    [Header("Animation")]
    [SerializeField] private float moveDistance = 80f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    [Header("Delay")]
    [SerializeField] private float delay = 0f;

    private Vector2 _initPos;
    private Tween _tween;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<RectTransform>();

        _initPos = text.anchoredPosition;
        Play();
    }

    public void Play()
    {
        _tween?.Kill();

        // â∫Ç…âBÇ∑ÅiÇ±ÇÍÇÕë¶éûé¿çsÅj
        text.anchoredPosition = _initPos - new Vector2(0, moveDistance);

        _tween = text.DOAnchorPosY(_initPos.y, duration)
                     .SetEase(ease)
                     .SetDelay(delay);
    }

    public void ResetState()
    {
        _tween?.Kill();
        text.anchoredPosition = _initPos - new Vector2(0, moveDistance);
    }

    public void OnDestroy()
    {
        _tween?.Kill();
    }
}

