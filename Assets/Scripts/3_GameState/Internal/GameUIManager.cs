using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using VContainer;

public class GameUIManager : MonoBehaviour
{
    [Header("GameOpening")]
    [SerializeField] private VignetteIntensityAnimation _gameOpeningVignetteAnimation;
    [SerializeField] private CanvasGroupFader _gameOpeningCanvasGroupFade;
    [SerializeField] private HeightChangeAnimation _gameOpeningHeightChangeAnimation;

    [Header("PreGame")]
    [SerializeField] private CanvasGroup _HUDCanvasGroup;
    [SerializeField] private PopupTextAnimation _preGamePopupTextAnimation;
    [SerializeField] private List<UIBounceMoveAnimation> _preGameHUDAnimation;

    [Header("ButtonSubscription")]
    [SerializeField] private ClickInputPublisher _clickInputPublisher;

    [Inject] private IGameStateChangeRequester _gameStateChangeRequester;

    private CompositeDisposable _disposables = new();

    private CancellationToken _destroyCancellationToken;

    private void Awake()
    {
        _destroyCancellationToken = this.GetCancellationTokenOnDestroy();

        _HUDCanvasGroup.alpha = 0f;
        _HUDCanvasGroup.interactable = false;
        _HUDCanvasGroup.blocksRaycasts = false;
    }

    public async UniTask GameOpening()
    {
        var ct = _destroyCancellationToken;

        if (_gameOpeningVignetteAnimation == null)
            return;

        if (_gameOpeningHeightChangeAnimation == null)
            return;

        if (_gameOpeningCanvasGroupFade == null)
            return;

        try
        {
            await _gameOpeningVignetteAnimation.PlayAsync(ct);

            ct.ThrowIfCancellationRequested();

            //_gameOpeningCanvasGroupFade.gameObject.SetActive(true);

            //await _gameOpeningCanvasGroupFade.FadeToAsync(0.9f, 0.3f);

            await UniTask.Delay(
                500,
                delayType: DelayType.UnscaledDeltaTime,
                cancellationToken: ct
            );

            ct.ThrowIfCancellationRequested();

            _gameOpeningHeightChangeAnimation.gameObject.SetActive(true);

            await _gameOpeningHeightChangeAnimation.PlayOpenAsync(ct);

            await UniTask.Delay(
                3000,
                delayType: DelayType.UnscaledDeltaTime,
                cancellationToken: ct
            );

            await _gameOpeningHeightChangeAnimation.PlayCloseAsync(ct);

            await UniTask.Delay(
                500,
                delayType: DelayType.UnscaledDeltaTime,
                cancellationToken: ct
            );

            if (_gameOpeningHeightChangeAnimation != null)
                _gameOpeningHeightChangeAnimation.gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] GameOpening Error: {e}");
        }
    }

    public async UniTask ShowPreGame()
    {
        var ct = _destroyCancellationToken;

        if (_preGamePopupTextAnimation == null)
            return;

        try
        {
            _HUDCanvasGroup.alpha = 0f;
            _HUDCanvasGroup.interactable = false;
            _HUDCanvasGroup.blocksRaycasts = false;

            // まず全HUDを正しい初期位置として記録
            for (int i = 0; i < _preGameHUDAnimation.Count; i++)
            {
                var anim = _preGameHUDAnimation[i];

                anim.gameObject.SetActive(true);
                anim.SetCurrentPositionAsInitial();
            }

            // レイアウトが絡む場合は1フレーム待つ
            await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);

            // もう一度、確定後の位置を初期位置として記録
            for (int i = 0; i < _preGameHUDAnimation.Count; i++)
            {
                var anim = _preGameHUDAnimation[i];
                anim.SetCurrentPositionAsInitial();
                anim.SetToEnterStartPosition();
            }

            // 画面外に置き終わってから見せる
            _HUDCanvasGroup.alpha = 1f;

            for (int i = 0; i < _preGameHUDAnimation.Count; i++)
            {
                var anim = _preGameHUDAnimation[i];

                anim.PlayEnterAsync(ct).Forget();

                await UniTask.Delay(
                    100,
                    delayType: DelayType.UnscaledDeltaTime,
                    cancellationToken: ct
                );
            }

            _HUDCanvasGroup.interactable = true;
            _HUDCanvasGroup.blocksRaycasts = true;

            _preGamePopupTextAnimation.gameObject.SetActive(true);

            await _preGamePopupTextAnimation.PlayPopupAsync(ct);

            ct.ThrowIfCancellationRequested();

            StartGameClickSubscription();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] ShowPreGame Error: {e}");
        }
    }

    private void StartGameClickSubscription()
    {
        _clickInputPublisher.OnClicked
            .Take(1) // 1回クリックされたら完了
            .Subscribe(_ =>
            {
                ExitPreGame();
                _gameStateChangeRequester.Request(GameStateKey.Play);
            })
            .AddTo(_disposables);
    }

    private void ExitPreGame()
    {
        _gameOpeningCanvasGroupFade.gameObject.SetActive(false);
        _preGamePopupTextAnimation.gameObject.SetActive(false);
    }

    private void OnDestroy()
        {
            _disposables.Dispose();
    }


}