using UnityEngine;
using DG.Tweening;

/// <summary>
/// GameModeSelectの時、Splineを上下に浮かせるアニメーションをループさせるクラス
/// </summary>
public class FloatLoopAnimation : MonoBehaviour
{
    [SerializeField] private float floatStrength = 0.3f;       // 上下に移動する距離
    [SerializeField] private float floatDuration = 1.5f;       // １往復にかかる時間

    private Tween moveTween;

    private Vector3 _initialPosition;

    private void Awake()
    {
        _initialPosition = transform.position;
    }

    public void Play()
    {
        _initialPosition = transform.position;
        Stop();

        // 上下移動
        moveTween = transform.DOMoveY(_initialPosition.y + floatStrength, floatDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);

    }

    /// <summary>
    /// アニメーション停止＆元の位置へ戻す
    /// </summary>
    public void Stop()
    {
        moveTween?.Kill();

        moveTween = null;

        transform.position = _initialPosition;
    }

    private void OnDestroy()
    {
        moveTween?.Kill();
    }

    private void OnDisable()
    {
        moveTween?.Kill();
    }
}