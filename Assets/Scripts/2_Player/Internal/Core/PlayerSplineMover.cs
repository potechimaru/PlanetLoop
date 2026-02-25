using System;
using UnityEngine;

internal class PlayerSplineMover
{
    private const float AttachRadius = 0.1f;
    private const float AttachDuration = 0.03f;

    private readonly PlayerView _view;
    private readonly PlayerModel _model;
    private readonly IPlayerExternalFacade _playerExternalFacade;

    private ClosedSplineLine _currentSpline;

    // --- spline move ---
    private float _totalLen;
    private float _distance;

    // --- attach lerp ---
    private bool _isAttaching;
    private float _attachT;
    private Vector3 _attachFrom;
    private Vector3 _attachTo;

    // --- jump state (Mover側に集約) ---
    private bool _isJumping;
    private Vector3 _jumpDir;     // ワールド方向（正規化）
    private float _jumpSpeed;     // 速度（一定で進む）
    private Vector3 _jumpPos;     // ジャンプ中の計算用位置（Viewのtransform依存を減らす）

    internal PlayerSplineMover(
        PlayerView view,
        PlayerModel model,
        IPlayerExternalFacade playerExternalFacade)
    {
        _view = view;
        _model = model;
        _playerExternalFacade = playerExternalFacade;

        _currentSpline = _view.Spline;
    }

    public void Initialize()
    {
        RebuildTable();

        if (_totalLen <= 0.0001f)
        {
            Debug.LogError("[PlayerSplineMover] Initialize failed: spline samples are empty");
        }
    }

    /* =========================
     * 通常周回
     * ========================= */

    public void Tick(float deltaTime)
    {
        if (_model.IsGameOver) return;
        if (_totalLen <= 0.0001f) return;

        // ジャンプ中はTickJumpで処理する想定（StateMachine側で分岐）
        if (_isJumping) return;

        // 吸着中はLerpで位置を寄せる
        if (_isAttaching)
        {
            _attachT += deltaTime / AttachDuration;
            float t = Mathf.SmoothStep(0f, 1f, _attachT);

            Vector3 pos = Vector3.Lerp(_attachFrom, _attachTo, t);
            _view.SetPosition(pos);

            if (_attachT >= 1f)
            {
                _isAttaching = false;
            }
            return;
        }

        float dir = _model.Clockwise ? -1f : 1f;
        _distance = Mathf.Repeat(
            _distance + dir * _model.CurrentMoveSpeed * deltaTime,
            _totalLen
        );

        ApplyPosition();
    }

    /* =========================
     * Jump（開始）
     * ========================= */

    public void StartJump()
    {
        if (_model.IsGameOver) return;

        // ジャンプ開始位置・方向・速度をMoverで確定
        _isJumping = true;

        _jumpPos = _view.transform.position;

        // 既存仕様：Spline外向き法線方向へ射出
        _jumpDir = GetOuterNormal().normalized;

        // 既存仕様：チャージ等で決まったジャンプ速度を使う（開始時にスナップショット）
        _jumpSpeed = _model.CurrentJumpspeed;
    }

    /* =========================
     * Jump（更新＋吸着判定）
     * ========================= */

    public bool TickJump(float dt)
    {
        if (_model.IsGameOver) return false;
        if (!_isJumping) return false;

        // ★ブラックホール重力：方向だけ曲げる（速度は変えない）
        // Facadeから参照できる前提（nullなら何もしない）
        if (_playerExternalFacade != null)
        {
            _jumpDir = _playerExternalFacade.BendDirection(_jumpPos, _jumpDir, dt);
        }

        // 位置更新（速度一定）
        _jumpPos += _jumpDir * _jumpSpeed * dt;
        _view.SetPosition(_jumpPos);

        // 他Splineに触れたか判定（更新後の位置で判定する）
        if (_playerExternalFacade.TryFindTouchedSpline(
            _jumpPos,
            AttachRadius,
            _currentSpline,
            out var touchedSpline))
        {
            AttachToSpline(touchedSpline, _jumpPos);

            // ジャンプ終了（吸着へ）
            _isJumping = false;
            return true;
        }

        return false;
    }

    /* =========================
     * 再吸着
     * ========================= */

    private void AttachToSpline(ClosedSplineLine newSpline, Vector3 playerWorldPos)
    {
        _view.SetSpline(newSpline);
        _currentSpline = newSpline;

        _distance = _currentSpline.FindNearestDistance(playerWorldPos);
        _totalLen = _currentSpline.GetTotalLength();

        _attachFrom = _view.transform.position;
        _attachTo = _currentSpline.EvaluateByDistance(_distance);

        _attachT = 0f;
        _isAttaching = true;

        // 着地演出（種類分け済み版が入っている想定）
        _view.PlaySplineAttachFx(_currentSpline, _distance, playerWorldPos);
    }

    /* =========================
     * Spline Table
     * ========================= */

    private void RebuildTable()
    {
        if (_currentSpline == null)
        {
            _totalLen = 0f;
            return;
        }

        _totalLen = _currentSpline.GetTotalLength();
    }

    private void ApplyPosition()
    {
        Vector3 pos = _currentSpline.EvaluateByDistance(_distance);
        _view.SetPosition(pos);
    }

    /* =========================
     * 幾何
     * ========================= */

    public Vector3 GetOuterNormal()
    {
        return _currentSpline.EvaluateNormalByDistance(_distance);
    }

    // 既存の外部からの互換用（必要なら）
    public bool IsJumping => _isJumping;
}