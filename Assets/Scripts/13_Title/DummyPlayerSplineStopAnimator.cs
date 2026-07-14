using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// SelectGameModeの時にDummyPlayerを左右に動かすためのアニメーション
/// </summary>
public class DummyPlayerSplineStopAnimator : MonoBehaviour
{
    // 右、左
    public enum MoveDirection
    {
        Forward,
        Backward
    }

    [Header("Refs")]
    [SerializeField] private Transform target;

    [Header("Animation")]
    [SerializeField, Min(0.01f)] private float duration = 1.0f;
    [SerializeField] private Ease ease = Ease.InOutSine;

    [Header("Options")]
    [SerializeField] private bool lookForward = true;

    private Tween _tween;
    private ClosedSplineLine _spline;
    private float _totalLength;

    private void Awake()
    {
        if (target == null)
            target = transform;
    }

    public void Initialize(ClosedSplineLine spline)
    {
        _spline = spline;
        _totalLength = _spline != null ? _spline.GetTotalLength() : 0f;
    }

    public void SetPositionImmediate(float distance)
    {
        if (_spline == null)
            return;

        if (_totalLength <= 1e-6f)
            _totalLength = _spline.GetTotalLength();

        ApplyPosition(distance);
    }

    public async UniTask PlayToDistanceAsync(
        float targetDistance,
        MoveDirection moveDirection,
        CancellationToken cancellationToken = default)
    {
        if (_spline == null || target == null)
            return;

        _totalLength = _spline.GetTotalLength();
        if (_totalLength <= 1e-6f)
        {
            Debug.LogWarning($"{nameof(DummyPlayerSplineStopAnimator)}: spline total length is 0.");
            return;
        }

        KillTween();

        float currentDistance = _spline.FindNearestDistance(target.position);
        float normalizedTargetDistance = Mathf.Repeat(targetDistance, _totalLength);

        float endDistance;

        if (moveDirection == MoveDirection.Forward)
        {
            float diff = normalizedTargetDistance - currentDistance;
            if (diff < 0f)
            {
                diff += _totalLength;
            }

            endDistance = currentDistance + diff;
        }
        else
        {
            float diff = currentDistance - normalizedTargetDistance;
            if (diff < 0f)
            {
                diff += _totalLength;
            }

            endDistance = currentDistance - diff;
        }

        _tween = DOVirtual.Float(
                currentDistance,
                endDistance,
                duration,
                d => ApplyPosition(d))
            .SetEase(ease);

        try
        {
            await AwaitTween(_tween, cancellationToken);
            ApplyPosition(normalizedTargetDistance);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(DummyPlayerSplineStopAnimator)} Error: {ex}");
        }
    }

    public void Stop()
    {
        KillTween();
    }

    private void ApplyPosition(float distance)
    {
        if (_spline == null || target == null)
            return;

        Vector3 pos = _spline.EvaluateByDistance(distance);
        target.position = pos;

        if (lookForward)
        {
            UpdateRotation(distance);
        }
    }

    private void UpdateRotation(float distance)
    {
        float delta = Mathf.Max(0.001f, _totalLength * 0.01f);

        Vector3 p1 = _spline.EvaluateByDistance(distance);
        Vector3 p2 = _spline.EvaluateByDistance(distance + delta);

        Vector3 dir = (p2 - p1).normalized;

        if (dir.sqrMagnitude > 0.0001f)
        {
            target.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }
    }

    private async UniTask AwaitTween(Tween tween, CancellationToken ct)
    {
        var tcs = new UniTaskCompletionSource();

        tween.OnComplete(() => tcs.TrySetResult());
        tween.OnKill(() => tcs.TrySetCanceled());

        using (ct.Register(() =>
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }))
        {
            await tcs.Task;
        }
    }

    private void KillTween()
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
            _tween = null;
        }
    }

    private void OnDisable()
    {
        KillTween();
    }

    private void OnDestroy()
    {
        KillTween();
    }
}