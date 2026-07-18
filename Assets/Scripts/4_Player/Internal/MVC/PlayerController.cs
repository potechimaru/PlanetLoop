
using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;

/// <summary>
/// Player・ｽS・ｽﾌの進・ｽs・ｽ・ｽ・ｽﾇ暦ｿｽ・ｽ・ｽ・ｽ・ｽR・ｽ・ｽ・ｽg・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ[・ｽB
///
/// ・ｽ・ｽﾉ以会ｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
///
/// ・ｽEPlayerModel・ｽAPlayerView・ｽAPlayerSplineMover・ｽﾌ連・ｽg
/// ・ｽEPlayerStateMachine・ｽﾌ擾ｿｽﾔ更・ｽV
/// ・ｽE・ｽﾚ難ｿｽ・ｽA・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽA・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾍの趣ｿｽt
/// ・ｽE・ｽG・ｽe・ｽA・ｽ・ｽ・ｽ[・ｽU・ｽ[・ｽA・ｽ・ｽQ・ｽ・ｽ・ｽﾈどとの接触・ｽ・ｽ・ｽ・ｽ
/// ・ｽE・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽﾉ会ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽ狽ﾌ設抵ｿｽ
/// ・ｽEPlayer・ｽ・ｽ・ｽS・ｽ・ｽ・ｽﾌ通知・ｽﾆ会ｿｽ・ｽo
///
/// ・ｽ・ｽ・ｽﾛゑｿｽSpline・ｽ・ｽﾌ移難ｿｽ・ｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽW・ｽﾌ計・ｽZ・ｽ・ｽ
/// PlayerSplineMover・ｽﾖ委擾ｿｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽB
/// </summary>
public class PlayerController : ITickable, IDisposable
{
    /* =========================================================
     * Player・ｽ・ｽ・ｽ\・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽN・ｽ・ｽ・ｽX
     * ========================================================= */

    /// <summary>
    /// Player・ｽﾌゲ・ｽ[・ｽ・ｽ・ｽ・ｽﾌ擾ｿｽﾔや数・ｽl・ｽ・ｽﾛ趣ｿｽ・ｽ・ｽ・ｽ・ｽModel・ｽB
    ///
    /// ・ｽ・ｽﾉ以会ｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽﾇ暦ｿｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽE・ｽ・ｽ・ｽﾝの移難ｿｽ・ｽ・ｽ・ｽx
    /// ・ｽE・ｽ・ｽ・ｽﾝのジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽx
    /// ・ｽE・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ
    /// ・ｽE・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽ・ｽ
    /// ・ｽE・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ
    /// ・ｽE・ｽc・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽ
    /// </summary>
    private readonly PlayerModel _model;

    /// <summary>
    /// Player・ｽﾌ鯉ｿｽ・ｽ・ｽ・ｽﾚゑｿｽTransform・ｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽView・ｽB
    ///
    /// ・ｽ・ｽﾉ以会ｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽﾉ使・ｽp・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽEPlayer・ｽ・ｽ・ｽW・ｽﾌ更・ｽV
    /// ・ｽE・ｽI・ｽ[・ｽ・ｽ・ｽF・ｽﾌ変更
    /// ・ｽE・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽK・ｽC・ｽh・ｽﾌ表・ｽ・ｽ
    /// ・ｽE・ｽ・ｽ・ｽS・ｽG・ｽt・ｽF・ｽN・ｽg・ｽﾌ再撰ｿｽ
    /// </summary>
    private readonly PlayerView _view;

    /// <summary>
    /// Player・ｽ・ｽSpline・ｽ・ｽﾌ移難ｿｽ・ｽA
    /// ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾌ移難ｿｽ・ｽA
    /// ・ｽ・ｽSpline・ｽﾖの吸・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽN・ｽ・ｽ・ｽX・ｽB
    /// </summary>
    private readonly PlayerSplineMover _mover;

    /// <summary>
    /// Player・ｽ・ｽSpline・ｽﾖ吸・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾛのイ・ｽx・ｽ・ｽ・ｽg・ｽ・ｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽﾉ以会ｿｽ・ｽﾌ通知・ｽ・ｽ・ｽs・ｽ・ｽ・ｽB
    ///
    /// ・ｽE・ｽ・ｽ・ｽ・ｽ・ｽO・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ
    /// ・ｽE・ｽV・ｽ・ｽ・ｽ・ｽSpline・ｽﾖの擾ｿｽ・ｽ・n
    /// ・ｽE・ｽ・ｽ・ｽn・ｽ・ｽ・ｽo
    /// </summary>
    private readonly AttachEvent _attachEvent;

    /// <summary>
    /// Player・ｽ@・ｽ\・ｽﾌ外・ｽ・ｽ・ｽﾉゑｿｽ・ｽ・ｽV・ｽX・ｽe・ｽ・ｽ・ｽﾖア・ｽN・ｽZ・ｽX・ｽ・ｽ・ｽ・ｽFacade・ｽB
    ///
    /// ・ｽ・ｽﾉ以会ｿｽ・ｽﾌ機・ｽ\・ｽ・p・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽE・ｽ・ｽ・ｽﾍイ・ｽx・ｽ・ｽ・ｽg・ｽﾌ購・ｽ・ｽ
    /// ・ｽESpline・ｽ・ｽ・ｽ・ｽ
    /// ・ｽE・ｽG・ｽ・ｽ・ｽQ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾌ接触・ｽﾊ知
    /// ・ｽE・ｽG・ｽﾌ鯉ｿｽ・ｽj
    /// ・ｽESE・ｽﾌ再撰ｿｽ・ｽﾆ抵ｿｽ~
    /// ・ｽE・ｽJ・ｽnSpline・ｽﾌ取得
    /// ・ｽE・ｽu・ｽ・ｽ・ｽb・ｽN・ｽz・ｽ[・ｽ・ｽ・ｽﾉゑｿｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽﾌ変更
    /// </summary>
    private readonly IPlayerExternalFacade _playerExternalFacade;

    /// <summary>
    /// Player・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽz・ｽu・ｽn・ｽ_・ｽ・ｽObstacle・ｽ・ｽPointObject・ｽﾈどゑｿｽ
    /// ・ｽd・ｽﾈゑｿｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽｻ定す・ｽ・ｽN・ｽ・ｽ・ｽX・ｽB
    ///
    /// PlayerSplineMover・ｽﾖ渡・ｽ・ｽ・ｽ・ｽA
    /// ・ｽ・ｽ・ｽS・ｽﾈ擾ｿｽ・ｽ・ｽ・ｽﾊ置・ｽﾌ鯉ｿｽ・ｽ・ｽ・ｽﾉ使・ｽp・ｽ・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    private readonly PlayerSpawnOverlapResolver _spawnOverlapResolver;


