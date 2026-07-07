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

    [Header("Tutorial")]
    [SerializeField] private TutorialPanelAnimation _tutorialAnimation;
    [SerializeField] private TutorialPageController _tutorialPageController;
    [SerializeField] private TextSwitcher _nextButtonTextSwitcher;

    [Header("PreGame")]
    [SerializeField] private CanvasGroup _HUDCanvasGroup;
    [SerializeField] private PopupTextAnimation _preGamePopupTextAnimation;
    [SerializeField] private List<UIBounceMoveAnimation> _preGameHUDAnimation;

    [Header("Result")]
    [SerializeField] private GameOverTextAnimation _gameOverTextAnimation;
    [SerializeField] private UIFadeMoveAnimation _scoreFadeMoveAnimation;
    [SerializeField] private UIFadeMoveAnimation _timeFadeMoveAnimation;
    [SerializeField] private UIFadeMoveAnimation _titleButtonFadeMoveAnimation;
    [SerializeField] private UIFadeMoveAnimation _restartButtonFadeMoveAnimation;
    [SerializeField] private CanvasGroupFader _resultCanvasGroupFader;
    [SerializeField] private List<UnderLineAnimation> _resultUnderLineAnimations;
    [SerializeField] private CanvasGroupFader _HUDCanvasGroupFader;
    [SerializeField] private CanvasGroupFader _helperUICanvasGroupFader;
    [SerializeField] private ScoreCountUpAnimation _scoreCountUpAnimation;
    [SerializeField] private TimeCountUpAnimation _timeCountUpAnimation;

    [Header("Setting")] 
    [SerializeField] private TutorialPanelAnimation _settingAnimation;


    [Header("ButtonSubscription")]
    [SerializeField] private ClickInputPublisher _gameStartClickInputPublisher;
    [SerializeField] private ClickInputPublisher _tutorialNextClickInputPublisher;
    [SerializeField] private ClickInputPublisher _tutorialBackClickInputPublisher;
    [SerializeField] private ClickInputPublisher _settingClickInputPublisher;
    [SerializeField] private ClickInputPublisher _hintClickInputPublisher;
    [SerializeField] private ToTitleButton _toTitleButton;
    [SerializeField] private RetryButton _retryButton;

    [Inject] private IGameStateExternalFacade _gameStateExternalFacade;
    [Inject] private IGameStateChangeRequester _gameStateChangeRequester;
    [Inject] private SceneLoader sceneLoader;
    [Inject] private IAppStateChangeRequester appStateChangeRequester;

    [Inject] private AudioManager _audioManager;

    private bool _isSettingOpen = false;
    private bool _isTutorialOpen = false;
    private bool _isGameStarted = false;

    private CompositeDisposable _disposables = new();


    private CompositeDisposable _tutorialDisposables = new();
    private CompositeDisposable _preGameDisposables = new();
    private CompositeDisposable _resultDisposables = new();

    private CancellationToken _destroyCancellationToken;

    private UniTaskCompletionSource _tutorialClosedTcs;

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
            _audioManager.PlaySE(SEType.PreOpeningNoise);
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

    public async UniTask ShowTutorial()
    {
        var ct = _destroyCancellationToken;

        try
        {
            if (_tutorialAnimation == null || _isTutorialOpen)
                return;

            _tutorialClosedTcs = new UniTaskCompletionSource();

            if (_tutorialPageController != null)
                _tutorialPageController.ResetPage();

            if (_nextButtonTextSwitcher != null)
                _nextButtonTextSwitcher.Switch(true);

            _isTutorialOpen = true;
            ApplyTimeScale();

            _tutorialAnimation.gameObject.SetActive(true);
            await _tutorialAnimation.OpenAsync();

            DisposeTutorialSubscriptions();
            TutorialClickSubscription();

            ct.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] ShowTutorial Error: {e}");
        }
    }

    public async UniTask HideTutorial()
    {
        var ct = _destroyCancellationToken;

        try
        {
            if (_tutorialAnimation == null || !_isTutorialOpen)
                return;

            await _tutorialAnimation.CloseAsync();

            DisposeTutorialSubscriptions();

            _isTutorialOpen = false;
            ApplyTimeScale();

            _tutorialClosedTcs?.TrySetResult();

            ct.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            _tutorialClosedTcs?.TrySetCanceled();
        }
        catch (Exception e)
        {
            _tutorialClosedTcs?.TrySetException(e);
            Debug.LogError($"[GameUIManager] HideTutorial Error: {e}");
        }
    }

    public async UniTask WaitUntilTutorialClosed()
    {
        if (_tutorialClosedTcs == null)
            return;

        await _tutorialClosedTcs.Task;
    }

    public async UniTask ShowPreGame()
    {
        var ct = _destroyCancellationToken;

        if (_preGamePopupTextAnimation == null)
            return;

        try
        {
            _gameOpeningCanvasGroupFade.gameObject.SetActive(true);

            _gameOpeningCanvasGroupFade.FadeToAsync(0.5f, 0.3f, ct).Forget();

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


            // もう一度、確定後の位置を初期位置として記録
            for (int i = 0; i < _preGameHUDAnimation.Count; i++)
            {
                var anim = _preGameHUDAnimation[i];
                anim.SetCurrentPositionAsInitial();
                anim.SetToEnterStartPosition();
            }

            // 画面外に置き終わってから見せる
            _HUDCanvasGroupFader.gameObject.SetActive(true);
            _HUDCanvasGroupFader.FadeInAsync(0.5f, ct).Forget();
            //Debug.Log("HUD appear");


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

            DisposePreGameSubscriptions();

            StartGameClickSubscription();
            SettingClickSubscription();
            HintClickSubscription();
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
            if (this == null) return;

            if (_gameOpeningCanvasGroupFade != null)
            {
                _gameOpeningCanvasGroupFade.gameObject.SetActive(true);
                await _gameOpeningCanvasGroupFade.FadeToAsync(0.9f, 0.3f, ct);
            }

            ct.ThrowIfCancellationRequested();
            if (this == null) return;

            if (_gameOverTextAnimation == null) return;

            if (_HUDCanvasGroupFader != null)
            {
                _HUDCanvasGroupFader.gameObject.SetActive(true);
                _HUDCanvasGroupFader.FadeOutAsync(0.8f, ct).Forget();
            }

            if (_helperUICanvasGroupFader != null)
            {
                _helperUICanvasGroupFader.gameObject.SetActive(true);
                _helperUICanvasGroupFader.FadeOutAsync(0.8f, ct).Forget();
            }

            _gameOverTextAnimation.gameObject.SetActive(true);
            _gameOverTextAnimation.ChangeActiveLetters();

            await _gameOverTextAnimation.PlayAsync(ct);

            ct.ThrowIfCancellationRequested();
            if (this == null) return;

            if (_scoreFadeMoveAnimation != null)
            {
                _scoreFadeMoveAnimation.gameObject.SetActive(true);
                _timeFadeMoveAnimation.gameObject.SetActive(true);

                if (_scoreCountUpAnimation != null)
                    _scoreCountUpAnimation.PlayAsync(_gameStateExternalFacade.GetScore()).Forget();

                if (_timeCountUpAnimation != null)
                    _timeCountUpAnimation.PlayAsync(_gameStateExternalFacade.GetElapsedTime()).Forget();

                await UniTask.WhenAll(
                    _scoreFadeMoveAnimation.PlayAsync(ct),
                    _timeFadeMoveAnimation.PlayAsync(ct)
                );
            }

            ct.ThrowIfCancellationRequested();
            if (this == null) return;

            if (_titleButtonFadeMoveAnimation != null)
            {
                _titleButtonFadeMoveAnimation.gameObject.SetActive(true);
                _titleButtonFadeMoveAnimation.PlayAsync(ct).Forget();
            }

            if (_restartButtonFadeMoveAnimation != null)
            {
                _restartButtonFadeMoveAnimation.gameObject.SetActive(true);
                _restartButtonFadeMoveAnimation.PlayAsync(ct).Forget();
            }

            foreach (var anim in _resultUnderLineAnimations)
            {
                if (anim == null) continue;

                anim.gameObject.SetActive(true);
                anim.Play();
            }

            ResultClickSubscription();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception e)
        {
            Debug.LogError($"[GameUIManager] ShowGameOver Error: {e}");
        }
    }

    private async UniTask ShowSetting()
    {
        if (_settingAnimation == null || _isSettingOpen)
            return;

        _isSettingOpen = true;
        ApplyTimeScale();

        _settingAnimation.gameObject.SetActive(true);
        await _settingAnimation.OpenAsync();
    }

    private async UniTask HideSetting()
    {
        if (_settingAnimation == null || !_isSettingOpen)
            return;

        await _settingAnimation.CloseAsync();

        _isSettingOpen = false;
        ApplyTimeScale();
    }

    private async UniTask ChangeToTitleScene()
    {
        var ct = _destroyCancellationToken;

        try
        {
            _resultCanvasGroupFader.gameObject.SetActive(true);
            await _resultCanvasGroupFader.FadeInAsync();
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
            await _resultCanvasGroupFader.FadeInAsync();
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

    private void TutorialClickSubscription()
    {
        _tutorialNextClickInputPublisher.OnClicked
            .Subscribe(_ =>
            {
                if (_tutorialPageController == null)
                    return;

                if (_tutorialPageController.IsLastPage)
                {
                    _audioManager.PlaySE(SEType.Cancel);
                    HideTutorial().Forget();
                    return;
                }

                _audioManager.PlaySE(SEType.Click);
                _tutorialPageController.NextPage();

                if (_tutorialPageController.CurrentPageIndex == _tutorialPageController.PageCount - 1)
                {
                    _nextButtonTextSwitcher.Switch(false);
                }
            })
            .AddTo(_tutorialDisposables);

        _tutorialBackClickInputPublisher.OnClicked
            .Subscribe(_ =>
            {
                if (_tutorialPageController == null)
                    return;

                _audioManager.PlaySE(SEType.Click);
                _tutorialPageController.BackPage();

                if (_nextButtonTextSwitcher != null)
                    _nextButtonTextSwitcher.Switch(true);
            })
            .AddTo(_tutorialDisposables);
    }

    private void StartGameClickSubscription()
    {
        _gameStartClickInputPublisher.gameObject.SetActive(true);
        _gameStartClickInputPublisher.OnClicked
            .Take(1) // 1回クリックされたら完了
            .Subscribe(_ =>
            {
                _isGameStarted = true;
                ExitPreGame();
                _gameStateChangeRequester.Request(GameStateKey.Play);
                _gameStartClickInputPublisher.gameObject.SetActive(false);
            })
            .AddTo(_preGameDisposables);


    }

    private void ResultClickSubscription()
    {
        _toTitleButton.OnClicked
            .Subscribe(_ =>
            {
                _audioManager.PlaySE(SEType.Click);
                ChangeToTitleScene().Forget();
            })
            .AddTo(_disposables);
        _retryButton.OnClicked
            .Subscribe(_ =>
            {
                _audioManager.PlaySE(SEType.Click);
                RetryGame().Forget();
            })
            .AddTo(_disposables);   

    }

    private void SettingClickSubscription()
    {
        _settingClickInputPublisher.OnClicked
            .Subscribe(_ =>
            {
                _audioManager.PlaySE(SEType.Click);

                if (_isSettingOpen)
                    HideSetting().Forget();
                else
                    ShowSetting().Forget();
            })
            .AddTo(_preGameDisposables);
    }

    private void HintClickSubscription()
    {
        _hintClickInputPublisher.OnClicked
            .Subscribe(_ =>
            {
                _audioManager.PlaySE(SEType.Click);

                if (_isTutorialOpen)
                    HideTutorial().Forget();
                else
                    ShowTutorial().Forget();
            })
            .AddTo(_preGameDisposables);
    }

    private void ExitPreGame()
    {
        _gameOpeningCanvasGroupFade.gameObject.SetActive(false);
        _preGamePopupTextAnimation.gameObject.SetActive(false);
    }

    private void ApplyTimeScale()
    {
        Time.timeScale = (_isSettingOpen || _isTutorialOpen || !_isGameStarted) ? 0f : 1f;
    }

    private void DisposeTutorialSubscriptions()
    {
        _tutorialDisposables.Dispose();
        _tutorialDisposables = new CompositeDisposable();
    }

    private void DisposePreGameSubscriptions()
    {
        _preGameDisposables.Dispose();
        _preGameDisposables = new CompositeDisposable();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
        _tutorialDisposables.Dispose();
        _preGameDisposables.Dispose();
        _resultDisposables.Dispose();

        Time.timeScale = 1f;
    }


}