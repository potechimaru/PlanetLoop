using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// GameModeSelectのスロットを右に回転させるためのボタンのクラス。クリック時にスロットを右に回転させるアニメーションを再生する。
/// </summary>
public class RightSelectButton : UIButtonBase
{
    [Header("Hover Animation")]
    [SerializeField] private HoverScaleAnimation _hoverScaleAnimation;

    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Click Animation")]
    [SerializeField] private float clickScale = 0.94f;
    [SerializeField] private float clickDuration = 0.08f;
    [SerializeField] private Ease clickEase = Ease.OutQuad;
    [SerializeField] private Ease clickReturnEase = Ease.OutCubic;

    private Tween _clickTween;
    private Vector3 _initialScale;

    protected override void Awake()
    {
        base.Awake();

        if (target == null)
            target = GetComponent<RectTransform>();

        if (target != null)
            _initialScale = target.localScale;
    }

    protected override void HandleHoverEnter()
    {
        _hoverScaleAnimation?.PlayHoverEnter();
    }

    protected override void HandleHoverExit()
    {
        _hoverScaleAnimation?.PlayHoverExit();
    }

    protected override void HandleClick(PointerEventData eventData)
    {
        if (target == null) return;

        _hoverScaleAnimation?.Kill();
        _clickTween?.Kill();

        Vector3 hoverTargetScale = _hoverScaleAnimation != null
            ? _hoverScaleAnimation.GetHoverScale()
            : _initialScale;

        Vector3 returnScale = IsHoverActive ? hoverTargetScale : _initialScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOScale(_initialScale * clickScale, clickDuration).SetEase(clickEase));
        seq.Append(target.DOScale(returnScale, clickDuration).SetEase(clickReturnEase));

        _clickTween = seq;
    }

    public override void ResetImmediately()
    {
        base.ResetImmediately();

        _clickTween?.Kill();
        _hoverScaleAnimation?.ResetImmediately();

        if (target != null)
            target.localScale = _initialScale;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _clickTween?.Kill();
        _hoverScaleAnimation?.Kill();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _clickTween?.Kill();
        _hoverScaleAnimation?.Kill();
    }
}