    /* =========================================================
     * ・ｽX・ｽe・ｽ[・ｽg・ｽ}・ｽV・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// Player・ｽﾌ鯉ｿｽ・ｽﾝ擾ｿｽﾔゑｿｽ・ｽﾇ暦ｿｽ・ｽ・ｽ・ｽ・ｽX・ｽe・ｽ[・ｽg・ｽ}・ｽV・ｽ・ｽ・ｽB
    ///
    /// ・ｽz・ｽ閧ｳ・ｽ・ｽ・ｽ・ｽﾔは以会ｿｽ・ｽB
    ///
    /// ・ｽEMoveState
    /// ・ｽEChargeState
    /// ・ｽEJumpState
    /// ・ｽEGameOverState
    /// </summary>
    private PlayerStateMachine _playerStateMachine;


    /* =========================================================
     * UniRx・ｽw・ｽﾇ管暦ｿｽ
     * ========================================================= */

    /// <summary>
    /// Player・ｽﾉ関ゑｿｽ・ｽ・ｽO・ｽ・ｽ・ｽC・ｽx・ｽ・ｽ・ｽg・ｽw・ｽﾇゑｿｽ・ｽﾜとめて管暦ｿｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// Dispose・ｽ・ｽ・ｽﾉ一括・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ驍ｱ・ｽﾆで、
    /// ・ｽw・ｽﾇの残・ｽ・ｽ・ｽ竭ｽ・ｽd・ｽw・ｽﾇゑｿｽh・ｽ~・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    private readonly CompositeDisposable _playerSubscriptions = new();


    /* =========================================================
     * PlayerController・ｽ・ｽ・ｽ・ｽO・ｽ・ｽ・ｽﾖ鯉ｿｽ・ｽJ・ｽ・ｽ・ｽ・ｽC・ｽx・ｽ・ｽ・ｽg
     * ========================================================= */

    /// <summary>
    /// ・ｽ・ｽ・ｽ・ｽ・ｽO・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽﾊ知・ｽ・ｽ・ｽ・ｽSubject・ｽB
    ///
    /// ・ｽ・ｽ・ｽﾛの費ｿｽ・ｽ・ｽ・ｽAttachEvent・ｽ・ｽ・ｽﾅ行・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    private Subject<Unit> _onLongJumped = new Subject<Unit>();

    /// <summary>
    /// ・ｽ・ｽ・ｽ・ｽ・ｽO・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾉ通知・ｽ・ｽ・ｽ・ｽ・ｽObservable・ｽB
    ///
    /// ・ｽO・ｽ・ｽ・ｽ・ｽ・ｽ・ｽOnNext・ｽ・ｽ・ｽﾄべなゑｿｽ・ｽ謔､・ｽA
    /// IObservable・ｽﾆゑｿｽ・ｽﾄ鯉ｿｽ・ｽJ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽB
    /// </summary>
    public IObservable<Unit> OnLongJumped => _onLongJumped;

    /// <summary>
    /// ・ｽ・ｽ・ｽK・ｽ・ｽ・ｽSpline・ｽﾖ擾ｿｽ・ｽﾟて吸・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽﾊ知・ｽ・ｽ・ｽ・ｽSubject・ｽB
    /// </summary>
    private Subject<Unit> _onNewOrbitAttached = new Subject<Unit>();

    /// <summary>
    /// ・ｽV・ｽ・ｽ・ｽ・ｽSpline・ｽﾖ擾ｿｽ・ｽﾟて吸・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾉ通知・ｽ・ｽ・ｽ・ｽ・ｽObservable・ｽB
    /// </summary>
    public IObservable<Unit> OnNewOrbitAttached => _onNewOrbitAttached;

    /// <summary>
    /// Player・ｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽﾊ知・ｽ・ｽ・ｽ・ｽSubject・ｽB
    /// </summary>
    private Subject<Unit> _onPlayerDead = new Subject<Unit>();

    /// <summary>
    /// Player・ｽ・ｽ・ｽS・ｽ・ｽ・ｽﾉ通知・ｽ・ｽ・ｽ・ｽ・ｽObservable・ｽB
    /// </summary>
    public IObservable<Unit> OnPlayerDead => _onPlayerDead;


    /* =========================================================
     * ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽﾔ管暦ｿｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽO・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽ_・ｽﾌチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽﾝのチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽﾆ費ｿｽr・ｽ・ｽ・ｽA
    /// ・ｽ・ｽ・ｽx・ｽ・ｽ・ｽ・ｽ・ｽﾏ会ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽu・ｽﾔゑｿｽ・ｽ・ｽSE・ｽ・ｽﾘゑｿｽﾖゑｿｽ・ｽ驍ｽ・ｽﾟに使・ｽp・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽSE・ｽ・ｽ・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽh・ｽ・ｽ・ｽﾅゑｿｽ・ｽ・ｽB
    /// </summary>
    private ChargeLevel _previousChargeLevel = ChargeLevel.Normal;


    /* =========================================================
     * ・ｽR・ｽ・ｽ・ｽX・ｽg・ｽ・ｽ・ｽN・ｽ^
     * ========================================================= */

    /// <summary>
    /// PlayerController・ｽｶ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// PlayerModel・ｽAAttachEvent・ｽAPlayerSplineMover・ｽ・ｽ
    /// ・ｽ・ｽ・ｽﾌコ・ｽ・ｽ・ｽX・ｽg・ｽ・ｽ・ｽN・ｽ^・ｽ・ｽ・ｽﾅ撰ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    /// <param name="view">
    /// Player・ｽﾌ表・ｽ・ｽ・ｽETransform・ｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽView・ｽB
    /// </param>
    /// <param name="playerExternalFacade">
    /// ・ｽO・ｽ・ｽ・ｽV・ｽX・ｽe・ｽ・ｽ・ｽﾖア・ｽN・ｽZ・ｽX・ｽ・ｽ・ｽ・ｽFacade・ｽB
    /// </param>
    /// <param name="spawnOverlapResolver">
    /// Player・ｽ・ｽ・ｽ・ｽ・ｽﾊ置・ｽﾌ重・ｽﾈゑｿｽｻ定す・ｽ・ｽN・ｽ・ｽ・ｽX・ｽB
    /// </param>
    public PlayerController(
        PlayerView view,
        IPlayerExternalFacade playerExternalFacade,
        PlayerSpawnOverlapResolver spawnOverlapResolver)
    {
        // Player・ｽﾌ擾ｿｽﾔデ・ｽ[・ｽ^・ｽ・ｽﾛ趣ｿｽ・ｽ・ｽ・ｽ・ｽModel・ｽｶ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
        _model = new PlayerModel();

        _view = view;
        _playerExternalFacade = playerExternalFacade;
        _spawnOverlapResolver = spawnOverlapResolver;

        /*
         * Spline・ｽz・ｽ・ｽ・ｽ・ｽ・ｽﾌイ・ｽx・ｽ・ｽ・ｽg・ｽ・ｽ・ｽ・ｽ・ｽｶ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
         *
         * ・ｽ・ｽ・ｽ・ｽ・ｽO・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽ・ｽV・ｽKSpline・ｽ・ｽ・ｽn・ｽ・ｽ・ｽﾉ、
         * ・ｽ・ｽ・ｽ・ｽController・ｽ・ｽ・ｽ・ｽ・ｽ・ｽSubject・ｽﾖ通知・ｽ・ｽ・ｽ・ｽB
         */
        _attachEvent = new AttachEvent(
            _view,
            _model,
            _onLongJumped,
            _onNewOrbitAttached
        );

        /*
         * Spline・ｽ・ｽﾌ移難ｿｽ・ｽA・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽA・ｽz・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ・ｽ・ｽMover・ｽｶ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
         */
        _mover = new PlayerSplineMover(
            _view,
            _model,
            _attachEvent,
            _playerExternalFacade,
            _spawnOverlapResolver
        );
    }


