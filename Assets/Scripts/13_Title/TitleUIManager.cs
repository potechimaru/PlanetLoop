using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

public interface ITitleUIManager
{
    UniTask EnterTitleAnimation();
    UniTask ExitTitleAnimation();
    UniTask EnterModeSelectAnimation();
}

public class TitleUIManager : MonoBehaviour, ITitleUIManager
{
    [Header("EnterTitleAnimation")]
    [SerializeField] private List<UnderLineAnimation> _underLineAnimations;
    [SerializeField] private List<TextRiseAnimation> _textRiseAnimations;
    [SerializeField] private DummyPlayerSplineAnimator _dummyPlayerSplineAnimator;

    [Header("ExitTitleAnimation")]
    [SerializeField] private List<UIBounceMoveAnimation> _uiExitBounceAnimations;

    [Header("EnterModeSelectAnimation")]
    [SerializeField] private SplineMoveAnimation _enterModeSelectSplineMoveAnimation;
    [SerializeField] private TextRiseAnimation _OKTextRiseAnimation;
    [SerializeField] private GameModeCarouselController _gameModeCarouselController;
    [SerializeField] private List<UIBounceMoveAnimation> _enterModeSelectBounceAnimations;
    [SerializeField] private FloatLoopAnimation _enterModeSelectFloatLoopAnimation;
    [SerializeField] private CanvasGroupFader _enterModeSelectDescriptionPanel;
    [SerializeField] private TMPDoTextAnimation _modeDescriptionTextAnimation;
    [SerializeField] private GameModeDatabase _gameModeDatabase;
    [SerializeField] private CanvasGroupFader _OKCanvasGroupFader;
    [SerializeField] private CanvasGroupFader _modeNameCanvasGroupFader;
    [SerializeField] private CanvasGroupFader _modeDescriptionCanvasGroupFader;
    [SerializeField] private TextMeshProUGUI _highScore;

    [Header("ExitModeSelectAnimation")]
    [SerializeField] private List<UIBounceMoveAnimation> _exitModeSelectBounceAnimations;
    [SerializeField] private CameraApproachAnimation _cameraApproachAnimation;
    [SerializeField] private CanvasGroupFader _blackBackFader;

    [Header("SettingAnimation")]
    [SerializeField] private TutorialPanelAnimation _settingPanelAnimation;

    [Header("ButtonSubscription")]
    [SerializeField] private StartButton _startButton;
    [SerializeField] private GearButton _gearButton;
    [SerializeField] private RightSelectButton _rightSelectButton;
    [SerializeField] private LeftSelectButton _leftSelectButton;
    [SerializeField] private OKButton _OKButton;
    [SerializeField] private ClickInputPublisher _settingBackClickInputPublisher;

    private IGameModeManager _gameModeManager;
    private TitleState _titleState;
    private ModeSelectState _modeSelectState;
    private IAppStateChangeRequester _stateChangeRequester;
    private AudioManager _audioManager;
    private SaveDataService _saveDataService;

    private bool _isModeChanging;
    private bool _initialized;
    private bool _isSettingOpen = false;

    [Inject]
    public void Construct(
    IGameModeManager gameModeManager,
    TitleState titleState,
    ModeSelectState modeSelectState,
    IAppStateChangeRequester stateChangeRequester,
    AudioManager audioManager,
    SaveDataService saveDataService)
    {
        //Debug.Log("[TitleUIManager] Construct called", this);

        _gameModeManager = gameModeManager;
        _titleState = titleState;
        _modeSelectState = modeSelectState;
        _stateChangeRequester = stateChangeRequester;
        _audioManager = audioManager;
        _saveDataService = saveDataService;

        //Debug.Log($"TitleState Null: {_titleState == null}", this);

        Initialize();
    }

    private void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;

        if (_startButton != null)
        {
            _startButton.OnClicked
                .Subscribe(_ =>
                {
                    _audioManager.PlaySE(SEType.Click);
                    _stateChangeRequester.Request(AppStateKey.ModeSelectState);
                })
                .AddTo(this);
        }

        if (_gearButton != null)
        {
            _gearButton.OnClicked
                .Subscribe(_ => {
                    _audioManager.PlaySE(SEType.Click);
                    
                    if (_isSettingOpen)
                        HideSetting().Forget();
                    else
                        ShowSetting().Forget();
                })
                .AddTo(this);
        }

        if (_settingBackClickInputPublisher != null)
        {
            _settingBackClickInputPublisher.OnClicked
            .Subscribe(_ =>
            {
                _audioManager.PlaySE(SEType.Click);
                if (_isSettingOpen)
                    HideSetting().Forget();
            })
            .AddTo(this);
        }

