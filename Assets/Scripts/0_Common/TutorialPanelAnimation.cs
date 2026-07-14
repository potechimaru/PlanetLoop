using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// チュートリアルのパネルの開閉アニメーションを制御するクラス（設定の開閉にも使っている）
/// </summary>
public class TutorialPanelAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform panel;

    [Header("Open")]
    [SerializeField, Min(0f)] private float openDuration = 0.4f;
    [SerializeField] private float openOvershoot = 1.5f;

    [Header("Close")]
    [SerializeField, Min(0f)] private float closeDuration = 0.25f;

    private Vector3 _defaultScale;
    private Tween _currentTween;
    private bool _initialized;

    private void Awake()
    {
        Initialize();
    }

    private bool Initialize()
    {
        if (_initialized)
            return true;

        if (panel == null)
            panel = GetComponent<RectTransform>();

        if (panel == null)
        {
            Debug.LogError("[TutorialPanelAnimation] panel ???????ｳ??B", this);
            return false;
        }

        _defaultScale = panel.localScale;
        _initialized = true;
        return true;
    }

    /// <summary>
    /// パネルを開く
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask OpenAsync(CancellationToken cancellationToken = default)
    {
        if (!Initialize())
            return;

        try
        {
            _currentTween?.Kill();
            _currentTween = null;

            gameObject.SetActive(true);
            panel.localScale = Vector3.zero;

            _currentTween = panel
                .DOScale(_defaultScale, openDuration)
                .SetEase(Ease.OutBack, openOvershoot)
                .SetUpdate(true)
                .SetLink(gameObject);

            using (cancellationToken.Register(() => _currentTween?.Kill()))
            {
                await _currentTween.AsyncWaitForCompletion();
            }
        }
        catch (OperationCanceledException)
        {
            _currentTween?.Kill();
        }
        catch (Exception e)
        {
            Debug.LogError($"[TutorialPanelAnimation] OpenAsync Error: {e}", this);
        }
        finally
        {
            _currentTween = null;
        }
    }

    /// <summary>
    /// パネルを閉じる
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask CloseAsync(CancellationToken cancellationToken = default)
    {
        if (!Initialize())
            return;

        try
        {
            _currentTween?.Kill();
            _currentTween = null;

            if (!gameObject.activeSelf)
                return;

            _currentTween = panel
                .DOScale(Vector3.zero, closeDuration)
                .SetEase(Ease.InBack)
                .SetUpdate(true)
                .SetLink(gameObject);

            using (cancellationToken.Register(() => _currentTween?.Kill()))
            {
                await _currentTween.AsyncWaitForCompletion();
            }

            gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
            _currentTween?.Kill();
        }
        catch (Exception e)
        {
            Debug.LogError($"[TutorialPanelAnimation] CloseAsync Error: {e}", this);
        }
        finally
        {
            _currentTween = null;
        }
    }

    private void OnDestroy()
    {
        _currentTween?.Kill();
        _currentTween = null;
    }
}