    /* =========================================================
     * ・ｽ・ｽ・ｽﾍイ・ｽx・ｽ・ｽ・ｽg・ｽﾌ登・ｽ^
     * ========================================================= */

    /// <summary>
    /// Player・ｽ・ｽ・ｽ・ｽﾉ関ゑｿｽ・ｽ・ｽ・ｽ・ｽﾍイ・ｽx・ｽ・ｽ・ｽg・ｽ・ｽo・ｽ^・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽEMove・ｽ・ｽ・ｽ・ｽ
    /// ・ｽEJumpPressed・ｽ・ｽ・ｽ・ｽ
    /// ・ｽEJumpReleased・ｽ・ｽ・ｽ・ｽ
    ///
    /// ・ｽ・ｽ・ｽﾍゑｿｽ・ｽﾌゑｿｽ・ｽﾌの鯉ｿｽ・ｽo・ｽﾍ外・ｽ・ｽFacade・ｽ・ｽ・ｽﾅ行・ｽ・ｽ・ｽA
    /// ・ｽ・ｽ・ｽﾌク・ｽ・ｽ・ｽX・ｽﾅは難ｿｽ・ｽﾍ鯉ｿｽﾌ擾ｿｽﾔ遷・ｽﾚゑｿｽ・ｽﾇ暦ｿｽ・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void RegisterInputSubscriptions()
    {
        /*
         * Move・ｽ・ｽ・ｽﾍ趣ｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽ・ｽﾍ趣ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽｽ転・ｽ・ｽ・ｽ・ｽB
         * ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾉ会ｿｽ・ｽ・ｽ・ｽ黷ｽ・ｽ鼾・ｿｽﾍチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽL・ｽ・ｽ・ｽ・ｽ・ｽZ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
         */
        _playerExternalFacade.MoveSubscribe(() =>
        {
            // GameOver・ｽ・ｽ・ｽﾍ難ｿｽ・ｽﾍゑｿｽ・ｽｯ付・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
            if (_playerStateMachine.CurrentState is GameOverState)
                return;

            /*
             * ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽ・ｽMove・ｽ・ｽ・ｽﾍゑｿｽ・ｽs・ｽ・ｽ黷ｽ・ｽ鼾・ｿｽA
             * ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾌ費ｿｽ・ｽ]・ｽﾅはなゑｿｽ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽL・ｽ・ｽ・ｽ・ｽ・ｽZ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾄ茨ｿｽ・ｽ・ｽ・ｽB
             */
            if (_playerStateMachine.CurrentState is ChargeState)
            {
                CancelChargeAndReturnMove();
                return;
            }

            // ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾆ費ｿｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽﾘゑｿｽﾖゑｿｽ・ｽ・ｽB
            _model.Clockwise = !_model.Clockwise;

            // ・ｽK・ｽv・ｽﾅゑｿｽ・ｽ・ｽﾎ会ｿｽ]・ｽ・ｽ・ｽ・ｽUI・ｽﾌ費ｿｽ・ｽ]・ｽ・ｽ・ｽo・ｽ・ｽ・ｽﾄび出・ｽ・ｽ・ｽB
            //_view.FlipRotateUI();
        });

        /*
         * ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾍを離ゑｿｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾉボ・ｽ^・ｽ・ｽ・ｽ｣ゑｿｽ・ｽ・ｽ・ｽﾆゑｿｽJumpState・ｽﾖ遷・ｽﾚゑｿｽ・ｽ・ｽB
         */
        _playerExternalFacade.JumpReleasedSubscribe(() =>
        {
            // ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽ・ｽﾈゑｿｽA・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽo・ｽR・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽﾟ厄ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
            if (_playerStateMachine.CurrentState is MoveState)
                return;

            // ・ｽ・ｽ・ｽﾉジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾈゑｿｽﾄ度・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽJ・ｽn・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
            if (_playerStateMachine.CurrentState is JumpState)
                return;

            // GameOver・ｽ・ｽ・ｽﾍ難ｿｽ・ｽﾍゑｿｽ・ｽｯ付・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
            if (_playerStateMachine.CurrentState is GameOverState)
                return;

            // ・ｽ・ｽ・ｽﾝのチ・ｽ・ｽ・ｽ[・ｽW・ｽﾊゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽp・ｽ・ｽ・ｽﾅジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽﾔへ移行・ｽ・ｽ・ｽ・ｽB
            _playerStateMachine.ChangeState(PlayerStateKey.Jump);
        });

        /*
         * ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾍゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽ・ｽﾉ会ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽChargeState・ｽﾖ遷・ｽﾚゑｿｽ・ｽ・ｽB
         */
        _playerExternalFacade.JumpPressedSubscribe(() =>
        {
            // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾍ新・ｽ・ｽ・ｽ・ｽ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽJ・ｽn・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
            if (_playerStateMachine.CurrentState is JumpState)
                return;

            // GameOver・ｽ・ｽ・ｽﾍ難ｿｽ・ｽﾍゑｿｽ・ｽｯ付・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
            if (_playerStateMachine.CurrentState is GameOverState)
                return;

            // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽﾔへ移行・ｽ・ｽ・ｽ・ｽB
            _playerStateMachine.ChangeState(PlayerStateKey.Charge);
        });
    }


    /* =========================================================
     * Player・ｽﾚ触・ｽC・ｽx・ｽ・ｽ・ｽg・ｽﾌ登・ｽ^
     * ========================================================= */

