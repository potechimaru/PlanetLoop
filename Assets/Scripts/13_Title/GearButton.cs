using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

/// <summary>
/// 設定のギアアイコンを回転させるUIボタンのクラス。ユーザーがボタンにホバーしたときにギアが回転する。
/// 「Setting」のCanvasが表示される。
/// </summary>
public class GearButton : UIButtonBase
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Speed")]
    [SerializeField, Min(0.01f)] private float rotateSpeedDegPerSec = 360f;

    [Header("Animation")]
    [SerializeField] private Ease rotateEase = Ease.Linear;
    [SerializeField] private Ease returnEase = Ease.Linear;

    [Header("Description")]
    [SerializeField] private CanvasGroup description;
    [SerializeField] private float descriptionFadeDuration = 0.2f;

    [Header("Events")]
    [SerializeField] private UnityEvent onClick;

    private Tween _rotateTween;
    private Tween _fadeTween;

    private float _initialZ;
    private float _currentZ;

    protected override void Awake()
    {
        base.Awake();

        if (target == null)
            target = GetComponent<RectTransform>();

        if (target != null)
        {
            _initialZ = target.localEulerAngles.z;
            _currentZ = _initialZ;
            ApplyRotation(_currentZ);
        }

        if (description != null)
        {
            description.alpha = 0f;
            description.blocksRaycasts = false;
            description.interactable = false;
        }
    }

    protected override void HandleHoverEnter()
    {
        PlayRotate();
        FadeInDescription();
    }

    protected override void HandleHoverExit()
    {
        ReturnToInitialRotation();
        FadeOutDescription();
    }

    protected override void HandleClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    /// <summary>
    /// ギアアイコンを回転させるアニメーションを開始する
    /// </summary>
    private void PlayRotate()
    {
        if (target == null)
            return;

        _rotateTween?.Kill();

        float targetZ = _currentZ - 360f;
        float distance = Mathf.Abs(targetZ - _currentZ);
        float duration = distance / rotateSpeedDegPerSec;

        _rotateTween = DOTween.To(
                () => _currentZ,
                x =>
                {
                    _currentZ = x;
                    ApplyRotation(_currentZ);
                },
                targetZ,
                duration
            )
            .SetEase(rotateEase);
    }

    /// <summary>
    /// ホバーを解除したらギアアイコンを元の角度に戻すアニメーションを開始する
    /// </summary>
    private void ReturnToInitialRotation()
    {
        if (target == null)
            return;

        _rotateTween?.Kill();

        float distance = Mathf.Abs(_currentZ - _initialZ);
        if (distance <= 0.001f)
        {
            _currentZ = _initialZ;
            ApplyRotation(_currentZ);
            return;
        }

        float duration = distance / rotateSpeedDegPerSec;

        _rotateTween = DOTween.To(
                () => _currentZ,
                x =>
                {
                    _currentZ = x;
                    ApplyRotation(_currentZ);
                },
                _initialZ,
                duration
            )
            .SetEase(returnEase);
    }

    private void FadeInDescription()
    {
        if (description == null)
            return;

        _fadeTween?.Kill();
        _fadeTween = description.DOFade(1f, descriptionFadeDuration)
            .SetEase(Ease.OutCubic);
    }

    private void FadeOutDescription()
    {
        if (description == null)
            return;

        _fadeTween?.Kill();
        _fadeTween = description.DOFade(0f, descriptionFadeDuration)
            .SetEase(Ease.OutCubic);
    }

    private void ApplyRotation(float z)
    {
        if (target == null)
            return;

        target.localRotation = Quaternion.Euler(0f, 0f, z);
    }

    public override void ResetImmediately()
    {
        base.ResetImmediately();

        _rotateTween?.Kill();
        _fadeTween?.Kill();

        _currentZ = _initialZ;
        ApplyRotation(_currentZ);

        if (description != null)
        {
            description.alpha = 0f;
            description.blocksRaycasts = false;
            description.interactable = false;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _rotateTween?.Kill();
        _fadeTween?.Kill();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _rotateTween?.Kill();
        _fadeTween?.Kill();
    }
}