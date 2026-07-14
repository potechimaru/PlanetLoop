
using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using VContainer.Unity;

/// <summary>
/// Player全体の進行を管理するコントローラー。
///
/// 主に以下の処理を担当する。
///
/// ・PlayerModel、PlayerView、PlayerSplineMoverの連携
/// ・PlayerStateMachineの状態更新
/// ・移動、ジャンプ、チャージ入力の受付
/// ・敵弾、レーザー、障害物などとの接触処理
/// ・チャージレベルに応じたガード回数の設定
/// ・Player死亡時の通知と演出
///
/// 実際のSpline上の移動やジャンプ座標の計算は
/// PlayerSplineMoverへ委譲している。
/// </summary>
public class PlayerController : ITickable, IDisposable
{
    /* =========================================================
     * Playerを構成する主要クラス
     * ========================================================= */

    /// <summary>
    /// Playerのゲーム上の状態や数値を保持するModel。
    ///
    /// 主に以下の情報を管理する。
    ///
    /// ・現在の移動速度
    /// ・現在のジャンプ速度
    /// ・周回方向
    /// ・チャージ時間
    /// ・チャージレベル
    /// ・残りガード回数
    /// </summary>
    private readonly PlayerModel _model;

    /// <summary>
    /// Playerの見た目やTransform操作を担当するView。
    ///
    /// 主に以下の処理に使用する。
    ///
    /// ・Player座標の更新
    /// ・オーラ色の変更
    /// ・ジャンプ方向ガイドの表示
    /// ・死亡エフェクトの再生
    /// </summary>
    private readonly PlayerView _view;

    /// <summary>
    /// PlayerのSpline上の移動、
    /// ジャンプ中の移動、
    /// 別Splineへの吸着処理を担当するクラス。
    /// </summary>
    private readonly PlayerSplineMover _mover;

    /// <summary>
    /// PlayerがSplineへ吸着した際のイベント処理を担当する。
    ///
    /// 主に以下の通知を行う。
    ///
    /// ・ロングジャンプ成立
    /// ・新しいSplineへの初回着地
    /// ・着地演出
    /// </summary>
    private readonly AttachEvent _attachEvent;

    /// <summary>
    /// Player機能の外側にあるシステムへアクセスするFacade。
    ///
    /// 主に以下の機能を利用する。
    ///
    /// ・入力イベントの購読
    /// ・Spline検索
    /// ・敵や障害物からの接触通知
    /// ・敵の撃破
    /// ・SEの再生と停止
    /// ・開始Splineの取得
    /// ・ブラックホールによるジャンプ方向の変更
    /// </summary>
    private readonly IPlayerExternalFacade _playerExternalFacade;

    /// <summary>
    /// Playerの初期配置地点にObstacleやPointObjectなどが
    /// 重なっていないかを判定するクラス。
    ///
    /// PlayerSplineMoverへ渡され、
    /// 安全な初期位置の検索に使用される。
    /// </summary>
    private readonly PlayerSpawnOverlapResolver _spawnOverlapResolver;


    /* =========================================================
     * ステートマシン
     * ========================================================= */

    /// <summary>
    /// Playerの現在状態を管理するステートマシン。
    ///
    /// 想定される状態は以下。
    ///
    /// ・MoveState
    /// ・ChargeState
    /// ・JumpState
    /// ・GameOverState
    /// </summary>
    private PlayerStateMachine _playerStateMachine;


    /* =========================================================
     * UniRx購読管理
     * ========================================================= */

    /// <summary>
    /// Playerに関する外部イベント購読をまとめて管理する。
    ///
    /// Dispose時に一括解除することで、
    /// 購読の残留や多重購読を防止する。
    /// </summary>
    private readonly CompositeDisposable _playerSubscriptions = new();


    /* =========================================================
     * PlayerControllerから外部へ公開するイベント
     * ========================================================= */

    /// <summary>
    /// ロングジャンプが成立したことを通知するSubject。
    ///
    /// 実際の判定はAttachEvent側で行われる。
    /// </summary>
    private Subject<Unit> _onLongJumped = new Subject<Unit>();

