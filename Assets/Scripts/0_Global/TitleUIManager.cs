using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

public interface ITitleUIManager
{
    UniTask EnterTitleAnimation();
    UniTask ExitTitleAnimation();
    UniTask EnterModeSelectAnimation();
     void ShowSettingAnimation();
     void HideSettingAnimation();
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

    [Header("ExitModeSelectAnimation")]
    [SerializeField] private List<UIBounceMoveAnimation> _exitModeSelectBounceAnimations;
    [SerializeField] private CameraApproachAnimation _cameraApproachAnimation;
    [SerializeField] private CanvasGroupFader _blackBackFader;

    [Header("ButtonSubscription")]
    [SerializeField] private StartButton _startButton;
    [SerializeField] private GearButton _gearButton;
    [SerializeField] private RightSelectButton _rightSelectButton;
    [SerializeField] private LeftSelectButton _leftSelectButton;
    [SerializeField] private OKButton _OKButton;

    [Inject] private IGameModeManager _gameModeManager;

    [Inject] private TitleState _titleState;
    [Inject] private ModeSelectState _modeSelectState;

    [Inject] private IAppStateChangeRequester _stateChangeRequester;
    [Inject] private SceneLoader _sceneLoader;

    private bool _isModeChanging;

    private void Start()
    {
        _startButton.OnClicked.Subscribe(_ => {
            _stateChangeRequester.Request(AppStateKey.ModeSelectState);
        }).AddTo(this);
        _gearButton.OnClicked.Subscribe(_ => ShowSettingAnimation()).AddTo(this);
        if (_rightSelectButton != null)
        {
            _rightSelectButton.OnClicked
                .Subscribe(_ =>
                {
                    if (_isModeChanging) return;
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
                    LeftChangeModeAnimation().Forget();
                })
                .AddTo(this);
        }

        if (_OKButton != null)
        {
            _OKButton.OnClicked
                .Subscribe(_ =>
                {
                    ExitModeSelectAnimation().Forget();
                })
                .AddTo(this);
        }

        if (_gameModeManager != null)
        {
            _gameModeManager.OnSelectedModeChanged
            .Subscribe(mode =>
            {
                var modeData = _gameModeDatabase.GetModeData(mode);
                if (modeData != null && _modeDescriptionTextAnimation != null)
                {

                    if (_enterModeSelectDescriptionPanel.gameObject.activeInHierarchy != false)
                    {

                        FadeModeDescription(modeData).Forget();
                        FadeModeName(modeData).Forget();

                        return;
                    }
                    else
                    {
                        _modeDescriptionTextAnimation.SetTargetText(modeData.Description);
                        _modeNameCanvasGroupFader.GetComponent<TextMeshProUGUI>().text = modeData.DisplayName;
                    }
                }
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
        await _modeNameCanvasGroupFader.FadeOutAsync();
        _modeNameCanvasGroupFader.GetComponent<TextMeshProUGUI>().text = gameModeData.DisplayName;
        await _modeNameCanvasGroupFader.FadeInAsync();

    }

    private async UniTask FadeModeDescription(GameModeData gameModeData)
    {
        await _modeDescriptionCanvasGroupFader.FadeOutAsync();
        _modeDescriptionTextAnimation.SetTargetText(gameModeData.Description);
    }


    /// <summary>
    /// タイトル画面のアニメーションを開始する
    /// </summary>
    public async UniTask EnterTitleAnimation()
    {
        var tasks = new List<UniTask>();

        foreach (var anim in _underLineAnimations)
        {
            anim.Play();
        }
        foreach (var anim in _textRiseAnimations)
        {
            tasks.Add(anim.PlayAsync());
        }
        if (_dummyPlayerSplineAnimator != null)
        {
            //Debug.Log("Play DummyPlayerSplineAnimator");
            _dummyPlayerSplineAnimator.Play();
        }

        await UniTask.WhenAll(tasks);
    }

    /// <summary>
    /// タイトル画面の終了アニメーションを開始する＆ゲームモード選択画面へ遷移する
    /// </summary>
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
            anim.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ゲームモード選択画面のアニメーションを開始する
    /// </summary>
    /// <returns></returns>
    public async UniTask EnterModeSelectAnimation()
    {
        var tasks = new List<UniTask>();

        if (_enterModeSelectSplineMoveAnimation != null)
        {
            await _enterModeSelectSplineMoveAnimation.PlayAsync();
        }

        if (_gameModeCarouselController != null)
        {
            _gameModeCarouselController.ActiveSlot();
            tasks.Add(_gameModeCarouselController.PlayFormationAsync());
        }

        foreach (var anim in _enterModeSelectBounceAnimations)
        {
            anim.gameObject.SetActive(true);
            if (anim != null)
                tasks.Add(anim.PlayEnterAsync());
        }

        if (_enterModeSelectDescriptionPanel != null)
        {
            _enterModeSelectDescriptionPanel.gameObject.SetActive(true);
            tasks.Add(_enterModeSelectDescriptionPanel.FadeInAsync());
        }

        if (_OKTextRiseAnimation != null)
        {
            _OKCanvasGroupFader.gameObject.SetActive(true);
            tasks.Add(_OKTextRiseAnimation.PlayAsync());
            tasks.Add(_OKCanvasGroupFader.FadeInAsync());
        }

        await UniTask.WhenAll(tasks);


        if (_enterModeSelectFloatLoopAnimation != null)
        {
            _enterModeSelectFloatLoopAnimation.Play();
        }

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
        {
            tasks.Add(_cameraApproachAnimation.PlayAsync());
        }

        if (_blackBackFader != null)
        {
            _blackBackFader.gameObject.SetActive(true);
            tasks.Add(_blackBackFader.FadeInAsync());
        }

        await UniTask.WhenAll(tasks);
        foreach (var anim in _exitModeSelectBounceAnimations)
        {
            anim.gameObject.SetActive(false);
        }

        var selectedMode = _gameModeManager.CurrentSelectedMode;
        var modeData = _gameModeDatabase.GetModeData(selectedMode);
        if (modeData != null)
        {
            _sceneLoader.LoadGameSceneAsync(modeData.GameModeType).Forget();
        }
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

            if (_modeDescriptionTextAnimation == null)
                return;

            _modeDescriptionTextAnimation.PlayAsync().Forget();
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

            if (_modeDescriptionTextAnimation == null)
                return;

            _modeDescriptionTextAnimation.PlayAsync().Forget();
            _modeDescriptionCanvasGroupFader.FullAlpha();
        }
        finally
        {
            _isModeChanging = false;
        }
    }

    /// <summary>
    /// 設定ボタンを押したときのウィンドウのアニメーション
    /// </summary>
    public void ShowSettingAnimation()
    {

    }

    public void HideSettingAnimation()
    {
    }


}
