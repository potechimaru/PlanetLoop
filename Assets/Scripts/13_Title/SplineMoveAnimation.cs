using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class SplineMoveAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform splineTransform;

    [Header("Destination")]
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 targetRotationEuler;

    [Header("Animation")]
    [SerializeField, Min(0.01f)] private float duration = 1.0f;
    [SerializeField] private Ease ease = Ease.InOutCubic;

    [Header("Options")]
    [SerializeField] private bool movePosition = true;
    [SerializeField] private bool rotate = true;

    private Tween _moveTween;
    private Tween _rotateTween;

    private void Awake()
    {
        if (splineTransform == null)
            splineTransform = transform;
    }

    public async UniTask PlayAsync(CancellationToken token = default)
    {
        if (splineTransform == null)
            return;

        _moveTween?.Kill();
        _rotateTween?.Kill();

        Sequence seq = DOTween.Sequence();

        if (movePosition)
        {
            var move = splineTransform.DOMove(targetPosition, duration).SetEase(ease);
            _moveTween = move;
            seq.Join(move);
        }

        if (rotate)
        {
            var rot = splineTransform.DORotate(targetRotationEuler, duration)
                .SetEase(ease);

            _rotateTween = rot;
            seq.Join(rot);
        }

        try
        {
            using (token.Register(() => seq.Kill()))
            {
                await seq.AsyncWaitForCompletion();
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("SplineMoveAnimation canceled");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SplineMoveAnimation Error: {ex}");
        }
    }

    private void OnDisable()
    {
        _moveTween?.Kill();
        _rotateTween?.Kill();
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
        _rotateTween?.Kill();
    }
}