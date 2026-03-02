using UnityEngine;
using DG.Tweening;
using TMPro;

public class PopUpAnimation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveDistance = 2.0f;
    [SerializeField] private float duration = 1.2f;
    [SerializeField] private Ease moveEase = Ease.OutCubic;
    [SerializeField] private float _fadeDuration = 0.6f;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private Sequence _sequence;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void Play()
    {
        _canvasGroup.alpha = 1f;

        _sequence = DOTween.Sequence();

        // Y軸を一周回転
        _sequence.Join(
            _rectTransform
                .DORotate(new Vector3(0f, 360f, 0f), duration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuad)
        );

        // 上方向へ移動
        _sequence.Join(
            _rectTransform
                .DOAnchorPosY(_rectTransform.anchoredPosition.y + moveDistance * 100f, duration)
                .SetEase(moveEase)
        );

        // フェードアウト
        _sequence.Append(
            _canvasGroup
                .DOFade(0f, _fadeDuration)
        );

        _sequence.OnComplete(() =>
        {
            // プールに戻したいが一旦はSetActive(false)で
            gameObject.SetActive(false);
        });
    }

    public void OnDestroy()
    {
        _sequence?.Kill();
    }
}