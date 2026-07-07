using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// GameOpening時の帯のHeightを制御するクラス
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class HeightChangeAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private float _initialHeight;
    private Tween _tween;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        _initialHeight = target.sizeDelta.y;
    }

    /// <summary>
    /// 0 → 初期高さ
    /// </summary>
    public async UniTask PlayOpenAsync(CancellationToken cancellationToken = default)
    {
        _tween?.Kill();

        try
        {
            SetHeight(0f);

            _tween = DOTween.To(
                () => target.sizeDelta.y,
                y => SetHeight(y),
                _initialHeight,
                duration
            ).SetEase(ease)
            .SetUpdate(true);

            // キャンセル連動
            using (cancellationToken.Register(() => _tween?.Kill()))
            {
                await _tween.AsyncWaitForCompletion();
            }
        }
        catch (OperationCanceledException)
        {
            _tween?.Kill();
        }
        catch (Exception e)
        {
            Debug.LogError($"[HeightChangeAnimation] Open Error: {e}");
            _tween?.Kill();
        }
    }

    /// <summary>
    /// 初期高さ → 0
    /// </summary>
    public async UniTask PlayCloseAsync(CancellationToken cancellationToken = default)
    {
        _tween?.Kill();

        try
        {
            _tween = DOTween.To(
                () => target.sizeDelta.y,
                y => SetHeight(y),
                0f,
                duration
            ).SetEase(ease)
            .SetUpdate(true);

            using (cancellationToken.Register(() => _tween?.Kill()))
            {
                await _tween.AsyncWaitForCompletion();
            }
        }
        catch (OperationCanceledException)
        {
            _tween?.Kill();
        }
        catch (Exception e)
        {
            Debug.LogError($"[HeightChangeAnimation] Close Error: {e}");
            _tween?.Kill();
        }
    }

    private void SetHeight(float y)
    {
        var size = target.sizeDelta;
        size.y = y;
        target.sizeDelta = size;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}