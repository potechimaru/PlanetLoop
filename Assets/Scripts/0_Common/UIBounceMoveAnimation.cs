using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// バうウドしながらUIを画面内に出入りさせるアニメーションを制御するクラス
/// </summary>
public class UIBounceMoveAnimation : MonoBehaviour
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Direction")]
    [SerializeField] private MoveDirection direction = MoveDirection.Up;

    [Header("Distance")]
    [SerializeField, Min(0f)] private float moveDistance = 1200f;
    [SerializeField, Min(0f)] private float preMoveDistance = 60f;

    [Header("Duration")]
    [SerializeField, Min(0.01f)] private float preMoveDuration = 0.12f;
    [SerializeField, Min(0.01f)] private float mainMoveDuration = 0.45f;

    [Header("Ease")]
    [SerializeField] private Ease preMoveEase = Ease.OutQuad;
    [SerializeField] private Ease mainMoveEase = Ease.InQuad;

    private Vector2 _initialAnchoredPos;
    private Tween _tween;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (target != null)
            _initialAnchoredPos = target.anchoredPosition;
    }

    /// <summary>
    /// 画面外に出るアニメーションを再生する
    /// </summary>
    public async UniTask PlayExitAsync(CancellationToken cancellationToken = default)
    {
        if (target == null)
            return;

        _tween?.Kill();

        Vector2 dir = GetDirectionVector(direction);
        Vector2 basePos = _initialAnchoredPos;
        Vector2 preMoveTarget = basePos - dir * preMoveDistance;
        Vector2 exitTarget = basePos + dir * moveDistance;

        target.anchoredPosition = basePos;

        var seq = DOTween.Sequence()
        .SetUpdate(true); 
        seq.Append(target.DOAnchorPos(preMoveTarget, preMoveDuration).SetEase(preMoveEase));
        seq.Append(target.DOAnchorPos(exitTarget, mainMoveDuration).SetEase(mainMoveEase));

        _tween = seq;

        try
        {
            using (cancellationToken.Register(() => _tween?.Kill()))
            {
                await _tween.AsyncWaitForCompletion();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(UIBounceMoveAnimation)} Exit Error: {ex}");
        }
    }

    /// <summary>
    /// 画面内に入るアニメーションを再生する
    /// </summary>
    public async UniTask PlayEnterAsync(CancellationToken cancellationToken = default)
    {
        if (target == null)
            return;

        _tween?.Kill();

        Vector2 dir = GetDirectionVector(direction);
        Vector2 basePos = _initialAnchoredPos;
        Vector2 enterStartPos = basePos + dir * moveDistance;
        Vector2 preEnterPos = basePos - dir * preMoveDistance;

        target.anchoredPosition = enterStartPos;

        var seq = DOTween.Sequence()
        .SetUpdate(true);
        seq.Append(target.DOAnchorPos(preEnterPos, mainMoveDuration).SetEase(mainMoveEase));
        seq.Append(target.DOAnchorPos(basePos, preMoveDuration).SetEase(preMoveEase));

        _tween = seq;

        try
        {
            using (cancellationToken.Register(() => _tween?.Kill()))
            {
                await _tween.AsyncWaitForCompletion();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(UIBounceMoveAnimation)} Enter Error: {ex}");
        }
    }

    public void SetToEnterStartPosition()
    {
        if (target == null)
            return;

        _tween?.Kill();

        Vector2 dir = GetDirectionVector(direction);
        Vector2 basePos = _initialAnchoredPos;
        Vector2 enterStartPos = basePos + dir * moveDistance;

        target.anchoredPosition = enterStartPos;
    }

    /// <summary>
    /// 出ていくアニメーションを同期的に再生する
    /// </summary>
    public void PlayExit()
    {
        PlayExitAsync().Forget();
    }

    /// <summary>
    ///入っていくアニメーションを同期的に再生する
    /// </summary>
    public void PlayEnter()
    {
        PlayEnterAsync().Forget();
    }

    public void ResetToInitialPosition()
    {
        _tween?.Kill();

        if (target == null)
            return;

        target.anchoredPosition = _initialAnchoredPos;
    }

    public void SetCurrentPositionAsInitial()
    {
        if (target == null)
            return;

        _initialAnchoredPos = target.anchoredPosition;
    }

    private Vector2 GetDirectionVector(MoveDirection moveDirection)
    {
        switch (moveDirection)
        {
            case MoveDirection.Up:
                return Vector2.up;
            case MoveDirection.Down:
                return Vector2.down;
            case MoveDirection.Left:
                return Vector2.left;
            case MoveDirection.Right:
                return Vector2.right;
            default:
                return Vector2.up;
        }
    }

    private void OnDisable()
    {
        _tween?.Kill();
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}