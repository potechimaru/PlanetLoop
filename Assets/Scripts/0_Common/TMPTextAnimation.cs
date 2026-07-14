using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using System;
using System.Threading;

/// <summary>
/// モード選択画面にて説明文を一文字ずつ表示するアニメーションクラス
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPDoTextAnimation : MonoBehaviour
{
    private string _targetText = "null";

    [Header("Animation Settings")]
    [SerializeField, Min(0.01f)]
    private float _charactersPerSecond = 20f;

    [SerializeField, Min(0f)]
    private float _startDelay = 0f;

    private TextMeshProUGUI _tmpText;
    private Sequence _sequence;

    private void Awake()
    {
        _tmpText = GetComponent<TextMeshProUGUI>();

        if (_tmpText == null)
        {
            Debug.LogError($"{nameof(TMPDoTextAnimation)}: TextMeshProUGUI が見つかりません。");
        }
    }

    /// <summary>
    /// アニメーション開始
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        if (_tmpText == null)
        {
            Debug.LogError($"{nameof(TMPDoTextAnimation)}: TextMeshProUGUI が未取得です。");
            return;
        }

        if (_charactersPerSecond <= 0f)
        {
            Debug.LogError($"{nameof(TMPDoTextAnimation)}: Characters Per Second は 0 より大きい値にしてください。");
            return;
        }

        try
        {
            KillSequence();

            _tmpText.text = _targetText ?? string.Empty;
            _tmpText.ForceMeshUpdate();

            int totalCharacters = _tmpText.textInfo.characterCount;
            _tmpText.maxVisibleCharacters = 0;

            float duration = totalCharacters / _charactersPerSecond;

            _sequence = DOTween.Sequence();

            if (_startDelay > 0f)
            {
                _sequence.AppendInterval(_startDelay);
            }

            _sequence.Append(
                DOTween.To(
                    () => _tmpText.maxVisibleCharacters,
                    x => _tmpText.maxVisibleCharacters = x,
                    totalCharacters,
                    duration)
                .SetEase(Ease.Linear)
            );

            await AwaitSequence(_sequence, cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Debug.LogError($"{nameof(TMPDoTextAnimation)} PlayAsync Error: {ex}");
        }
    }

    public void SetTextImmediately()
    {
        if (_tmpText == null)
        {
            Debug.LogError($"{nameof(TMPDoTextAnimation)}: TextMeshProUGUI が未取得です。");
            return;
        }

        KillSequence();

        _tmpText.text = _targetText ?? string.Empty;
        _tmpText.ForceMeshUpdate();
        _tmpText.maxVisibleCharacters = _tmpText.textInfo.characterCount;
    }

    public void SetTargetText(string text)
    {
        _targetText = text;
    }

    public void Stop()
    {
        KillSequence();
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