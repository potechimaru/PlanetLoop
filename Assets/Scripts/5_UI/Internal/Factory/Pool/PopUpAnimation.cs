using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class PopUpAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveDistance = 2.0f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;
    [SerializeField] private float _fadeDuration = 0.6f;

    [Header("Rotation")]
    [SerializeField] private bool useRotation = true;
    [SerializeField] private Vector3 rotateAmount = new Vector3(0f, 360f, 0f);
    [SerializeField] private Ease rotateEase = Ease.OutQuad;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Sequence _sequence;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        if (_rectTransform == null || _canvasGroup == null) return;

        _sequence?.Kill();
        _sequence = null;

        _canvasGroup.alpha = 1f;

        Vector2 startPos = _rectTransform.anchoredPosition;
        _rectTransform.localRotation = Quaternion.identity;

        _sequence = DOTween.Sequence()
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        if (useRotation)
        {
            _sequence.Join(
                _rectTransform
                    .DORotate(rotateAmount, duration, RotateMode.FastBeyond360)
                    .SetEase(rotateEase)
            );
        }

        _sequence.Join(
            _rectTransform
                .DOAnchorPosY(startPos.y + moveDistance * 100f, duration)
                .SetEase(moveEase)
        );

        _sequence.Append(
            _canvasGroup.DOFade(0f, _fadeDuration)
        );

        try
        {
            await _sequence.AsyncWaitForCompletion();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void OnDisable()
    {
        _sequence?.Kill();
        _sequence = null;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
        _sequence = null;
    }
}