    /// <summary>
    /// Player・ｽ・ｽ・ｽG・ｽe・ｽA・ｽ・ｽ・ｽ[・ｽU・ｽ[・ｽA・ｽ・ｽQ・ｽ・ｽ・ｽﾈどへ接触・ｽ・ｽ・ｽ・ｽ・ｽﾛゑｿｽ
    /// ・ｽO・ｽ・ｽ・ｽC・ｽx・ｽ・ｽ・ｽg・ｽ・ｽ・ｽw・ｽﾇゑｿｽ・ｽ・ｽB
    ///
    /// ・ｽK・ｽ[・ｽh・ｽﾂ能・ｽﾈ攻・ｽ・ｽ・ｽﾌ場合・ｽ・ｽGuardCount・ｽ・ｽ・ｽ・ｽ・ｽ・ｵ・ｽA
    /// ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽGameOverState・ｽﾖ移行・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void RegisterPlayerSubscriptions()
    {
        /*
         * ・ｽG・ｽe・ｽﾉ接触・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ鼾・ｿｽﾍ攻・ｽ・ｽ・ｽｳ鯉ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
         * ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽﾎゲ・ｽ[・ｽ・ｽ・ｽI・ｽ[・ｽo・ｽ[・ｽﾉなゑｿｽB
         */
        _playerExternalFacade
            .OnPlayerHitByEnemyBullet
            .Subscribe(_ =>
            {
                if (TryGuardEnemyBullet())
                    return;

                _playerStateMachine.ChangeState(
                    PlayerStateKey.GameOver
                );
            })
            .AddTo(_playerSubscriptions);

        /*
         * ・ｽ・ｽ・ｽ[・ｽU・ｽ[・ｽr・ｽ[・ｽ・ｽ・ｽﾉ接触・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽ・ｽ・ｽﾝの仕・ｽl・ｽﾅは・ｿｽ・ｽ[・ｽU・ｽ[・ｽﾍガ・ｽ[・ｽh・ｽﾎ象外・ｽﾅ、
         * ・ｽﾚ触・ｽ・ｽ・ｽ・ｽﾆ托ｿｽ・ｽ・ｽ・ｽﾉゲ・ｽ[・ｽ・ｽ・ｽI・ｽ[・ｽo・ｽ[・ｽﾉなゑｿｽB
         */
        _playerExternalFacade
            .OnPlayerHitByLaserBeam
            .Subscribe(_ =>
            {
                _playerStateMachine.ChangeState(
                    PlayerStateKey.GameOver
                );
            })
            .AddTo(_playerSubscriptions);

        /*
         * ・ｽ・ｽQ・ｽ・ｽ・ｽﾉ接触・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ・ｽ1・ｽｪ擾ｿｽ・ｽ・ｵ・ｽﾄ防・ｽ・ｽ・ｽB
         * ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽﾎゲ・ｽ[・ｽ・ｽ・ｽI・ｽ[・ｽo・ｽ[・ｽﾉなゑｿｽB
         */
        _playerExternalFacade
            .OnPlayerHitObstacle
            .Subscribe(_ =>
            {
                if (TryGuardObstacle())
                    return;

                _playerStateMachine.ChangeState(
                    PlayerStateKey.GameOver
                );
            })
            .AddTo(_playerSubscriptions);

        /*
         * ・ｽu・ｽ・ｽ・ｽb・ｽN・ｽz・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾖ侵・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾅゲ・ｽ[・ｽ・ｽ・ｽI・ｽ[・ｽo・ｽ[・ｽﾉなゑｿｽB
         */
        _playerExternalFacade
            .OnPlayerEnteredBlackHole
            .Subscribe(_ =>
            {
                _playerStateMachine.ChangeState(
                    PlayerStateKey.GameOver
                );
            })
            .AddTo(_playerSubscriptions);

        /*
         * ・ｽX・ｽe・ｽ[・ｽW・ｽO・ｽ・ｽ・ｽﾌ撰ｿｽ・ｽ・ｽ・ｽﾍ囲を超ゑｿｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽ・ｽO・ｽﾖ出・ｽ・ｽ・ｽ・ｽ・ｽﾟゲ・ｽ[・ｽ・ｽ・ｽI・ｽ[・ｽo・ｽ[・ｽﾉゑｿｽ・ｽ・ｽB
         */
        _playerExternalFacade
            .OnPlayerExitedOuterLimit
            .Subscribe(_ =>
            {
                _playerStateMachine.ChangeState(
                    PlayerStateKey.GameOver
                );
            })
            .AddTo(_playerSubscriptions);

        /*
         * Player・ｽ・ｽ・ｽG・ｽ{・ｽﾌへ接触・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
         *
         * ・ｽ・ｽ・ｽﾝのコ・ｽ[・ｽh・ｽﾅはガ・ｽ[・ｽh・ｽ・ｽﾔの確・ｽF・ｽ・ｽ・ｽs・ｽ墲ｸ・ｽA
         * ・ｽﾚ触・ｽ・ｽ・ｽ・ｽ・ｽG・ｽ・ｽ・ｽ・ｽﾉ鯉ｿｽ・ｽj・ｽ・ｽ・ｽ・ｽB
         *
         * ・ｽd・ｽl・ｽﾆゑｿｽ・ｽﾄ「・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾌみ敵・ｽ・ｽ|・ｽ・ｽ・ｽ・ｽv・ｽﾌでゑｿｽ・ｽ・ｽﾎ、
         * ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽGuardCount・ｽ・ｽChargeLevel・ｽﾌ確・ｽF・ｽ・ｽ・ｽK・ｽv・ｽB
         */
        _playerExternalFacade
            .OnPlayerTouchedEnemy
            .Subscribe(enemyHandle =>
            {
                _playerExternalFacade.DefeatEnemy(enemyHandle);
            })
            .AddTo(_playerSubscriptions);
    }


    /* =========================================================
     * ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽL・ｽ・ｽ・ｽ・ｽ・ｽZ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽL・ｽ・ｽ・ｽ・ｽ・ｽZ・ｽ・ｽ・ｽ・ｽ・ｽA・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽﾔへ戻ゑｿｽ・ｽB
    ///
    /// ・ｽE・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽ・ｽ・ｽZ・ｽb・ｽg
    /// ・ｽE・ｽ`・ｽ・ｽ・ｽ[・ｽWSE・ｽ・ｽ・ｽ~
    /// ・ｽE・ｽﾚ難ｿｽ・ｽ・ｽ・ｽx・ｽﾆジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽx・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽl・ｽﾖ戻ゑｿｽ
    /// ・ｽE・ｽI・ｽ[・ｽ・ｽ・ｽ・ｽﾊ擾ｿｽF・ｽﾖ戻ゑｿｽ
    /// ・ｽEMoveState・ｽﾖ遷・ｽ・ｽ
    /// </summary>
    private void CancelChargeAndReturnMove()
    {
        // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽﾉゑｿｽ・ｽ・ｽﾄ難ｿｽ・ｽ・ｽ・ｽ・ｽ\・ｽ閧ｾ・ｽ・ｽ・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽj・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
        _model.GuardCount = 0;

        // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾌ・ｿｽ・ｽ[・ｽvSE・ｽ・ｽ・ｽ~・ｽ・ｽ・ｽ・ｽB
        _playerExternalFacade.StopLoopSE();

        // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽﾉゑｿｽ・ｽ・ｽﾄ変会ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽx・ｽ・ｽﾊ擾ｿｽl・ｽﾖ戻ゑｿｽ・ｽB
        _model.InitializeMoveSpeed();
        _model.InitializeJumpSpeed();

        // Player・ｽﾌオ・ｽ[・ｽ・ｽ・ｽ・ｽﾊ擾ｿｽ・ｽﾔへ戻ゑｿｽ・ｽB
        _view.SetAuraColor(ChargeLevel.Normal);

        // ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽﾔへ戻ゑｿｽB
        _playerStateMachine.ChangeState(PlayerStateKey.Move);
    }


    /* =========================================================
     * ・ｽX・ｽe・ｽ[・ｽg・ｽ}・ｽV・ｽ・ｽ・ｽﾌ設抵ｿｽE・ｽX・ｽV
     * ========================================================= */

