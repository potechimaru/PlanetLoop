using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 指定のCanvasGroupのフェードイン・フェードアウトを制御するクラス
/// </summary>
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
            Debug.LogError($"{nameof(CanvasGroupFader)}: CanvasGroup が見つかりません");
        }
    }

    // フェードイン
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

    // フェードアウト
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

    /// <summary>
    /// カスタム性能のあるフェード処理を行う
    /// </summary>
    /// <param name="targetAlpha">目標の透明度</param>
    /// <param name="duration">遷移時間</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    // 瞬時に透明にする
    public void ResetAlpha()
    {
        SetAlpha(0f);
    }

    // 瞬時に不透明にする
    public void FullAlpha()
    {
        SetAlpha(1f);
    }

    // 指定の透明度に瞬時に設定する
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