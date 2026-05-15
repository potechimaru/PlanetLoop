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

    [Header("Result")]
    [SerializeField] private GameOverTextAnimation _gameOverTextAnimation;
    [SerializeField] private UIFadeMoveAnimation _scoreFadeMoveAnimation;
    [SerializeField] private UIFadeMoveAnimation _titleButtonFadeMoveAnimation;
    [SerializeField] private UIFadeMoveAnimation _restartButtonFadeMoveAnimation;
    [SerializeField] private CanvasGroupFader _resultCanvasGroupFader;
    [SerializeField] private List<UnderLineAnimation> _resultUnderLineAnimations;
    [SerializeField] private CanvasGroupFader _HUDCanvasGroupFader;

    [Header("ButtonSubscription")]
    [SerializeField] private ClickInputPublisher _clickInputPublisher;
    [SerializeField] private ToTitleButton _toTitleButton;
    [SerializeField] private RetryButton _retryButton;

    [Inject] private IGameStateChangeRequester _gameStateChangeRequester;
    [Inject] private SceneLoader sceneLoader;
    [Inject] private IAppStateChangeRequester appStateChangeRequester;

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
            _gameOpeningCanvasGroupFade.gameObject.SetActive(true);

            await _gameOpeningCanvasGroupFade.FadeToAsync(0.3f, 0.3f, ct);

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

    public async UniTask ShowGameOver()
    {
        var ct = _destroyCancellationToken;
        try
        {
            _gameOpeningCanvasGroupFade.gameObject.SetActive(true);
            await _gameOpeningCanvasGroupFade.FadeToAsync(0.9f, 0.3f, ct);

            if (_gameOverTextAnimation != null)
            {
                _HUDCanvasGroupFader.gameObject.SetActive(true);
                _HUDCanvasGroupFader.FadeOutAsync(0.8f, ct).Forget();
                _gameOverTextAnimation.gameObject.SetActive(true);
                _gameOverTextAnimation.ChangeActiveLetters();
                await _gameOverTextAnimation.PlayAsync(ct);
                _scoreFadeMoveAnimation.gameObject.SetActive(true);
                await _scoreFadeMoveAnimation.PlayAsync(ct);
                _titleButtonFadeMoveAnimation.gameObject.SetActive(true);
                _restartButtonFadeMoveAnimation.gameObject.SetActive(true);
                _titleButtonFadeMoveAnimation.PlayAsync(ct).Forget();
                _restartButtonFadeMoveAnimation.PlayAsync(ct).Forget();
                for (int i = 0; i < _resultUnderLineAnimations.Count; i++)
                {
                    var anim = _resultUnderLineAnimations[i];
                    anim.gameObject.SetActive(true);
                    anim.Play();
                }
                ResultClickSubscription();
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] ShowGameOver Error: {e}");
        }
    }

    private async UniTask ChangeToTitleScene()
    {
        var ct = _destroyCancellationToken;

        try
        {
            _resultCanvasGroupFader.gameObject.SetActive(true);
            await _resultCanvasGroupFader.FadeOutAsync();
            await sceneLoader.LoadTitleSceneAsync();
            await UniTask.Yield();


        }
        catch (OperationCanceledException)
        {

        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] ShowGameOver Error: {e}");
        }

    }

    private async UniTask RetryGame()
    {
        var ct = _destroyCancellationToken;
        try
        {
            _resultCanvasGroupFader.gameObject.SetActive(true);
            await _resultCanvasGroupFader.FadeOutAsync();
            await sceneLoader.RetryGameAsync();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] ShowGameOver Error: {e}");
        }

    }

    private void StartGameClickSubscription()
    {
        _clickInputPublisher.gameObject.SetActive(true);
        _clickInputPublisher.OnClicked
            .Take(1) // 1回クリックされたら完了
            .Subscribe(_ =>
            {
                ExitPreGame();
                _gameStateChangeRequester.Request(GameStateKey.Play);
                _clickInputPublisher.gameObject.SetActive(false);
            })
            .AddTo(_disposables);
    }

    private void ResultClickSubscription()
    {
        _toTitleButton.OnClicked
            .Subscribe(_ =>
            {
                ChangeToTitleScene().Forget();
            })
            .AddTo(_disposables);
        _retryButton.OnClicked
            .Subscribe(_ =>
            {
                RetryGame().Forget();
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