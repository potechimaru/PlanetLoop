using UnityEngine;

/// <summary>
/// Playerのチャージ段階を表す列挙型。
///
/// Normal  : 未チャージ、またはチャージ時間が1段階目未満
/// Charge1 : 1段階目のチャージ完了
/// Charge2 : 2段階目のチャージ完了
/// </summary>
internal enum ChargeLevel
{
    /// <summary>
    /// 通常状態。
    /// </summary>
    Normal,

    /// <summary>
    /// 1段階目のチャージ状態。
    /// </summary>
    Charge1,

    /// <summary>
    /// 2段階目のチャージ状態。
    /// </summary>
    Charge2
}

/// <summary>
/// Playerの移動・ジャンプ・チャージ・ガードに関する
/// 数値と状態を保持するModelクラス。
///
/// 主に以下の情報を管理する。
///
/// ・現在の周回移動速度
/// ・現在のジャンプ速度
/// ・Splineの周回方向
/// ・現在のチャージ経過時間
/// ・現在のチャージレベル
/// ・チャージレベルごとの速度
/// ・ロングジャンプの判定距離
/// ・残りガード回数
///
/// Playerの見た目やTransform操作は行わず、
/// ゲーム上の数値と状態のみを管理する。
/// </summary>
internal class PlayerModel
{
    /* =========================================================
     * 現在のPlayer状態
     * ========================================================= */

    /// <summary>
    /// 現在のSpline上の移動速度。
    ///
    /// 通常時は5、
    /// Charge1では3.5、
    /// Charge2では2になる。
    ///
    /// チャージ中はApplyChargeMoveSpeedによって更新される。
    /// </summary>
    public float CurrentMoveSpeed { get; set; } = 5f;

    /// <summary>
    /// 現在のジャンプ速度。
    ///
    /// 通常時は5、
    /// Charge1では10、
    /// Charge2では20になる。
    ///
    /// チャージ中はApplyChargeJumpSpeedによって更新される。
    ///
    /// 名前の「Jumpspeed」は一般的には
    /// 「JumpSpeed」と大文字を区切る方が読みやすい。
    /// </summary>
    public float CurrentJumpspeed { get; set; } = 5f;

    /// <summary>
    /// Playerが現在どちら向きにSplineを周回しているか。
    ///
    /// true  : 時計回り
    /// false : 反時計回り
    ///
    /// 実際の方向はSplineの制御点の並び順によって
    /// 見た目上逆になる可能性がある。
    /// </summary>
    public bool Clockwise { get; set; } = false;

    /// <summary>
    /// 現在のチャージ経過時間。
    ///
    /// PlayerControllerのTickChargeによって
    /// 毎フレームTime.deltaTimeが加算される。
    ///
    /// 単位は秒。
    ///
    /// 「Duaration」はスペルミスで、
    /// 正しくは「Duration」。
    /// </summary>
    public float CurrentChargeDuaration { get; set; } = 0f;


    /* =========================================================
     * 外部公開用の設定値
     * ========================================================= */

    /// <summary>
    /// ロングジャンプと判定するための距離しきい値。
    ///
    /// 現在は40に設定されている。
    /// </summary>
    public float LongJumpDistanceThreshold =>
        _LONG_JUMP_DISTANCE;


    /* =========================================================
     * 現在のチャージレベル
     * ========================================================= */

    /// <summary>
    /// CurrentChargeDuarationから、
    /// 現在のチャージレベルを計算して返す。
    ///
    /// 現在の判定は以下。
    ///
    /// 0秒～1秒以下     : Normal
    /// 1秒より大きく2秒以下 : Charge1
    /// 2秒より大きい    : Charge2
    ///
    /// 「ちょうど1秒でCharge1にしたい」場合は、
    /// 比較演算子を見直す必要がある。
    /// </summary>
    public ChargeLevel CurrentChargeLevel
    {
        get
        {
            /*
             * チャージ時間が1段階目の基準以下ならNormal。
             *
             * 現在は「<=」なので、ちょうど1.0秒でもNormalになる。
             */
            if (CurrentChargeDuaration <= _CHARGE_DUARATION_1)
            {
                return ChargeLevel.Normal;
            }

            /*
             * 1段階目を超え、2段階目の基準以下ならCharge1。
             *
             * 現在は、1秒より大きく2秒以下の範囲。
             */
            if (CurrentChargeDuaration <= _CHARGE_DUARATION_2)
            {
                return ChargeLevel.Charge1;
            }

            // 2段階目の基準を超えた場合はCharge2。
            return ChargeLevel.Charge2;
        }
    }