    /// <summary>
    /// ロングジャンプ成立時に通知されるObservable。
    ///
    /// 外部からOnNextを呼べないよう、
    /// IObservableとして公開している。
    /// </summary>
    public IObservable<Unit> OnLongJumped => _onLongJumped;

    /// <summary>
    /// 未訪問のSplineへ初めて吸着したことを通知するSubject。
    /// </summary>
    private Subject<Unit> _onNewOrbitAttached = new Subject<Unit>();

    /// <summary>
    /// 新しいSplineへ初めて吸着したときに通知されるObservable。
    /// </summary>
    public IObservable<Unit> OnNewOrbitAttached => _onNewOrbitAttached;

    /// <summary>
    /// Playerが死亡したことを通知するSubject。
    /// </summary>
    private Subject<Unit> _onPlayerDead = new Subject<Unit>();

    /// <summary>
    /// Player死亡時に通知されるObservable。
    /// </summary>
    public IObservable<Unit> OnPlayerDead => _onPlayerDead;


    /* =========================================================
     * チャージ状態管理
     * ========================================================= */

    /// <summary>
    /// 前フレーム時点のチャージレベル。
    ///
    /// 現在のチャージレベルと比較し、
    /// レベルが変化した瞬間だけSEを切り替えるために使用する。
    ///
    /// 毎フレームSEを再生し直すことを防いでいる。
    /// </summary>
    private ChargeLevel _previousChargeLevel = ChargeLevel.Normal;


    /* =========================================================
     * コンストラクタ
     * ========================================================= */

