using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CameraApproachAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Vector3 target;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float duration = 1.0f;

    [Header("Stop Distance")]
    [SerializeField, Min(0f)] private float stopDistance = 2.0f;

    [Header("Ease")]
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Tween _tween;

    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        if (target == null)
        {
            Debug.LogError($"{nameof(CameraApproachAnimation)}: target Ç™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒÅB", this);
            return;
        }

        if (duration <= 0f)
        {
            Debug.LogWarning($"{nameof(CameraApproachAnimation)}: duration Ç™ 0 à»â∫Ç»ÇÃÇ≈ë¶éûà⁄ìÆÇµÇ‹Ç∑ÅB", this);
            MoveImmediately();
            return;
        }

        try
        {
            _tween?.Kill();

            Vector3 destination = CalculateDestination();

            _tween = transform
                .DOMove(destination, duration)
                .SetEase(ease)
                .SetLink(gameObject);

            await _tween
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _tween?.Kill();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
            _tween?.Kill();
        }
    }

    private Vector3 CalculateDestination()
    {
        Vector3 cameraPos = transform.position;
        Vector3 targetPos = target;

        Vector3 directionFromTargetToCamera = cameraPos - targetPos;

        if (directionFromTargetToCamera.sqrMagnitude <= Mathf.Epsilon)
        {
            directionFromTargetToCamera = -transform.forward;
        }

        Vector3 normalizedDirection = directionFromTargetToCamera.normalized;

        return targetPos + normalizedDirection * stopDistance;
    }

    private void MoveImmediately()
    {
        _tween?.Kill();
        transform.position = CalculateDestination();
    }

    public void Kill()
    {
        _tween?.Kill();
        _tween = null;
    }

    private void OnDisable()
    {
        Kill();
    }

    private void OnDestroy()
    {
        Kill();
    }
}