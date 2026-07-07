using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// ポイント獲得時に表示されるポイントの概要を示すUIのアニメーションを制御するクラス
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class PopupTextAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform target;
    [SerializeField] private TMP_Text text;

    [Header("Animation")]
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease ease = Ease.OutBack; // ポップ感を出す
    [SerializeField] private float startScale = 0f;
    [SerializeField] private float endScale = 1f;

    private Tween _tween;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (text == null)
            text = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// ポップアップ表示（拡大しながら出現）
    /// </summary>
    public async UniTask PlayPopupAsync(CancellationToken ct = default)
    {
        _tween?.Kill();

        try
        {
            // 初期状態
            target.localScale = Vector3.one * startScale;

            if (text != null)
                text.alpha = 1f;

            target.gameObject.SetActive(true);

            _tween = target
                .DOScale(endScale, duration)
                .SetEase(ease)
                .SetUpdate(true); // 時間停止無効（強い）（うん）

            using (ct.Register(() => _tween?.Kill()))
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
            Debug.LogError($"[PopupTextAnimation] Error: {e}");
            _tween?.Kill();
        }
    }

    /// <summary>
    /// 初期状態に戻す
    /// </summary>
    public void ResetState()
    {
        _tween?.Kill();
        target.localScale = Vector3.one * startScale;

        if (text != null)
            text.alpha = 0f;
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }
}