    /// <summary>
    /// PlayerControllerを生成する。
    ///
    /// PlayerModel、AttachEvent、PlayerSplineMoverは
    /// このコンストラクタ内で生成される。
    /// </summary>
    /// <param name="view">
    /// Playerの表示・Transform操作を担当するView。
    /// </param>
    /// <param name="playerExternalFacade">
    /// 外部システムへアクセスするFacade。
    /// </param>
    /// <param name="spawnOverlapResolver">
    /// Player初期位置の重なりを判定するクラス。
    /// </param>
    public PlayerController(
        PlayerView view,
        IPlayerExternalFacade playerExternalFacade,
        PlayerSpawnOverlapResolver spawnOverlapResolver)
    {
        // Playerの状態データを保持するModelを生成する。
        _model = new PlayerModel();

        _view = view;
        _playerExternalFacade = playerExternalFacade;
        _spawnOverlapResolver = spawnOverlapResolver;

        /*
         * Spline吸着時のイベント処理を生成する。
         *
         * ロングジャンプ成立や新規Spline着地時に、
         * このControllerが持つSubjectへ通知する。
         */
        _attachEvent = new AttachEvent(
            _view,
            _model,
            _onLongJumped,
            _onNewOrbitAttached
        );

        /*
         * Spline上の移動、ジャンプ、吸着処理を担当するMoverを生成する。
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
     * 入力イベントの登録
     * ========================================================= */

    /// <summary>
    /// Player操作に関する入力イベントを登録する。
    ///
    /// ・Move入力
    /// ・JumpPressed入力
    /// ・JumpReleased入力
    ///
    /// 入力そのものの検出は外部Facade側で行い、
    /// このクラスでは入力後の状態遷移を管理する。
    /// </summary>
    public void RegisterInputSubscriptions()
    {
        /*
         * Move入力時の処理。
         *
         * 通常移動中は周回方向を反転する。
         * チャージ中に押された場合はチャージをキャンセルする。
         */
        _playerExternalFacade.MoveSubscribe(() =>
        {
            // GameOver中は入力を受け付けない。
            if (_playerStateMachine.CurrentState is GameOverState)
                return;

            /*
             * チャージ中にMove入力が行われた場合、
             * 周回方向の反転ではなくチャージキャンセルとして扱う。
             */
            if (_playerStateMachine.CurrentState is ChargeState)
            {
                CancelChargeAndReturnMove();
                return;
            }

            // 時計回りと反時計回りを切り替える。
            _model.Clockwise = !_model.Clockwise;

            // 必要であれば回転方向UIの反転演出を呼び出す。
            //_view.FlipRotateUI();
        });

        /*
         * ジャンプ入力を離したときの処理。
         *
         * チャージ中にボタンを離すことでJumpStateへ遷移する。
         */
        _playerExternalFacade.JumpReleasedSubscribe(() =>
        {
            // 通常移動中なら、チャージを経由していないため無視する。
            if (_playerStateMachine.CurrentState is MoveState)
                return;

            // 既にジャンプ中なら再度ジャンプを開始しない。
            if (_playerStateMachine.CurrentState is JumpState)
                return;

            // GameOver中は入力を受け付けない。
            if (_playerStateMachine.CurrentState is GameOverState)
                return;

            // 現在のチャージ量を引き継いでジャンプ状態へ移行する。
            _playerStateMachine.ChangeState(PlayerStateKey.Jump);
        });

        /*
         * ジャンプ入力を押したときの処理。
         *
         * 通常移動中に押すことでChargeStateへ遷移する。
         */
        _playerExternalFacade.JumpPressedSubscribe(() =>
        {
            // ジャンプ中は新しくチャージを開始しない。
            if (_playerStateMachine.CurrentState is JumpState)
                return;

            // GameOver中は入力を受け付けない。
            if (_playerStateMachine.CurrentState is GameOverState)
                return;

            // チャージ状態へ移行する。
            _playerStateMachine.ChangeState(PlayerStateKey.Charge);
        });
    }


    /* =========================================================
     * Player接触イベントの登録
     * ========================================================= */

    /// <summary>
    /// Playerが敵弾、レーザー、障害物などへ接触した際の
    /// 外部イベントを購読する。
    ///
    /// ガード可能な攻撃の場合はGuardCountを消費し、
    /// ガードできなければGameOverStateへ移行する。
    /// </summary>
    public void RegisterPlayerSubscriptions()
    {
        /*
         * 敵弾に接触したときの処理。
         *
         * ガード回数が残っている場合は攻撃を無効化する。
         * ガードできなければゲームオーバーになる。
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
         * レーザービームに接触したときの処理。
         *
         * 現在の仕様ではレーザーはガード対象外で、
         * 接触すると即座にゲームオーバーになる。
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
         * 障害物に接触したときの処理。
         *
         * ガード回数が残っていれば1回分消費して防ぐ。
         * ガードできなければゲームオーバーになる。
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
         * ブラックホール内部へ侵入したときの処理。
         *
         * 現在の仕様では無条件でゲームオーバーになる。
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
         * ステージ外周の制限範囲を超えたときの処理。
         *
         * 場外へ出たためゲームオーバーにする。
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
         * Playerが敵本体へ接触したときの処理。
         *
         * 現在のコードではガード状態の確認を行わず、
         * 接触した敵を常に撃破する。
         *
         * 仕様として「チャージ中のみ敵を倒せる」のであれば、
         * ここでGuardCountやChargeLevelの確認が必要。
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
     * チャージキャンセル
     * ========================================================= */

    /// <summary>
    /// チャージをキャンセルし、通常移動状態へ戻す。
    ///
    /// ・ガード回数をリセット
    /// ・チャージSEを停止
    /// ・移動速度とジャンプ速度を初期値へ戻す
    /// ・オーラを通常色へ戻す
    /// ・MoveStateへ遷移
    /// </summary>
    private void CancelChargeAndReturnMove()
    {
        // チャージによって得られる予定だったガードを破棄する。
        _model.GuardCount = 0;

        // チャージ中のループSEを停止する。
        _playerExternalFacade.StopLoopSE();

        // チャージによって変化した速度を通常値へ戻す。
        _model.InitializeMoveSpeed();
        _model.InitializeJumpSpeed();

        // Playerのオーラを通常状態へ戻す。
        _view.SetAuraColor(ChargeLevel.Normal);

        // 通常移動状態へ戻る。
        _playerStateMachine.ChangeState(PlayerStateKey.Move);
    }


    /* =========================================================
     * ステートマシンの設定・更新
     * ========================================================= */

    /// <summary>
    /// PlayerControllerが利用するPlayerStateMachineを設定する。
    ///
    /// StateMachine側の生成後に外部から注入する。
    /// </summary>
    /// <param name="playerStateMachine">
    /// Playerの状態を管理するステートマシン。
    /// </param>
    public void SetPlayerStateMachine(
        PlayerStateMachine playerStateMachine)
    {
        _playerStateMachine = playerStateMachine;
    }

    /// <summary>
    /// VContainerのITickableによって毎フレーム呼ばれる。
    ///
    /// 現在のPlayerStateのTick処理を実行する。
    /// </summary>
    public void Tick()
    {
        // StateMachineが設定されている場合のみ更新する。
        _playerStateMachine?.Tick();
    }


    /* =========================================================
     * Player初期配置
     * ========================================================= */

    /// <summary>
    /// Playerをゲーム開始用Splineの始点へ配置する。
    ///
    /// 実際の安全位置検索と座標反映は
    /// PlayerSplineMover側で行われる。
    /// </summary>
    public void SetPlayer()
    {
        _mover.SetPlayer(
            _playerExternalFacade.StartSpline,
            0f
        );
    }


    /* =========================================================
     * MoveState用処理
     * ========================================================= */

    /// <summary>
    /// 通常移動状態を開始するときの初期化処理。
    ///
    /// 主にMoveStateのEnter処理から呼ばれる。
    /// </summary>
    public void StartMove()
    {
        // ジャンプ方向ガイドを非表示にする。
        _view.HideJumpNormalGuide();

        // Playerのオーラを通常色へ戻す。
        _view.SetAuraColor(ChargeLevel.Normal);

        // 現在のSpline情報を使ってMoverを初期化する。
        _mover.InitializeMove();

        // 移動速度を通常値へ戻す。
        _model.InitializeMoveSpeed();

        // 通常移動開始時はガード回数をリセットする。
        _model.GuardCount = 0;
    }

    /// <summary>
    /// 通常移動中の1フレーム分の処理。
    ///
    /// Playerを現在のSplineに沿って移動させる。
    /// </summary>
    public void TickMove()
    {
        _mover.Tick(Time.deltaTime);
    }


    /* =========================================================
     * JumpState用処理
     * ========================================================= */

    /// <summary>
    /// ジャンプ開始時の初期化処理。
    ///
    /// 現在のチャージレベルからガード回数を確定し、
    /// Splineの外向き法線方向へジャンプを開始する。
    /// </summary>
    public void StartJump()
    {
        /*
         * ChargeLevelに応じて、
         * 今回のジャンプ中に使用できるガード回数を設定する。
         */
        RefreshGuardCount();

        // チャージ中に再生していたループSEを停止する。
        _playerExternalFacade.StopLoopSE();

        // ジャンプ開始後は方向ガイドを隠す。
        _view.HideJumpNormalGuide();

        // PlayerSplineMover側でジャンプを開始する。
        _mover.StartJump();
    }

    /// <summary>
    /// ジャンプ中の1フレーム分の処理。
    /// </summary>
    /// <returns>
    /// 別のSplineへ接触し、吸着を開始した場合はtrue。
    /// 接触していない場合はfalse。
    /// </returns>
    public bool TickJump()
    {
        return _mover.TickJump(Time.deltaTime);
    }


    /* =========================================================
     * ChargeState用処理
     * ========================================================= */

    /// <summary>
    /// チャージ開始時の初期化処理。
    ///
    /// 主にChargeStateのEnter処理から呼ばれる。
    /// </summary>
    public void StartCharge()
    {
        // 現在のチャージ経過時間を0へ戻す。
        _model.CurrentChargeDuaration = 0f;

        /*
         * チャージ開始時点ではまだジャンプしていないため、
         * ガード回数を0に戻す。
         *
         * 実際のGuardCountはStartJump時に確定する。
         */
        _model.GuardCount = 0;

        // 前回チャージレベルを通常状態へ戻す。
        _previousChargeLevel = ChargeLevel.Normal;

        // 前回のループSEが残っている可能性があるため停止する。
        _playerExternalFacade.StopLoopSE();
    }

    /// <summary>
    /// チャージ中の1フレーム分の処理。
    ///
    /// ・チャージ時間の加算
    /// ・ジャンプ速度の更新
    /// ・周回速度の更新
    /// ・チャージレベル変化時のSE切り替え
    /// ・オーラ色の更新
    /// ・ジャンプ方向ガイドの表示
    /// </summary>
    public void TickCharge()
    {
        // チャージ経過時間を加算する。
        _model.CurrentChargeDuaration += Time.deltaTime;

        /*
         * 現在のチャージ時間に応じて、
         * ジャンプ速度と移動速度を更新する。
         */
        _model.ApplyChargeJumpSpeed();
        _model.ApplyChargeMoveSpeed();

        // 更新後のチャージレベルを取得する。
        ChargeLevel currentLevel =
            _model.CurrentChargeLevel;

        /*
         * ChargeLevelが前フレームから変化したときだけ、
         * チャージSEを切り替える。
         */
        if (currentLevel != _previousChargeLevel)
        {
            switch (currentLevel)
            {
                case ChargeLevel.Charge1:

                    // 以前のループSEを停止する。
                    _playerExternalFacade.StopLoopSE();

                    // Charge1用SEをループ再生する。
                    _playerExternalFacade.StartLoopSE(
                        SEType.Charge1
                    );
                    break;

                case ChargeLevel.Charge2:

                    // Charge1用SEなどが再生中なら停止する。
                    _playerExternalFacade.StopLoopSE();

                    // Charge2用SEをループ再生する。
                    _playerExternalFacade.StartLoopSE(
                        SEType.Charge2
                    );
                    break;

                case ChargeLevel.Normal:
                default:

                    // 通常状態ではチャージSEを再生しない。
                    _playerExternalFacade.StopLoopSE();
                    break;
            }

            // 次回比較用に現在レベルを保存する。
            _previousChargeLevel = currentLevel;
        }

        // 現在のChargeLevelに対応するオーラ色へ変更する。
        _view.SetAuraColor(currentLevel);

        /*
         * 現在地点のSpline外向き法線を取得し、
         * ジャンプ予定方向としてガイドを表示する。
         */
        _view.ShowJumpNormalGuide(
            _mover.GetOuterNormal()
        );
    }


    /* =========================================================
     * ガード回数の確定
     * ========================================================= */

    /// <summary>
    /// 現在のチャージレベルから、
    /// ジャンプ開始時に使用できるガード回数を設定する。
    ///
    /// Normal  : 0回
    /// Charge1 : 1回
    /// Charge2 : 2回
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
     * 敵弾に対するガード処理
     * ========================================================= */

    /// <summary>
    /// 敵弾をガードできるか判定し、
    /// 可能な場合はGuardCountを1消費する。
    /// </summary>
    /// <returns>
    /// 敵弾をガードした場合はtrue。
    /// ガードできなかった場合はfalse。
    /// </returns>
    private bool TryGuardEnemyBullet()
    {
        // ガード回数が残っていなければ防げない。
        if (_model.GuardCount <= 0)
            return false;

        // 敵弾1回分のガードを消費する。
        _model.GuardCount--;

        /*
         * ガード後の残り回数に応じて、
         * Playerの見た目や速度状態を更新する。
         */
        switch (_model.GuardCount)
        {
            case 1:

                /*
                 * Charge2から1回ガードを消費し、
                 * Charge1相当の状態になったことを見た目へ反映する。
                 *
                 * この敵弾用処理では速度変更を行っていない。
                 */
                _view.SetAuraColor(ChargeLevel.Charge1);
                break;

            case 0:

                // ガードをすべて使い切ったため通常色へ戻す。
                _view.SetAuraColor(ChargeLevel.Normal);

                // 速度を通常状態へ戻す。
                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        // 敵弾を正常にガードした。
        return true;
    }


    /* =========================================================
     * 障害物に対するガード処理
     * ========================================================= */

    /// <summary>
    /// 障害物への接触をガードできるか判定し、
    /// 可能な場合はGuardCountを1消費する。
    /// </summary>
    /// <returns>
    /// 障害物をガードした場合はtrue。
    /// ガードできなかった場合はfalse。
    /// </returns>
    private bool TryGuardObstacle()
    {
        // ガード回数が残っていなければ防げない。
        if (_model.GuardCount <= 0)
            return false;

        // 障害物1回分のガードを消費する。
        _model.GuardCount--;

        switch (_model.GuardCount)
        {
            case 1:

                /*
                 * Charge2状態から1回分を消費したため、
                 * Charge1相当の見た目と速度へ変更する。
                 */
                _view.SetAuraColor(ChargeLevel.Charge1);

                _model.SetCharge1JumpSpeed();
                _model.SetCharge1MoveSpeed();
                break;

            case 0:

                // すべてのガードを消費したため通常状態へ戻す。
                _view.SetAuraColor(ChargeLevel.Normal);

                _model.InitializeMoveSpeed();
                _model.InitializeJumpSpeed();
                break;
        }

        return true;
    }


    /* =========================================================
     * 旧・敵本体接触時のガード処理
     * ========================================================= */

    /*
     * 現在は使用されていない処理。
     *
     * 敵本体へ接触した際にGuardCountを消費し、
     * Charge2からCharge1、Charge1からNormalへ
     * 段階的に移行する想定だったコード。
     *
     * 現在のRegisterPlayerSubscriptionsでは、
     * 敵へ接触すると無条件でDefeatEnemyを呼び出している。
     */

    //private bool TryGuardEnemyContact()
    //{
    //    // ガード回数が残っていなければ防げない。
    //    if (_model.GuardCount <= 0)
    //        return false;
    //
    //    // 敵接触1回分のガードを消費する。
    //    _model.GuardCount--;
    //
    //    switch (_model.GuardCount)
    //    {
    //        case 1:
    //
    //            // Charge2からCharge1相当の状態へ変更する。
    //            _view.SetAuraColor(ChargeLevel.Charge1);
    //            _model.SetCharge1JumpSpeed();
    //            _model.SetCharge1MoveSpeed();
    //            break;
    //
    //        case 0:
    //
    //            // ガードを使い切ったため通常状態へ戻す。
    //            _view.SetAuraColor(ChargeLevel.Normal);
    //            _model.InitializeMoveSpeed();
    //            _model.InitializeJumpSpeed();
    //            break;
    //    }
    //
    //    return true;
    //}


    /* =========================================================
     * 死亡処理
     * ========================================================= */

    /// <summary>
    /// Player死亡時の共通処理を実行する。
    ///
    /// 主にGameOverStateの開始時などから呼ばれる。
    ///
    /// ・死亡イベントを通知
    /// ・死亡SEを再生
    /// ・死亡エフェクトを再生
    /// ・ジャンプ方向ガイドを非表示
    /// ・ループSEを停止
    /// </summary>
    public void Dead()
    {
        // 外部へPlayer死亡を通知する。
        _onPlayerDead.OnNext(Unit.Default);

        // Player死亡SEを再生する。
        _playerExternalFacade.PlaySE(
            SEType.PlayerDead
        );

        /*
         * PlayerViewの死亡エフェクトを非同期再生する。
         *
         * Forgetにより完了待ちは行わない。
         * 例外処理が必要な場合はForgetの扱いに注意する。
         */
        _view.PlayDeadEffect().Forget();

        // ジャンプ予定方向のガイドを隠す。
        _view.HideJumpNormalGuide();

        // チャージなどのループSEを停止する。
        _playerExternalFacade.StopLoopSE();
    }


    /* =========================================================
     * 破棄処理
     * ========================================================= */

    /// <summary>
    /// PlayerControllerが保持するUniRx購読とSubjectを破棄する。
    ///
    /// シーン破棄後にイベントが呼ばれ続けることや、
    /// 多重購読、メモリリークを防止する。
    /// </summary>
    public void Dispose()
    {
        // 敵弾や障害物などの外部イベント購読を一括解除する。
        _playerSubscriptions.Dispose();

        // PlayerControllerが所有するSubjectを破棄する。
        _onLongJumped.Dispose();
        _onNewOrbitAttached.Dispose();
        _onPlayerDead.Dispose();
    }
}