    /// <summary>
    /// PlayerController・ｽ・ｽ・ｽ・ｽ・ｽp・ｽ・ｽ・ｽ・ｽPlayerStateMachine・ｽ・ｽﾝ定す・ｽ・ｽB
    ///
    /// StateMachine・ｽ・ｽ・ｽﾌ撰ｿｽ・ｽ・ｽ・ｽ・ｽﾉ外・ｽ・ｽ・ｽ・ｽ・ｽ迺搾ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    /// <param name="playerStateMachine">
    /// Player・ｽﾌ擾ｿｽﾔゑｿｽ・ｽﾇ暦ｿｽ・ｽ・ｽ・ｽ・ｽX・ｽe・ｽ[・ｽg・ｽ}・ｽV・ｽ・ｽ・ｽB
    /// </param>
    public void SetPlayerStateMachine(
        PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
    }

    /// <summary>
    /// VContainer・ｽ・ｽITickable・ｽﾉゑｿｽ・ｽ・ｽﾄ厄ｿｽ・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽﾄばゑｿｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽﾝゑｿｽPlayerState・ｽ・ｽTick・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽs・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void Tick()
    {
        // StateMachine・ｽ・ｽ・ｽﾝ定さ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ鼾・ｿｽﾌみ更・ｽV・ｽ・ｽ・ｽ・ｽB
        _playerStateMachine?.Tick();
    }


    /* =========================================================
     * Player・ｽ・ｽ・ｽ・ｽ・ｽz・ｽu
     * ========================================================= */

    /// <summary>
    /// Player・ｽ・ｽ・ｽQ・ｽ[・ｽ・ｽ・ｽJ・ｽn・ｽpSpline・ｽﾌ始・ｽ_・ｽﾖ配・ｽu・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽﾛの茨ｿｽ・ｽS・ｽﾊ置・ｽ・ｽ・ｽ・ｽ・ｽﾆ搾ｿｽ・ｽW・ｽ・ｽ・ｽf・ｽ・ｽ
    /// PlayerSplineMover・ｽ・ｽ・ｽﾅ行・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void SetPlayer()
    {
        _mover.SetPlayer(
            _playerExternalFacade.StartSpline,
            0f
        );
    }


    /* =========================================================
     * MoveState・ｽp・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽﾔゑｿｽ・ｽJ・ｽn・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽMoveState・ｽ・ｽEnter・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾄばゑｿｽ・ｽB
    /// </summary>
    public void StartMove()
    {
        // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽK・ｽC・ｽh・ｽ・ｽ・ｽ\・ｽ・ｽ・ｽﾉゑｿｽ・ｽ・ｽB
        _view.HideJumpNormalGuide();

        // Player・ｽﾌオ・ｽ[・ｽ・ｽ・ｽ・ｽﾊ擾ｿｽF・ｽﾖ戻ゑｿｽ・ｽB
        _view.SetAuraColor(ChargeLevel.Normal);

        // ・ｽ・ｽ・ｽﾝゑｿｽSpline・ｽ・ｽ・ｽ・ｽ・ｽg・ｽ・ｽ・ｽ・ｽMover・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
        _mover.InitializeMove();

        // ・ｽﾚ難ｿｽ・ｽ・ｽ・ｽx・ｽ・ｽﾊ擾ｿｽl・ｽﾖ戻ゑｿｽ・ｽB
        _model.InitializeMoveSpeed();

        // ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽJ・ｽn・ｽ・ｽ・ｽﾍガ・ｽ[・ｽh・ｽ狽・ｽ・ｽ・ｽ・ｽZ・ｽb・ｽg・ｽ・ｽ・ｽ・ｽB
        _model.GuardCount = 0;
    }

    /// <summary>
    /// ・ｽﾊ擾ｿｽﾚ難ｿｽ・ｽ・ｽ・ｽ・ｽ1・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
    ///
    /// Player・ｽ・ｽ・ｽ・ｽ・ｽﾝゑｿｽSpline・ｽﾉ会ｿｽ・ｽ・ｽ・ｽﾄ移難ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void TickMove()
    {
        _mover.Tick(Time.deltaTime);
    }


    /* =========================================================
     * JumpState・ｽp・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽJ・ｽn・ｽ・ｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽﾝのチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽ・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽm・ｽ閧ｵ・ｽA
    /// Spline・ｽﾌ外・ｽ・ｽ・ｽ・ｽ・ｽ@・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾖジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽJ・ｽn・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void StartJump()
    {
        /*
         * ChargeLevel・ｽﾉ会ｿｽ・ｽ・ｽ・ｽﾄ、
         * ・ｽ・ｽ・ｽ・ｽﾌジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾉ使・ｽp・ｽﾅゑｿｽ・ｽ・ｽK・ｽ[・ｽh・ｽ狽・ｽﾝ定す・ｽ・ｽB
         */
        RefreshGuardCount();

        // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾉ再撰ｿｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ[・ｽvSE・ｽ・ｽ・ｽ~・ｽ・ｽ・ｽ・ｽB
        _playerExternalFacade.StopLoopSE();

        // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽJ・ｽn・ｽ・ｽﾍ包ｿｽ・ｽ・ｽ・ｽK・ｽC・ｽh・ｽ・ｽ・ｽB・ｽ・ｽ・ｽB
        _view.HideJumpNormalGuide();