    /* =========================================================
     * チャージ時間のしきい値
     * ========================================================= */

    /// <summary>
    /// Charge1へ移行するためのチャージ時間。
    ///
    /// 現在の判定式では、
    /// この値を「超えた」ときにCharge1になる。
    /// </summary>
    private readonly float _CHARGE_DUARATION_1 = 1f;

    /// <summary>
    /// Charge2へ移行するためのチャージ時間。
    ///
    /// 現在の判定式では、
    /// この値を「超えた」ときにCharge2になる。
    /// </summary>
    private readonly float _CHARGE_DUARATION_2 = 2f;


    /* =========================================================
     * ジャンプ速度設定
     * ========================================================= */

    /// <summary>
    /// 通常状態のジャンプ速度。
    /// </summary>
    private readonly float _NORMAL_JUMP_SPEED = 5f;

    /// <summary>
    /// Charge1状態のジャンプ速度。
    /// </summary>
    private readonly float _CHARGE_JUMP_SPEED_1 = 10f;

    /// <summary>
    /// Charge2状態のジャンプ速度。
    /// </summary>
    private readonly float _CHARGE_JUMP_SPEED_2 = 20f;


    /* =========================================================
     * Spline上の移動速度設定
     * ========================================================= */

    /// <summary>
    /// 通常状態のSpline上の移動速度。
    /// </summary>
    private readonly float _NORMAL_MOVE_SPEED = 5f;

    /// <summary>
    /// Charge1状態のSpline上の移動速度。
    ///
    /// チャージ中は通常より少し遅くなる。
    /// </summary>
    private readonly float _CHARGE_MOVE_SPEED_1 = 3.5f;

    /// <summary>
    /// Charge2状態のSpline上の移動速度。
    ///
    /// Charge1よりさらに遅くなる。
    /// </summary>
    private readonly float _CHARGE_MOVE_SPEED_2 = 2f;


    /* =========================================================
     * ロングジャンプ設定
     * ========================================================= */

    /// <summary>
    /// ロングジャンプ成立に必要な移動距離。
    ///
    /// PlayerSplineMoverで計測したジャンプ経路の長さが
    /// この値以上かどうかをAttachEvent側で判定する。
    /// </summary>
    private readonly float _LONG_JUMP_DISTANCE = 40f;


    /* =========================================================
     * ガード状態
     * ========================================================= */

    /// <summary>
    /// 現在残っているガード回数。
    ///
    /// 通常状態 : 0
    /// Charge1 : 1
    /// Charge2 : 2
    ///
    /// 敵弾や障害物へ接触すると1ずつ減少する。
    /// </summary>
    public int GuardCount { get; set; } = 0;


    /* =========================================================
     * チャージに応じたジャンプ速度の取得
     * ========================================================= */

    /// <summary>
    /// 現在のチャージ時間に対応するジャンプ速度を取得する。
    /// </summary>
    /// <returns>
    /// 現在のチャージ段階に対応するジャンプ速度。
    /// </returns>
    private float GetChargeJumpSpeed()
    {
        // 1段階目のチャージ時間以下なら通常ジャンプ速度。
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_1)
        {
            return _NORMAL_JUMP_SPEED;
        }

