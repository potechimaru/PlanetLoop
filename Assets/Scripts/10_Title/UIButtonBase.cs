using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

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

    private Tween _hoverStateTween;
    private bool _isPointerInside;
    private bool _isHoverActive;

    protected virtual void Awake()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerInside = true;
        ScheduleHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerInside = false;
        ScheduleHoverExit();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HandleClick(eventData);
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
    }
}