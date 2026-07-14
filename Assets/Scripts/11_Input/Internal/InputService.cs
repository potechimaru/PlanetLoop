using System;
using UniRx;
using UnityEngine.InputSystem;

/// <summary>
/// Unity Input Systemから入力を取得し、
/// UniRxのObservableとして公開するサービスクラス。
///
/// このクラス自身はゲーム処理を行わず、
/// 入力が発生したことだけを通知する役割を持つ。
///
/// 現在対応している入力は以下。
///
/// ・Move（右クリック）
///     → Playerの周回方向反転
///
/// ・JumpPressed（左クリック押下）
///     → チャージ開始
///
/// ・JumpReleased（左クリック離す）
///     → ジャンプ開始
///
/// PlayerControllerなどは、このObservableを購読して
/// 実際のゲーム処理を実行する。
/// </summary>
public class InputService : IDisposable
{
    /* =========================================================
     * Unity Input System
     * ========================================================= */

    /// <summary>
    /// Input Systemによって自動生成された入力クラス。
    ///
    /// Input Actions Assetに定義された
    /// Playerアクションマップを保持している。
    ///
    /// このクラスからMoveやJumpなどのInputActionへアクセスする。
    /// </summary>
    private PlayerInput _playerActions = new();


    /* =========================================================
     * Move入力
     * ========================================================= */

    /// <summary>
    /// Move入力（右クリック）が行われたことを通知するSubject。
    ///
    /// このクラス内部だけがOnNextを実行し、
    /// 外部へはObservableとして公開する。
    /// </summary>
    private Subject<Unit> _onMove = new();

    /// <summary>
    /// Move入力が行われたときに通知されるObservable。
    ///
    /// PlayerControllerなどが購読し、
    /// Playerの周回方向反転処理を実行する。
    /// </summary>
    public IObservable<Unit> OnMove => _onMove;


    /* =========================================================
     * Jump入力
     * ========================================================= */

    /// <summary>
    /// Jumpボタンが押された瞬間を通知するSubject。
    ///
    /// InputAction.startedに対応する。
    /// </summary>
    private Subject<Unit> _onJumpPressed = new();

    /// <summary>
    /// Jumpボタンが離された瞬間を通知するSubject。
    ///
    /// InputAction.canceledに対応する。
    /// </summary>
    private Subject<Unit> _onJumpReleased = new();

    /// <summary>
    /// Jumpボタン押下時に通知されるObservable。
    ///
    /// PlayerControllerでは
    /// ChargeStateへの遷移に使用される。
    /// </summary>
    public IObservable<Unit> OnJumpPressed => _onJumpPressed;

    /// <summary>
    /// Jumpボタンを離したときに通知されるObservable。
    ///
    /// PlayerControllerでは
    /// JumpStateへの遷移に使用される。
    /// </summary>
    public IObservable<Unit> OnJumpReleased => _onJumpReleased;


    /* =========================================================
     * コンストラクタ
     * ========================================================= */

    /// <summary>
    /// InputServiceを生成する。
    ///
    /// Input Systemを有効化し、
    /// MoveとJump入力をUniRxイベントへ変換する。
    /// </summary>
    internal InputService()
    {
        /*
         * Input Actions Asset内の
         * Playerアクションマップから
         * Moveアクションを取得する。
         */
        var move = _playerActions.Player.Move;

        /*
         * Playerアクションマップから
         * Jumpアクションを取得する。
         */
        var jump = _playerActions.Player.Jump;

        /*
         * Playerアクションマップ全体を有効化する。
         *
         * Enableしないと入力イベントは発生しない。
         */
        move.actionMap.Enable();

        /* =====================================================
         * Move入力（右クリック）
         * ===================================================== */

        /*
         * Move入力がperformedした瞬間、
         * UniRxイベントへ変換して通知する。
         *
         * performedは
         * 「入力が成立した瞬間」
         * に一度だけ呼ばれる。
         *
         * 現在は右クリックによる
         * Playerの周回方向反転に利用されている。
         */
        move.performed += _ =>
        {
            _onMove.OnNext(Unit.Default);
        };


        /* =====================================================
         * Jump入力（左クリック）
         * ===================================================== */

        /*
         * 左クリックが押された瞬間。
         *
         * InputAction.startedは、
         * ボタンを押した瞬間に一度だけ呼ばれる。
         *
         * PlayerControllerでは
         * ChargeState開始に利用している。
         */
        jump.started += _ =>
        {
            _onJumpPressed.OnNext(Unit.Default);
        };

        /*
         * 左クリックを離した瞬間。
         *
         * InputAction.canceledは、
         * ボタンを離した瞬間に一度だけ呼ばれる。
         *
         * PlayerControllerでは
         * チャージ終了・ジャンプ開始に利用している。
         */
        jump.canceled += _ =>
        {
            _onJumpReleased.OnNext(Unit.Default);
        };
    }


    /* =========================================================
     * 破棄処理
     * ========================================================= */

    /// <summary>
    /// InputServiceが保持しているSubjectと
    /// Input Systemを破棄する。
    ///
    /// シーン終了時などに呼び出され、
    /// メモリリークや不要な入力通知を防ぐ。
    /// </summary>
    public void Dispose()
    {
        /*
         * UniRxのSubjectを破棄する。
         *
         * Dispose後はOnNextなどを呼び出せなくなる。
         */
        _onMove?.Dispose();
        _onJumpPressed?.Dispose();
        _onJumpReleased?.Dispose();

        /*
         * Input Systemが生成したPlayerInputを破棄する。
         *
         * InputActionMapもあわせて解放される。
         */
        _playerActions?.Dispose();
    }
}
