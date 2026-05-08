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

        var startPos = _rectTransform.anchoredPosition;

        _sequence = DOTween.Sequence()
            .SetLink(gameObject, LinkBehaviour.KillOnDisable);

        _sequence.Join(
            _rectTransform
                .DORotate(new Vector3(0f, 360f, 0f), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
        );

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
            // ƒLƒƒƒ“ƒZƒ‹Žž‚Í–³Ž‹
        }
        catch (Exception ex)
        {
            if (_sequence == null || !_sequence.IsActive())
                return;

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