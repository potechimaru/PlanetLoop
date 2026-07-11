using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// ボタンのホバー時にGlowするアニメーションを制御するクラス
/// </summary>
public class HoverGlowAnimation : MonoBehaviour
{
    [Header("Target Image (ShaderGraph Material)")]
    [SerializeField] private Image targetImage;

    [Header("Shader Property Name")]
    [SerializeField] private string alphaPropertyName = "_Alpha";

    [Header("Animation")]
    [SerializeField] private float hoverAlpha = 1.0f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Material _materialInstance;
    private Tween _tween;

    private float _currentAlpha;

    private void Awake()
    {
        if (targetImage == null)
        {
            Debug.LogWarning("HoverGlowAnimation: targetImage is null");
            return;
        }

        // マテリアルをインスタンス化
        _materialInstance = Instantiate(targetImage.material);
        targetImage.material = _materialInstance;

        // 初期は透明
        SetAlphaImmediate(0f);
    }

    public void PlayHoverEnter()
    {
        PlayTween(hoverAlpha);
    }

    public void PlayHoverExit()
    {
        PlayTween(0f);
    }

    private void PlayTween(float target)
    {
        _tween?.Kill();

        _tween = DOTween.To(
                () => _currentAlpha,
                value =>
                {
                    _currentAlpha = value;
                    _materialInstance.SetFloat(alphaPropertyName, _currentAlpha);
                },
                target,
                duration
            )
            .SetEase(ease);
    }

    private void SetAlphaImmediate(float value)
    {
        _currentAlpha = value;
        _materialInstance.SetFloat(alphaPropertyName, value);
    }

    private void OnDisable()
    {
        _tween?.Kill();
    }

    private void OnDestroy()
    {
        _tween?.Kill();

        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
            _materialInstance = null;
        }
    }
}