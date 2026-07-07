using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ゲーム説明画面を開閉する際のアニメーションを制御するクラス
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
            Debug.LogError("[TutorialPanelAnimation] panel が未設定です。", this);
            return false;
        }

        _defaultScale = panel.localScale;
        _initialized = true;
        return true;
    }

    public async UniTask OpenAsync()
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

            //Debug.Log($"[TutorialPanelAnimation] OpenAsync: Starting tween with duration {openDuration} and overshoot {openOvershoot}", this);
            await _currentTween.AsyncWaitForCompletion();
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

    public async UniTask CloseAsync()
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

            await _currentTween.AsyncWaitForCompletion();

            gameObject.SetActive(false);
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