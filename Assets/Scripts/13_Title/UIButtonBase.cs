using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UniRx;
using System;

public abstract class UIButtonBase : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("Hover Stabilization")]
    [SerializeField, Min(0f)] private float enterDebounce = 0.08f;
    [SerializeField, Min(0f)] private float exitDebounce = 0.08f;

    protected bool IsPointerInside => _isPointerInside;
    protected bool IsHoverActive => _isHoverActive;

    public IObservable<Unit> OnClicked => _onClicked;

    private Tween _hoverStateTween;
    private bool _isPointerInside;
    private bool _isHoverActive;

    private bool _isButtonActive = true;

    public virtual bool IsButtonActive
    {
        get => _isButtonActive;
        set => _isButtonActive = value;
    }

    private readonly Subject<Unit> _onClicked = new();

    protected virtual void Awake()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsButtonActive) return;
        _isPointerInside = true;
        ScheduleHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsButtonActive) return;
        _isPointerInside = false;
        ScheduleHoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsButtonActive) return;
        HandleClick(eventData);
        _onClicked.OnNext(Unit.Default);
    }

    private void ScheduleHoverEnter()
    {
        _hoverStateTween?.Kill();

        _hoverStateTween = DOVirtual.DelayedCall(enterDebounce, () =>
        {
            if (!_isPointerInside)
                return;

            if (_isHoverActive)
                return;

            _isHoverActive = true;
            HandleHoverEnter();
        });
    }

    private void ScheduleHoverExit()
    {
        _hoverStateTween?.Kill();

        _hoverStateTween = DOVirtual.DelayedCall(exitDebounce, () =>
        {
            if (_isPointerInside)
                return;

            if (!_isHoverActive)
                return;

            _isHoverActive = false;
            HandleHoverExit();
        });
    }

    protected abstract void HandleHoverEnter();
    protected abstract void HandleHoverExit();
    protected abstract void HandleClick(PointerEventData eventData);

    public virtual void ResetImmediately()
    {
        _hoverStateTween?.Kill();
        _isPointerInside = false;
        _isHoverActive = false;
    }

    protected virtual void OnDisable()
    {
        _hoverStateTween?.Kill();
    }

    protected virtual void OnDestroy()
    {
        _hoverStateTween?.Kill();
        _onClicked.OnCompleted();
        _onClicked.Dispose();
    }
}