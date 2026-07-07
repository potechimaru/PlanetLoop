using UnityEngine;
using DG.Tweening;

/// <summary>
/// モード選択画面にてボタンのホバー時に拡大するアニメーションを制御するクラス
/// </summary>
public class HoverScaleAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Hover Animation")]
    [SerializeField] private float hoverScale = 1.08f;
    [SerializeField] private float hoverDuration = 0.15f;
    [SerializeField] private Ease hoverEase = Ease.OutCubic;
    [SerializeField] private Ease returnEase = Ease.OutCubic;

    private Tween _scaleTween;
    private Vector3 _initialScale;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (target != null)
            _initialScale = target.localScale;
    }

    public void PlayHoverEnter()
    {
        if (target == null) return;

        _scaleTween?.Kill();
        _scaleTween = target.DOScale(_initialScale * hoverScale, hoverDuration)
            .SetEase(hoverEase);
    }

    public void PlayHoverExit()
    {
        if (target == null) return;

        _scaleTween?.Kill();
        _scaleTween = target.DOScale(_initialScale, hoverDuration)
            .SetEase(returnEase);
    }

    public Vector3 GetHoverScale()
    {
        return _initialScale * hoverScale;
    }

    public Vector3 GetInitialScale()
    {
        return _initialScale;
    }

    public void Kill()
    {
        _scaleTween?.Kill();
    }

    public void ResetImmediately()
    {
        _scaleTween?.Kill();

        if (target != null)
            target.localScale = _initialScale;
    }

    private void OnDisable()
    {
        _scaleTween?.Kill();
    }

    private void OnDestroy()
    {
        _scaleTween?.Kill();
    }
}