using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// 画面遷移した際に、UIが下から上にスライドして表示されるアニメーションを制御するクラス
/// </summary>
public class TextRiseAnimation : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private RectTransform text;

    [Header("Animation")]
    [SerializeField] private float moveDistance = 80f;
    [SerializeField] private float duration = 0.6f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    [Header("Delay")]
    [SerializeField] private float delay = 0f;

    private Vector2 _initPos;
    private Sequence _sequence;

    private void Awake()
    {
        if (text == null)
            text = GetComponent<RectTransform>();

        if (text == null)
        {
            Debug.LogError($"{nameof(TextRiseAnimation)}: RectTransform が見つかりません。");
            return;
        }

        _initPos = text.anchoredPosition;
    }

    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        if (text == null)
        {
            Debug.LogError($"{nameof(TextRiseAnimation)}: RectTransform が未設定です。");
            return;
        }

        try
        {
            KillSequence();

            // 初期位置（下に隠す）
            text.anchoredPosition = _initPos - new Vector2(0, moveDistance);

            _sequence = DOTween.Sequence();

            if (delay > 0f)
            {
                _sequence.AppendInterval(delay);
            }

            _sequence.Append(
                text.DOAnchorPosY(_initPos.y, duration)
                    .SetEase(ease)
            );

            await AwaitSequence(_sequence, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // キャンセルは正常系として扱う
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(TextRiseAnimation)} PlayAsync Error: {ex}");
        }
    }

    public void ResetState()
    {
        KillSequence();

        if (text != null)
        {
            text.anchoredPosition = _initPos - new Vector2(0, moveDistance);
        }
    }

    private async UniTask AwaitSequence(Sequence seq, CancellationToken ct)
    {
        var tcs = new UniTaskCompletionSource();

        seq.OnComplete(() => tcs.TrySetResult());
        seq.OnKill(() => tcs.TrySetCanceled());

        using (ct.Register(() =>
        {
            if (seq != null && seq.IsActive())
            {
                seq.Kill();
            }
        }))
        {
            await tcs.Task;
        }
    }

    private void KillSequence()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
            _sequence = null;
        }
    }

    private void OnDestroy()
    {
        KillSequence();
    }
}