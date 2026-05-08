using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public void Play()
    {
        _tween?.Kill();

        _image.fillAmount = 0f;

        _tween = DOTween.To(
            () => _image.fillAmount,
            x => _image.fillAmount = x,
            1f,
            duration
        ).SetEase(ease);
    }

    public void ResetState()
    {
        _tween?.Kill();
        _image.fillAmount = 0f;
    }

    public void OnDestroy()
    {
        _tween?.Kill();
    }
}
