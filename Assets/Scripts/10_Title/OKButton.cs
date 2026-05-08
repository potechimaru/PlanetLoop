using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class OKButton : UIButtonBase
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Hover Animations")]
    [SerializeField] private HoverGlowAnimation _hoverGlow;
    [SerializeField] private HoverScaleAnimation _hoverScale;

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
        _hoverGlow?.PlayHoverEnter();
        _hoverScale?.PlayHoverEnter();
    }

    protected override void HandleHoverExit()
    {
        _hoverGlow?.PlayHoverExit();
        _hoverScale?.PlayHoverExit();
    }

    protected override void HandleClick(PointerEventData eventData)
    {
        if (target == null) return;

        _hoverScale?.Kill();
        _clickTween?.Kill();

        Vector3 hoverTargetScale = _hoverScale != null
            ? _hoverScale.GetHoverScale()
            : _initialScale;

        Vector3 returnScale = _initialScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOScale(_initialScale * clickScale, clickDuration).SetEase(clickEase));
        seq.Append(target.DOScale(returnScale, clickDuration).SetEase(clickReturnEase));

        _clickTween = seq;
    }

    public override void ResetImmediately()
    {
        base.ResetImmediately();

        _clickTween?.Kill();
        _hoverScale?.ResetImmediately();

        if (target != null)
            target.localScale = _initialScale;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _clickTween?.Kill();
        _hoverScale?.Kill();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _clickTween?.Kill();
        _hoverScale?.Kill();
    }
}