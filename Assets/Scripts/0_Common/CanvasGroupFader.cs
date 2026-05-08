using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private float defaultFadeInDuration = 0.5f;
    [SerializeField] private float defaultFadeOutDuration = 0.5f;
    [SerializeField] private Ease fadeEase = Ease.OutCubic;

    private CanvasGroup _canvasGroup;
    private Sequence _sequence;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            Debug.LogError($"{nameof(CanvasGroupFader)}: CanvasGroup が見つかりません");
        }
    }

    // =========================
    // Fade In
    // =========================
    public async UniTask FadeInAsync(
        float? duration = null,
        CancellationToken cancellationToken = default)
    {
        float d = duration ?? defaultFadeInDuration;

        _canvasGroup.alpha = 0f;

        try
        {
            KillSequence();

            _sequence = DOTween.Sequence();

            _sequence.Append(
                _canvasGroup
                    .DOFade(1f, d)
                    .SetEase(fadeEase)
            );

            await AwaitSequence(_sequence, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"FadeInAsync Error: {ex}");
        }
    }

    // =========================
    // Fade Out
    // =========================
    public async UniTask FadeOutAsync(
        float? duration = null,
        CancellationToken cancellationToken = default)
    {
        float d = duration ?? defaultFadeOutDuration;

        _canvasGroup.alpha = 1f;

        try
        {
            KillSequence();

            _sequence = DOTween.Sequence();

            _sequence.Append(
                _canvasGroup
                    .DOFade(0f, d)
                    .SetEase(fadeEase)
            );

            await AwaitSequence(_sequence, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"FadeOutAsync Error: {ex}");
        }
    }

    // =========================
    // Fade To
    // =========================
    public async UniTask FadeToAsync(
    float targetAlpha,
    float duration,
    CancellationToken cancellationToken = default)
    {
        try
        {
            KillSequence();

            _sequence = DOTween.Sequence()
                .SetUpdate(true); // timeScale = 0 でも動く

            _sequence.Append(
                _canvasGroup
                    .DOFade(targetAlpha, duration)
                    .SetEase(fadeEase)
            );

            await AwaitSequence(_sequence, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"FadeToAsync Error: {ex}");
        }
    }

    public void ResetAlpha()
    {
        KillSequence();
        SetAlpha(0f);
    }

    public void FullAlpha()
    {
        KillSequence();
        SetAlpha(1f);
    }

    // =========================
    // 即時反映
    // =========================
    private void SetAlpha(float alpha)
    {
        KillSequence();
        _canvasGroup.alpha = alpha;
    }

    // =========================
    // Sequence待機（重要）
    // =========================
    private async UniTask AwaitSequence(Sequence seq, CancellationToken ct)
    {
        var tcs = new UniTaskCompletionSource();

        seq.OnComplete(() => tcs.TrySetResult());
        seq.OnKill(() => tcs.TrySetCanceled());


        using (ct.Register(() =>
        {
            if (seq.IsActive())
            {
                seq.Kill();
            }
        }))
        {
            await tcs.Task;
        }
    }

    // =========================
    // Kill
    // =========================
    private void KillSequence()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
            _sequence = null;
        }
    }

    private void OnDestroy()
    {
        KillSequence();
    }
}