        if (_rightSelectButton != null)
        {
            _rightSelectButton.OnClicked
                .Subscribe(_ =>
                {
                    if (_isModeChanging) return;
                    _audioManager.PlaySE(SEType.Click);
                    RightChangeModeAnimation().Forget();
                })
                .AddTo(this);
        }

        if (_leftSelectButton != null)
        {
            _leftSelectButton.OnClicked
                .Subscribe(_ =>
                {
                    if (_isModeChanging) return;
                    _audioManager.PlaySE(SEType.Click);
                    LeftChangeModeAnimation().Forget();
                })
                .AddTo(this);
        }

        if (_OKButton != null)
        {
            _OKButton.OnClicked
                .Subscribe(_ =>
                {
                    _audioManager.PlaySE(SEType.Click);
                    ExitModeSelectAnimation().Forget();
                })
                .AddTo(this);
        }

        if (_gameModeManager != null)
        {
            _gameModeManager.OnSelectedModeChanged
    .Subscribe(mode =>
    {
        if (_gameModeDatabase == null)
            return;

        var modeData = _gameModeDatabase.GetModeData(mode);

        if (modeData == null)
            return;

        if (_modeDescriptionTextAnimation != null)
        {
            if (_enterModeSelectDescriptionPanel != null &&
                _enterModeSelectDescriptionPanel.gameObject.activeInHierarchy)
            {
                FadeModeDescription(modeData).Forget();
                FadeModeName(modeData).Forget();
            }
            else
            {
                _modeDescriptionTextAnimation.SetTargetText(modeData.Description);

                if (_modeNameCanvasGroupFader != null)
                {
                    var text =
                        _modeNameCanvasGroupFader.GetComponent<TextMeshProUGUI>();

                    if (text != null)
                        text.text = modeData.DisplayName;
                }
            }
        }

        UpdateOKButtonState(mode, animated: true);
    })
    .AddTo(this);
        }


        _titleState.OnEntered
            .Subscribe(_ => EnterTitleAnimation().Forget())
            .AddTo(this);

        _titleState.OnExited
            .Subscribe(_ => ExitTitleAnimation().Forget())
            .AddTo(this);

        _modeSelectState.OnEntered
            .Subscribe(_ => EnterModeSelectAnimation().Forget())
            .AddTo(this);

