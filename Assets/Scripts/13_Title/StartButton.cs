using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// タイトル画面のボタン。押すとGameModeSelectに入る。
/// </summary>
public class StartButton : UIButtonBase
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Hover Animation")]
    //[SerializeField] private float hoverScale = 1.08f;
    //[SerializeField] private float hoverDuration = 0.15f;
    //[SerializeField] private Ease hoverEase = Ease.OutCubic;
    //[SerializeField] private Ease returnEase = Ease.OutCubic;
    [SerializeField] private HoverGlowAnimation _hoverGlow;

    [Header("Click Animation")]
    [SerializeField] private float clickScale = 0.94f;
    [SerializeField] private float clickDuration = 0.08f;
    [SerializeField] private Ease clickEase = Ease.OutQuad;

    [SerializeField] private SplineBurstEmitter _splineBurstEmitter;
    [SerializeField] private ClosedSplineLine _splineLine;

    private Tween _scaleTween;
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
    }

    protected override void HandleHoverExit()
    {
        _hoverGlow?.PlayHoverExit();
    }

    protected override void HandleClick(PointerEventData eventData)
    {
        // 必要なら有効化
        // if (target != null)
        // {
        //     _scaleTween?.Kill();
        //
        //     Sequence seq = DOTween.Sequence();
        //     seq.Append(target.DOScale(_initialScale * clickScale, clickDuration).SetEase(clickEase));
        //     seq.Append(target.DOScale(IsHoverActive ? _initialScale * hoverScale : _initialScale, clickDuration).SetEase(hoverEase));
        //
        //     _scaleTween = seq;
        // }

        _splineBurstEmitter?.ScatterGlobal();
        _splineLine?.FlashLandingMaterial();
    }

    public override void ResetImmediately()
    {
        base.ResetImmediately();

        _scaleTween?.Kill();

        if (target != null)
            target.localScale = _initialScale;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _scaleTween?.Kill();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _scaleTween?.Kill();
    }
}