        // 2段階目のチャージ時間以下ならCharge1のジャンプ速度。
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_2)
        {
            return _CHARGE_JUMP_SPEED_1;
        }

        // 2段階目を超えた場合はCharge2のジャンプ速度。
        return _CHARGE_JUMP_SPEED_2;
    }


    /* =========================================================
     * チャージに応じた移動速度の取得
     * ========================================================= */

    /// <summary>
    /// 現在のチャージ時間に対応するSpline上の移動速度を取得する。
    /// </summary>
    /// <returns>
    /// 現在のチャージ段階に対応する移動速度。
    /// </returns>
    private float GetMoveSpeed()
    {
        // 1段階目のチャージ時間以下なら通常移動速度。
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_1)
        {
            return _NORMAL_MOVE_SPEED;
        }

        // 2段階目のチャージ時間以下ならCharge1の移動速度。
        if (CurrentChargeDuaration <= _CHARGE_DUARATION_2)
        {
            return _CHARGE_MOVE_SPEED_1;
        }

        // 2段階目を超えた場合はCharge2の移動速度。
        return _CHARGE_MOVE_SPEED_2;
    }


    /* =========================================================
     * チャージ速度の適用
     * ========================================================= */

    /// <summary>
    /// 現在のチャージ時間に対応するジャンプ速度を
    /// CurrentJumpspeedへ適用する。
    ///
    /// PlayerControllerのTickChargeから毎フレーム呼ばれる。
    /// </summary>
    public void ApplyChargeJumpSpeed()
    {
        CurrentJumpspeed = GetChargeJumpSpeed();
    }

    /// <summary>
    /// 現在のチャージ時間に対応する移動速度を
    /// CurrentMoveSpeedへ適用する。
    ///
    /// PlayerControllerのTickChargeから毎フレーム呼ばれる。
    /// </summary>
    public void ApplyChargeMoveSpeed()
    {
        CurrentMoveSpeed = GetMoveSpeed();
    }


    /* =========================================================
     * 速度の初期化
     * ========================================================= */

    /// <summary>
    /// 現在のSpline上の移動速度を通常値へ戻す。
    ///
    /// チャージキャンセル時やガードを使い切った際に使用する。
    /// </summary>
    public void InitializeMoveSpeed()
    {
        CurrentMoveSpeed = _NORMAL_MOVE_SPEED;
    }

    /// <summary>
    /// 現在のジャンプ速度を通常値へ戻す。
    ///
    /// チャージキャンセル時やガードを使い切った際に使用する。
    /// </summary>
    public void InitializeJumpSpeed()
    {
        CurrentJumpspeed = _NORMAL_JUMP_SPEED;
    }


    /* =========================================================
     * Charge1相当の速度へ変更
     * ========================================================= */

    /// <summary>
    /// 現在の移動速度をCharge1相当へ変更する想定のメソッド。
    ///
    /// ただし現在のコードでは、
    /// CurrentMoveSpeedではなくCurrentJumpspeedへ
    /// ジャンプ速度を代入している。
    ///
    /// メソッド名と処理内容が一致していない可能性がある。
    /// </summary>
    public void SetCharge1MoveSpeed()
    {
        /*
         * 現在の処理：
         * ジャンプ速度をCharge1へ変更している。
         *
         * メソッド名どおり移動速度を変更するなら、
         * 次のようになる。
         *
         * CurrentMoveSpeed = _CHARGE_MOVE_SPEED_1;
         */
        CurrentJumpspeed = _CHARGE_JUMP_SPEED_1;
    }

    /// <summary>
    /// 現在のジャンプ速度をCharge1相当へ変更する想定のメソッド。
    ///
    /// ただし現在のコードでは、
    /// CurrentJumpspeedではなくCurrentMoveSpeedへ
    /// 移動速度を代入している。
    ///
    /// メソッド名と処理内容が一致していない可能性がある。
    /// </summary>
    public void SetCharge1JumpSpeed()
    {
        /*
         * 現在の処理：
         * 移動速度をCharge1へ変更している。
         *
         * メソッド名どおりジャンプ速度を変更するなら、
         * 次のようになる。
         *
         * CurrentJumpspeed = _CHARGE_JUMP_SPEED_1;
         */
        CurrentMoveSpeed = _CHARGE_MOVE_SPEED_1;
    }
}

