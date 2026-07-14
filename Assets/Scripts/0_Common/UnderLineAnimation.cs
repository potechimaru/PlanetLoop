using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// UIの装飾用のアンダーラインのアニメーションを制御するクラス
/// </summary>
public class UnderLineAnimation : MonoBehaviour
{
    [SerializeField] private Image _image;

    [Header("Animation")]
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Tween _tween;

    private void Awake()
    {
        if (_image == null)
            _image = GetComponent<Image>();
    }

    /// <summary>
    /// 再生
    /// </summary>
    public void Play()
    {
        if (_image == null) return;

        KillTween();

        _image.fillAmount = 0f;

        _tween = DOTween.To(
                () => _image != null ? _image.fillAmount : 0f,
                x =>
                {
                    if (_image == null) return;
                    _image.fillAmount = x;
                },
                1f,
                duration
            )
            .SetEase(ease)
            .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
    }

    public void ResetState()
    {
        KillTween();

        if (_image != null)
            _image.fillAmount = 0f;
    }

    private void KillTween()
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }

        _tween = null;
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