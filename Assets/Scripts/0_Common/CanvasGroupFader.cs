using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupFader : MonoBehaviour
{
    [Header("Default Settings")]
    [SerializeField] private float defaultFadeInDuration = 0.5f;
    [SerializeField] private float defaultFadeOutDuration = 0.5f;
    [SerializeField] private Ease fadeEase = Ease.OutCubic;
    [SerializeField] private bool useUnscaledTime = true;

    private CanvasGroup _canvasGroup;
    private Tween _tween;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            Debug.LogError($"{nameof(CanvasGroupFader)}: CanvasGroup ‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ");
        }
    }

    public async UniTask FadeInAsync(
        float? duration = null,
        CancellationToken cancellationToken = default)
    {
        float d = duration ?? defaultFadeInDuration;

        try
        {
            KillTween();

            _canvasGroup.alpha = 0f;

            _tween = _canvasGroup
                .DOFade(1f, d)
                .SetEase(fadeEase)
                .SetUpdate(useUnscaledTime);

            await _tween
                .AsyncWaitForCompletion();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"FadeInAsync Error: {ex}");
        }
        finally
        {
            _tween = null;
        }
    }

    public async UniTask FadeOutAsync(
        float? duration = null,
        CancellationToken cancellationToken = default)
    {
        float d = duration ?? defaultFadeOutDuration;

        try
        {
            KillTween();

            _canvasGroup.alpha = 1f;

            _tween = _canvasGroup
                .DOFade(0f, d)
                .SetEase(fadeEase)
                .SetUpdate(useUnscaledTime);

            await _tween
                .AsyncWaitForCompletion();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"FadeOutAsync Error: {ex}");
        }
        finally
        {
            _tween = null;
        }
    }

    public async UniTask FadeToAsync(
        float targetAlpha,
        float duration,
        CancellationToken cancellationToken = default)
    {
        try
        {
            KillTween();

            _tween = _canvasGroup
                .DOFade(targetAlpha, duration)
                .SetEase(fadeEase)
                .SetUpdate(useUnscaledTime);

            await _tween
                .AsyncWaitForCompletion();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"FadeToAsync Error: {ex}");
        }
        finally
        {
            _tween = null;
        }
    }

    public void ResetAlpha()
    {
        SetAlpha(0f);
    }

    public void FullAlpha()
    {
        SetAlpha(1f);
    }

    public void SetAlpha(float alpha)
    {
        KillTween();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = alpha;
    }

    public void Kill()
    {
        KillTween();
    }

    private void KillTween()
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }

        _tween = null;
    }

    private void OnDestroy()
    {
        KillTween();
    }
}