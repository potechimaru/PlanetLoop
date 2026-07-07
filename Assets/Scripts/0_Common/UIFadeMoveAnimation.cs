using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 移動しながらフェードも加えるアニメーションクラス
/// バウンスなし。
/// </summary>
public class UIFadeMoveAnimation : MonoBehaviour
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Target")]
    [SerializeField] private RectTransform _target;

    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Move")]
    [SerializeField] private MoveDirection _direction = MoveDirection.Up;

    [SerializeField] private float _moveDistance = 100f;

    [SerializeField] private float _duration = 0.4f;

    [SerializeField] private Ease _ease = Ease.OutCubic;

    [Header("Fade")]
    [SerializeField] private float _startAlpha = 0f;

    [SerializeField] private float _endAlpha = 1f;

    [Header("Option")]
    [SerializeField] private bool _useUnscaledTime = true;

    private Vector2 _initialPosition;

    private Sequence _sequence;

    private void Awake()
    {
        if (_target == null)
            _target = GetComponent<RectTransform>();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _initialPosition = _target.anchoredPosition;
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _sequence?.Kill();

            Vector2 direction = GetDirectionVector(_direction);

            Vector2 startPosition =
                _initialPosition - direction * _moveDistance;

            _target.anchoredPosition = startPosition;
            _canvasGroup.alpha = _startAlpha;

            _sequence = DOTween.Sequence()
                .SetUpdate(_useUnscaledTime)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            _sequence.Join(
                _target.DOAnchorPos(_initialPosition, _duration)
                    .SetEase(_ease)
            );

            _sequence.Join(
                _canvasGroup.DOFade(_endAlpha, _duration)
                    .SetEase(Ease.Linear)
            );

            await _sequence
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _sequence?.Kill();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void ResetView()
    {
        _sequence?.Kill();

        Vector2 direction = GetDirectionVector(_direction);

        _target.anchoredPosition =
            _initialPosition - direction * _moveDistance;

        _canvasGroup.alpha = _startAlpha;
    }

    private Vector2 GetDirectionVector(MoveDirection direction)
    {
        return direction switch
        {
            MoveDirection.Up => Vector2.up,
            MoveDirection.Down => Vector2.down,
            MoveDirection.Left => Vector2.left,
            MoveDirection.Right => Vector2.right,
            _ => Vector2.up
        };
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }
}