        _modeSelectState.OnExited
            .Subscribe(_ => { })
            .AddTo(this);
    }

    private async UniTask FadeModeName(GameModeData gameModeData)
    {
        if (_modeNameCanvasGroupFader == null)
            return;

        await _modeNameCanvasGroupFader.FadeOutAsync();

        var text = _modeNameCanvasGroupFader.GetComponent<TextMeshProUGUI>();
        if (text != null)
            text.text = gameModeData.DisplayName;

        await _modeNameCanvasGroupFader.FadeInAsync();
    }

    private async UniTask FadeModeDescription(GameModeData gameModeData)
    {
        if (_modeDescriptionCanvasGroupFader == null)
            return;

        await _modeDescriptionCanvasGroupFader.FadeOutAsync();

        if (_modeDescriptionTextAnimation != null)
            _modeDescriptionTextAnimation.SetTargetText(gameModeData.Description);
    }

    public async UniTask EnterTitleAnimation()
    {
        var tasks = new List<UniTask>();

        foreach (var anim in _underLineAnimations)
        {
            if (anim != null)
                anim.Play();
        }

        foreach (var anim in _textRiseAnimations)
        {
            if (anim != null)
                tasks.Add(anim.PlayAsync());
        }

        if (_dummyPlayerSplineAnimator != null)
            _dummyPlayerSplineAnimator.Play();

        await UniTask.WhenAll(tasks);
    }

    public async UniTask ExitTitleAnimation()
    {
        var tasks = new List<UniTask>();

        foreach (var anim in _uiExitBounceAnimations)
        {
            if (anim != null)
                tasks.Add(anim.PlayExitAsync());
        }

        await UniTask.WhenAll(tasks);

        foreach (var anim in _uiExitBounceAnimations)
        {
            if (anim != null)
                anim.gameObject.SetActive(false);
        }
    }

    public async UniTask EnterModeSelectAnimation()
    {
        var tasks = new List<UniTask>();

        if (_enterModeSelectSplineMoveAnimation != null)
            await _enterModeSelectSplineMoveAnimation.PlayAsync();

        _highScore.text = _saveDataService.GetHighScore(_gameModeManager.CurrentSelectedMode).ToString();

        if (_gameModeCarouselController != null)
        {
            _gameModeCarouselController.ActiveSlot();
            tasks.Add(_gameModeCarouselController.PlayFormationAsync());
        }

        foreach (var anim in _enterModeSelectBounceAnimations)
        {
            if (anim == null)
                continue;

            anim.gameObject.SetActive(true);
            tasks.Add(anim.PlayEnterAsync());
        }

        if (_enterModeSelectDescriptionPanel != null)
        {
            _enterModeSelectDescriptionPanel.gameObject.SetActive(true);
            tasks.Add(_enterModeSelectDescriptionPanel.FadeInAsync());
        }

        if (_OKTextRiseAnimation != null)
        {
            if (_OKCanvasGroupFader != null)
            {
                _OKCanvasGroupFader.gameObject.SetActive(true);
                tasks.Add(_OKCanvasGroupFader.FadeInAsync());
            }

            tasks.Add(_OKTextRiseAnimation.PlayAsync());
        }

        await UniTask.WhenAll(tasks);

        if (_enterModeSelectFloatLoopAnimation != null)
            _enterModeSelectFloatLoopAnimation.Play();

        if (_modeDescriptionTextAnimation != null)
        {
            _modeDescriptionTextAnimation.gameObject.SetActive(true);
            _modeDescriptionTextAnimation.PlayAsync().Forget();
        }
    }

    public async UniTask ExitModeSelectAnimation()
    {
        var tasks = new List<UniTask>();

        foreach (var anim in _exitModeSelectBounceAnimations)
        {
            if (anim != null)
                tasks.Add(anim.PlayExitAsync());
        }

        if (_cameraApproachAnimation != null)
            tasks.Add(_cameraApproachAnimation.PlayAsync());

        if (_blackBackFader != null)
        {
            _blackBackFader.gameObject.SetActive(true);
            tasks.Add(_blackBackFader.FadeInAsync());
        }

        await UniTask.WhenAll(tasks);

        foreach (var anim in _exitModeSelectBounceAnimations)
        {
            if (anim != null)
                anim.gameObject.SetActive(false);
        }

        _stateChangeRequester.Request(AppStateKey.Game);
    }

    public async UniTask RightChangeModeAnimation()
    {
        if (_gameModeCarouselController == null)
            return;

        if (_isModeChanging)
            return;

        _isModeChanging = true;

        try
        {

            await _gameModeCarouselController.RotateRightAsync();

            _highScore.text = _saveDataService.GetHighScore(_gameModeManager.CurrentSelectedMode).ToString();

            if (_modeDescriptionTextAnimation == null)
                return;

            _modeDescriptionTextAnimation.PlayAsync().Forget();

            if (_modeDescriptionCanvasGroupFader != null)
                _modeDescriptionCanvasGroupFader.FullAlpha();
        }
        finally
        {
            _isModeChanging = false;
        }
    }

    public async UniTask LeftChangeModeAnimation()
    {
        if (_gameModeCarouselController == null)
            return;

        if (_isModeChanging)
            return;

        _isModeChanging = true;

        try
        {
            

            await _gameModeCarouselController.RotateLeftAsync();

            _highScore.text = _saveDataService.GetHighScore(_gameModeManager.CurrentSelectedMode).ToString();

            if (_modeDescriptionTextAnimation == null)
                return;

            _modeDescriptionTextAnimation.PlayAsync().Forget();

            if (_modeDescriptionCanvasGroupFader != null)
                _modeDescriptionCanvasGroupFader.FullAlpha();
        }
        finally
        {
            _isModeChanging = false;
        }
    }

    private async UniTask ShowSetting()
    {
        if (_settingPanelAnimation == null || _isSettingOpen)
            return;

        _isSettingOpen = true;

        _settingPanelAnimation.gameObject.SetActive(true);
        await _settingPanelAnimation.OpenAsync();
    }

    private async UniTask HideSetting()
    {
        if (_settingPanelAnimation == null || !_isSettingOpen)
            return;

        await _settingPanelAnimation.CloseAsync();

        _isSettingOpen = false;
    }

    public void ChangeOKButtonActive()
    {
        _OKButton.IsButtonActive = !_OKButton.IsButtonActive;
        if (_OKButton.IsButtonActive)
        {
            _OKCanvasGroupFader.FadeToAsync(1f, 0.2f).Forget();
        }
        else
        {
            _OKCanvasGroupFader.FadeToAsync(0.5f, 0.2f).Forget();
        }

    }

    private void UpdateOKButtonState(GameModeType mode, bool animated)
    {
        if (_OKButton == null)
            return;

        bool isComingSoon = _gameModeManager.JudgeComminSoonGameMode(mode);

        _OKButton.IsButtonActive = !isComingSoon;

        if (_OKCanvasGroupFader == null)
            return;

        float targetAlpha = isComingSoon ? 0.5f : 1f;

        if (!animated || !_OKCanvasGroupFader.gameObject.activeInHierarchy)
        {
            _OKCanvasGroupFader.SetAlpha(targetAlpha);
            return;
        }

        _OKCanvasGroupFader
            .FadeToAsync(targetAlpha, 0.2f)
            .Forget();
    }

}