using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Threading;

/// <summary>
/// 一文字ずつ落下して表示される「GameOver」テキストのアニメーションを制御するクラス
/// </summary>
public class GameOverTextAnimation : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private string _text = "GameOver";

    [Header("Letters")]
    [SerializeField] private List<TextMeshProUGUI> _letterTexts = new();

    [Header("Layout")]
    [SerializeField] private float _letterSpacing = 80f;

    [Header("Animation")]
    [SerializeField] private float _dropDistance = 120f;
    [SerializeField] private float _duration = 0.25f;
    [SerializeField] private float _interval = 0.08f;

    private readonly List<Vector2> _targetPositions = new();
    private readonly List<CanvasGroup> _canvasGroups = new();

    private Sequence _sequence;

    private void Awake()
    {
        SetupLetters();
    }

    /// <summary>
    /// ゲーム開始時に、各文字の初期位置とターゲット位置を設定する
    /// </summary>
    private void SetupLetters()
    {
        _targetPositions.Clear();
        _canvasGroups.Clear();

        int count = Mathf.Min(_text.Length, _letterTexts.Count);

        float totalWidth = (count - 1) * _letterSpacing;
        float startX = -totalWidth * 0.5f;

        for (int i = 0; i < _letterTexts.Count; i++)
        {
            var letter = _letterTexts[i];
            if (letter == null) continue;

            RectTransform rect = letter.rectTransform;

            Vector2 targetPos = new Vector2(
                startX + i * _letterSpacing,
                rect.anchoredPosition.y
            );

            rect.anchoredPosition = targetPos;

            _targetPositions.Add(targetPos);

            CanvasGroup canvasGroup = letter.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = letter.gameObject.AddComponent<CanvasGroup>();

            _canvasGroups.Add(canvasGroup);
        }
    }

    /// <summary>
    /// 落下アニメーションを再生する
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async UniTask PlayAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            SetupLetters();

            _sequence?.Kill();

            _sequence = DOTween.Sequence()
                .SetUpdate(true)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);

            int count = Mathf.Min(_text.Length, _letterTexts.Count);

            for (int i = 0; i < count; i++)
            {
                var letter = _letterTexts[i];
                if (letter == null) continue;

                RectTransform rect = letter.rectTransform;
                CanvasGroup canvasGroup = _canvasGroups[i];

                Vector2 targetPos = _targetPositions[i];
                Vector2 startPos = targetPos + Vector2.up * _dropDistance;

                rect.anchoredPosition = startPos;
                canvasGroup.alpha = 0f;

                float startTime = i * _interval;

                _sequence.Insert(
                    startTime,
                    rect.DOAnchorPos(targetPos, _duration)
                        .SetEase(Ease.Linear)
                );

                _sequence.Insert(
                    startTime,
                    canvasGroup.DOFade(1f, _duration)
                        .SetEase(Ease.Linear)
                );
            }

            await _sequence
                .AsyncWaitForCompletion()
                .AsUniTask()
                .AttachExternalCancellation(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _sequence?.Kill();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void ResetView()
    {
        SetupLetters();

        int count = Mathf.Min(_text.Length, _letterTexts.Count);

        for (int i = 0; i < count; i++)
        {
            var letter = _letterTexts[i];
            if (letter == null) continue;

            letter.rectTransform.anchoredPosition =
                _targetPositions[i] + Vector2.up * _dropDistance;

            _canvasGroups[i].alpha = 0f;
        }
    }

    /// <summary>
    /// アクティブな文字を変更する。_textの長さに応じて、_letterTextsの表示を切り替える
    /// </summary>
    public void ChangeActiveLetters()
    {
        int count = Mathf.Min(_text.Length, _letterTexts.Count);
        for (int i = 0; i < _letterTexts.Count; i++)
        {
            var letter = _letterTexts[i];
            if (letter == null) continue;
            if (i < _text.Length)
            {
                letter.text = _text[i].ToString();
                letter.gameObject.SetActive(true);
            }
            else
            {
                letter.text = "";
                letter.gameObject.SetActive(false);
            }
        }

    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }
}