        // PlayerSplineMover・ｽ・ｽ・ｽﾅジ・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽJ・ｽn・ｽ・ｽ・ｽ・ｽB
        _mover.StartJump();
    }

    /// <summary>
    /// ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ1・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
    /// </summary>
    /// <returns>
    /// ・ｽﾊゑｿｽSpline・ｽﾖ接触・ｽ・ｽ・ｽA・ｽz・ｽ・ｽ・ｽ・ｽ・ｽJ・ｽn・ｽ・ｽ・ｽ・ｽ・ｽ鼾・ｿｽ・ｽtrue・ｽB
    /// ・ｽﾚ触・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ鼾・ｿｽ・ｽfalse・ｽB
    /// </returns>
    public bool TickJump()
    {
        return _mover.TickJump(Time.deltaTime);
    }


    /* =========================================================
     * ChargeState・ｽp・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽJ・ｽn・ｽ・ｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽChargeState・ｽ・ｽEnter・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾄばゑｿｽ・ｽB
    /// </summary>
    public void StartCharge()
    {
        // ・ｽ・ｽ・ｽﾝのチ・ｽ・ｽ・ｽ[・ｽW・ｽo・ｽﾟ趣ｿｽ・ｽﾔゑｿｽ0・ｽﾖ戻ゑｿｽ・ｽB
        _model.CurrentChargeDuaration = 0f;

        /*
         * ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽJ・ｽn・ｽ・ｽ・ｽ_・ｽﾅはまゑｿｽ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽﾟ、
         * ・ｽK・ｽ[・ｽh・ｽ狽・ｽ0・ｽﾉ戻ゑｿｽ・ｽB
         *
         * ・ｽ・ｽ・ｽﾛゑｿｽGuardCount・ｽ・ｽStartJump・ｽ・ｽ・ｽﾉ確・ｽ閧ｷ・ｽ・ｽB
         */
        _model.GuardCount = 0;

        // ・ｽO・ｽ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽ・ｽﾊ擾ｿｽ・ｽﾔへ戻ゑｿｽ・ｽB
        _previousChargeLevel = ChargeLevel.Normal;

        // ・ｽO・ｽ・ｽﾌ・ｿｽ・ｽ[・ｽvSE・ｽ・ｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽﾂ能・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ驍ｽ・ｽﾟ抵ｿｽ~・ｽ・ｽ・ｽ・ｽB
        _playerExternalFacade.StopLoopSE();
    }

    /// <summary>
    /// ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽ・ｽ1・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽﾌ擾ｿｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽE・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾔの会ｿｽ・ｽZ
    /// ・ｽE・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽx・ｽﾌ更・ｽV
    /// ・ｽE・ｽ・ｽ・ｽｬ度・ｽﾌ更・ｽV
    /// ・ｽE・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽﾏ会ｿｽ・ｽ・ｽ・ｽ・ｽSE・ｽﾘゑｿｽﾖゑｿｽ
    /// ・ｽE・ｽI・ｽ[・ｽ・ｽ・ｽF・ｽﾌ更・ｽV
    /// ・ｽE・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽK・ｽC・ｽh・ｽﾌ表・ｽ・ｽ
    /// </summary>
    public void TickCharge()
    {
        // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽo・ｽﾟ趣ｿｽ・ｽﾔゑｿｽ・ｽ・ｽ・ｽZ・ｽ・ｽ・ｽ・ｽB
        _model.CurrentChargeDuaration += Time.deltaTime;

        /*
         * ・ｽ・ｽ・ｽﾝのチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽﾔに会ｿｽ・ｽ・ｽ・ｽﾄ、
         * ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽx・ｽﾆ移難ｿｽ・ｽ・ｽ・ｽx・ｽ・ｽ・ｽX・ｽV・ｽ・ｽ・ｽ・ｽB
         */
        _model.ApplyChargeJumpSpeed();
        _model.ApplyChargeMoveSpeed();

        // ・ｽX・ｽV・ｽ・ｽﾌチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽ・ｽ・ｽ謫ｾ・ｽ・ｽ・ｽ・ｽB
        ChargeLevel currentLevel =
            _model.CurrentChargeLevel;

        /*
         * ChargeLevel・ｽ・ｽ・ｽO・ｽt・ｽ・ｽ・ｽ[・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾏ会ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽA
         * ・ｽ`・ｽ・ｽ・ｽ[・ｽWSE・ｽ・ｽﾘゑｿｽﾖゑｿｽ・ｽ・ｽB
         */
        if (currentLevel != _previousChargeLevel)
        {
            switch (currentLevel)
            {
                case ChargeLevel.Charge1:

                    // ・ｽﾈ前・ｽﾌ・ｿｽ・ｽ[・ｽvSE・ｽ・ｽ・ｽ~・ｽ・ｽ・ｽ・ｽB
                    _playerExternalFacade.StopLoopSE();

                    // Charge1・ｽpSE・ｽ・ｽ・ｽ・ｽ・ｽ[・ｽv・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
                    _playerExternalFacade.StartLoopSE(
                        SEType.Charge1
                    );
                    break;

                case ChargeLevel.Charge2:

                    // Charge1・ｽpSE・ｽﾈどゑｿｽ・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽﾈゑｿｽ・ｽ~・ｽ・ｽ・ｽ・ｽB
                    _playerExternalFacade.StopLoopSE();

                    // Charge2・ｽpSE・ｽ・ｽ・ｽ・ｽ・ｽ[・ｽv・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
                    _playerExternalFacade.StartLoopSE(
                        SEType.Charge2
                    );
                    break;

                case ChargeLevel.Normal:
                default:

                    // ・ｽﾊ擾ｿｽ・ｽﾔではチ・ｽ・ｽ・ｽ[・ｽWSE・ｽ・ｽ・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
                    _playerExternalFacade.StopLoopSE();
                    break;
            }

            // ・ｽ・ｽ・ｽ・ｽ・ｽr・ｽp・ｽﾉ鯉ｿｽ・ｽﾝ・ｿｽ・ｽx・ｽ・ｽ・ｽ・ｽﾛ托ｿｽ・ｽ・ｽ・ｽ・ｽB
            _previousChargeLevel = currentLevel;
        }

        // ・ｽ・ｽ・ｽﾝゑｿｽChargeLevel・ｽﾉ対会ｿｽ・ｽ・ｽ・ｽ・ｽI・ｽ[・ｽ・ｽ・ｽF・ｽﾖ変更・ｽ・ｽ・ｽ・ｽB
        _view.SetAuraColor(currentLevel);

        /*
         * ・ｽ・ｽ・ｽﾝ地・ｽ_・ｽ・ｽSpline・ｽO・ｽ・ｽ・ｽ・ｽ・ｽ@・ｽ・ｽ・ｽ・ｽ・ｽ謫ｾ・ｽ・ｽ・ｽA
         * ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ\・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽﾄガ・ｽC・ｽh・ｽ・ｽ\・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
         */
        _view.ShowJumpNormalGuide(
            _mover.GetOuterNormal()
        );
    }


    /* =========================================================
     * ・ｽK・ｽ[・ｽh・ｽ狽ﾌ確・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽ・ｽ・ｽﾝのチ・ｽ・ｽ・ｽ[・ｽW・ｽ・ｽ・ｽx・ｽ・ｽ・ｽ・ｽ・ｽ・ｽA
    /// ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽJ・ｽn・ｽ・ｽ・ｽﾉ使・ｽp・ｽﾅゑｿｽ・ｽ・ｽK・ｽ[・ｽh・ｽ狽・ｽﾝ定す・ｽ・ｽB
    ///
    /// Normal  : 0・ｽ・ｽ
    /// Charge1 : 1・ｽ・ｽ
    /// Charge2 : 2・ｽ・ｽ
    /// </summary>
    private void RefreshGuardCount()
    {
        switch (_model.CurrentChargeLevel)
        {
            case ChargeLevel.Charge1:

                _model.GuardCount = 1;
                break;

            case ChargeLevel.Charge2:

                _model.GuardCount = 2;
                break;

            case ChargeLevel.Normal:
            default:

                _model.GuardCount = 0;
                break;
        }
    }


    /* =========================================================
     * ・ｽG・ｽe・ｽﾉ対ゑｿｽ・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽG・ｽe・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽ驍ｩ・ｽ・ｽ・ｽ閧ｵ・ｽA
    /// ・ｽﾂ能・ｽﾈ場合・ｽ・ｽGuardCount・ｽ・ｽ1・ｽ・ｽ・ｽ・ｷ・ｽ・ｽB
    /// </summary>
    /// <returns>
    /// ・ｽG・ｽe・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ鼾・ｿｽ・ｽtrue・ｽB
    /// ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ鼾・ｿｽ・ｽfalse・ｽB
    /// </returns>
    private bool TryGuardEnemyBullet()
    {
        // ・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽﾎ防・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
        if (_model.GuardCount <= 0)
            return false;

        // ・ｽG・ｽe1・ｽｪのガ・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ・ｷ・ｽ・ｽB
        _model.GuardCount--;

        /*
         * ・ｽK・ｽ[・ｽh・ｽ・ｽﾌ残・ｽ・ｽ狽ﾉ会ｿｽ・ｽ・ｽ・ｽﾄ、
         * Player・ｽﾌ鯉ｿｽ・ｽ・ｽ・ｽﾚや速・ｽx・ｽ・ｽﾔゑｿｽ・ｽX・ｽV・ｽ・ｽ・ｽ・ｽB
         */
        switch (_model.GuardCount)
        {
            case 1:

                /*
                 * Charge2・ｽ・ｽ・ｽ・ｽ1・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ・ｵ・ｽA
                 * Charge1・ｽ・ｽ・ｽ・ｽ・ｽﾌ擾ｿｽﾔになゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽﾆゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽﾚへ費ｿｽ・ｽf・ｽ・ｽ・ｽ・ｽB
                 *
                 * ・ｽ・ｽ・ｽﾌ敵・ｽe・ｽp・ｽ・ｽ・ｽ・ｽ・ｽﾅは托ｿｽ・ｽx・ｽﾏ更・ｽ・ｽ・ｽs・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽB
                 */
                _view.SetAuraColor(ChargeLevel.Charge1);
                break;

            case 0:

                // ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽﾗて使・ｽ・ｽ・ｽﾘゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽﾟ通擾ｿｽF・ｽﾖ戻ゑｿｽ・ｽB
                _view.SetAuraColor(ChargeLevel.Normal);

                // ・ｽ・ｽ・ｽx・ｽ・ｽﾊ擾ｿｽ・ｽﾔへ戻ゑｿｽ・ｽB
                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        // ・ｽG・ｽe・ｽｳ擾ｿｽﾉガ・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽB
        return true;
    }


    /* =========================================================
     * ・ｽ・ｽQ・ｽ・ｽ・ｽﾉ対ゑｿｽ・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// ・ｽ・ｽQ・ｽ・ｽ・ｽﾖの接触・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽ驍ｩ・ｽ・ｽ・ｽ閧ｵ・ｽA
    /// ・ｽﾂ能・ｽﾈ場合・ｽ・ｽGuardCount・ｽ・ｽ1・ｽ・ｽ・ｽ・ｷ・ｽ・ｽB
    /// </summary>
    /// <returns>
    /// ・ｽ・ｽQ・ｽ・ｽ・ｽ・ｽ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ鼾・ｿｽ・ｽtrue・ｽB
    /// ・ｽK・ｽ[・ｽh・ｽﾅゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ鼾・ｿｽ・ｽfalse・ｽB
    /// </returns>
    private bool TryGuardObstacle()
    {
        // ・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽﾎ防・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
        if (_model.GuardCount <= 0)
            return false;

        // ・ｽ・ｽQ・ｽ・ｽ1・ｽｪのガ・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ・ｷ・ｽ・ｽB
        _model.GuardCount--;

        switch (_model.GuardCount)
        {
            case 1:

                /*
                 * Charge2・ｽ・ｽﾔゑｿｽ・ｽ・ｽ1・ｽｪゑｿｽ・ｽ・ｽ・ｽ・ｵ・ｽ・ｽ・ｽ・ｽ・ｽﾟ、
                 * Charge1・ｽ・ｽ・ｽ・ｽ・ｽﾌ鯉ｿｽ・ｽ・ｽ・ｽﾚと托ｿｽ・ｽx・ｽﾖ変更・ｽ・ｽ・ｽ・ｽB
                 */
                _view.SetAuraColor(ChargeLevel.Charge1);

                _model.SetCharge1JumpSpeed();
                _model.SetCharge1MoveSpeed();
                break;

            case 0:

                // ・ｽ・ｽ・ｽﾗてのガ・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ・ｵ・ｽ・ｽ・ｽ・ｽ・ｽﾟ通擾ｿｽ・ｽﾔへ戻ゑｿｽ・ｽB
                _view.SetAuraColor(ChargeLevel.Normal);

                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        return true;
    }


    /* =========================================================
     * ・ｽ・ｽ・ｽE・ｽG・ｽ{・ｽﾌ接触・ｽ・ｽ・ｽﾌガ・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /*
     * ・ｽ・ｽ・ｽﾝは使・ｽp・ｽ・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽB
     *
     * ・ｽG・ｽ{・ｽﾌへ接触・ｽ・ｽ・ｽ・ｽ・ｽﾛゑｿｽGuardCount・ｽ・ｽ・ｽ・ｽ・ｽ・ｵ・ｽA
     * Charge2・ｽ・ｽ・ｽ・ｽCharge1・ｽACharge1・ｽ・ｽ・ｽ・ｽNormal・ｽ・ｽ
     * ・ｽi・ｽK・ｽI・ｽﾉ移行・ｽ・ｽ・ｽ・ｽz・ｽ閧ｾ・ｽ・ｽ・ｽ・ｽ・ｽR・ｽ[・ｽh・ｽB
     *
     * ・ｽ・ｽ・ｽﾝゑｿｽRegisterPlayerSubscriptions・ｽﾅは、
     * ・ｽG・ｽﾖ接触・ｽ・ｽ・ｽ・ｽﾆ厄ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽDefeatEnemy・ｽ・ｽ・ｽﾄび出・ｽ・ｽ・ｽﾄゑｿｽ・ｽ・ｽB
     */

    //private bool TryGuardEnemyContact()
    //{
    //    // ・ｽK・ｽ[・ｽh・ｽ狽・ｽ・ｽc・ｽ・ｽ・ｽﾄゑｿｽ・ｽﾈゑｿｽ・ｽ・ｽﾎ防・ｽ・ｽ・ｽﾈゑｿｽ・ｽB
    //    if (_model.GuardCount <= 0)
    //        return false;
    //
    //    // ・ｽG・ｽﾚ触1・ｽｪのガ・ｽ[・ｽh・ｽ・ｽ・ｽ・ｽ・ｽ・ｷ・ｽ・ｽB
    //    _model.GuardCount--;
    //
    //    switch (_model.GuardCount)
    //    {
    //        case 1:
    //
    //            // Charge2・ｽ・ｽ・ｽ・ｽCharge1・ｽ・ｽ・ｽ・ｽ・ｽﾌ擾ｿｽﾔへ変更・ｽ・ｽ・ｽ・ｽB
    //            _view.SetAuraColor(ChargeLevel.Charge1);
    //            _model.SetCharge1JumpSpeed();
    //            _model.SetCharge1MoveSpeed();
    //            break;
    //
    //        case 0:
    //
    //            // ・ｽK・ｽ[・ｽh・ｽ・ｽ・ｽg・ｽ・ｽ・ｽﾘゑｿｽ・ｽ・ｽ・ｽ・ｽ・ｽﾟ通擾ｿｽ・ｽﾔへ戻ゑｿｽ・ｽB
    //            _view.SetAuraColor(ChargeLevel.Normal);
    //            _model.InitializeMoveSpeed();
    //            _model.InitializeJumpSpeed();
    //            break;
    //    }
    //
    //    return true;
    //}


    /* =========================================================
     * ・ｽ・ｽ・ｽS・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// Player・ｽ・ｽ・ｽS・ｽ・ｽ・ｽﾌ具ｿｽ・ｽﾊ擾ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽs・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽ・ｽ・ｽGameOverState・ｽﾌ開・ｽn・ｽ・ｽ・ｽﾈどゑｿｽ・ｽ・ｽﾄばゑｿｽ・ｽB
    ///
    /// ・ｽE・ｽ・ｽ・ｽS・ｽC・ｽx・ｽ・ｽ・ｽg・ｽ・ｽﾊ知
    /// ・ｽE・ｽ・ｽ・ｽSSE・ｽ・ｽ・ｽﾄ撰ｿｽ
    /// ・ｽE・ｽ・ｽ・ｽS・ｽG・ｽt・ｽF・ｽN・ｽg・ｽ・ｽ・ｽﾄ撰ｿｽ
    /// ・ｽE・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ・ｽ・ｽ・ｽ・ｽK・ｽC・ｽh・ｽ・ｽ・ｽ\・ｽ・ｽ
    /// ・ｽE・ｽ・ｽ・ｽ[・ｽvSE・ｽ・ｽ・ｽ~
    /// </summary>
    public void Dead()
    {
        // ・ｽO・ｽ・ｽ・ｽ・ｽPlayer・ｽ・ｽ・ｽS・ｽ・ｽﾊ知・ｽ・ｽ・ｽ・ｽB
        _onPlayerDead.OnNext(Unit.Default);

        // Player・ｽ・ｽ・ｽSSE・ｽ・ｽ・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
        _playerExternalFacade.PlaySE(
            SEType.PlayerDead
        );

        /*
         * PlayerView・ｽﾌ趣ｿｽ・ｽS・ｽG・ｽt・ｽF・ｽN・ｽg・ｽ・ｽｯ奇ｿｽ・ｽﾄ撰ｿｽ・ｽ・ｽ・ｽ・ｽB
         *
         * Forget・ｽﾉゑｿｽ闃ｮ・ｽ・ｽ・ｽﾒゑｿｽ・ｽﾍ行・ｽ・ｽﾈゑｿｽ・ｽB
         * ・ｽ・ｽO・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽK・ｽv・ｽﾈ場合・ｽ・ｽForget・ｽﾌ茨ｿｽ・ｽ・ｽ・ｽﾉ抵ｿｽ・ｽﾓゑｿｽ・ｽ・ｽB
         */
        _view.PlayDeadEffect().Forget();

        // ・ｽW・ｽ・ｽ・ｽ・ｽ・ｽv・ｽ\・ｽ・ｽ・ｽ・ｽ・ｽ・ｽﾌガ・ｽC・ｽh・ｽ・ｽ・ｽB・ｽ・ｽ・ｽB
        _view.HideJumpNormalGuide();

        // ・ｽ`・ｽ・ｽ・ｽ[・ｽW・ｽﾈどの・ｿｽ・ｽ[・ｽvSE・ｽ・ｽ・ｽ~・ｽ・ｽ・ｽ・ｽB
        _playerExternalFacade.StopLoopSE();
    }


    /* =========================================================
     * ・ｽj・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ
     * ========================================================= */

    /// <summary>
    /// PlayerController・ｽ・ｽ・ｽﾛ趣ｿｽ・ｽ・ｽ・ｽ・ｽUniRx・ｽw・ｽﾇゑｿｽSubject・ｽ・ｽj・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
    ///
    /// ・ｽV・ｽ[・ｽ・ｽ・ｽj・ｽ・ｽ・ｽ・ｽﾉイ・ｽx・ｽ・ｽ・ｽg・ｽ・ｽ・ｽﾄばれ続・ｽ・ｽ・ｽ驍ｱ・ｽﾆゑｿｽA
    /// ・ｽ・ｽ・ｽd・ｽw・ｽﾇ、・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ[・ｽN・ｽ・ｽh・ｽ~・ｽ・ｽ・ｽ・ｽB
    /// </summary>
    public void Dispose()
    {
        // ・ｽG・ｽe・ｽ・ｽ・ｽQ・ｽ・ｽ・ｽﾈどの外・ｽ・ｽ・ｽC・ｽx・ｽ・ｽ・ｽg・ｽw・ｽﾇゑｿｽ・ｽ鼕・ｿｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
        _playerSubscriptions.Dispose();

        // PlayerController・ｽ・ｽ・ｽ・ｽ・ｽL・ｽ・ｽ・ｽ・ｽSubject・ｽ・ｽj・ｽ・ｽ・ｽ・ｽ・ｽ・ｽB
        _onLongJumped.Dispose();
        _onNewOrbitAttached.Dispose();
        _onPlayerDead.Dispose();
    }
}

