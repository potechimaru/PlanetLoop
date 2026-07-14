
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// Playerの見た目とTransform操作を担当するViewクラス。
///
/// 主に以下の処理を担当する。
///
/// ・Playerのワールド座標を更新する
/// ・現在所属しているSplineを保持する
/// ・チャージレベルに応じてオーラのマテリアルを変更する
/// ・ジャンプ方向ガイドを表示・非表示にする
/// ・Spline着地時のエフェクトを再生する
/// ・Player死亡時のエフェクトと消失アニメーションを再生する
///
/// Playerの移動計算やゲーム上の状態管理は行わず、
/// PlayerControllerやPlayerSplineMoverから渡された結果を
/// 見た目へ反映する役割を持つ。
/// </summary>
public class PlayerView : MonoBehaviour
{
    /* =========================================================
     * Spline関連設定
     * ========================================================= */

    /// <summary>
    /// Playerが現在所属しているSpline。
    ///
    /// ゲーム開始時はInspectorで設定されたSplineが使用され、
    /// 別のSplineへ着地した際はSetSplineによって更新される。
    /// </summary>
    [Header("Spline")]
    [SerializeField]
    private ClosedSplineLine _spline;

    /// <summary>
    /// Playerの移動平面として、
    /// ローカルXY平面を使用するかどうか。
    ///
    /// 現在のPlayerView内では直接使用されていないが、
    /// 外部クラスからUseLocalPlaneXYプロパティ経由で参照できる。
    /// </summary>
    [SerializeField]
    private bool _useLocalPlaneXY = true;

    /// <summary>
    /// チャージ中にPlayerのジャンプ予定方向を表示するガイド。
    ///
    /// Spline上の現在位置における外向き法線方向を
    /// 視覚的に表示する。
    /// </summary>
    [SerializeField]
    private JumpNormalGuide _jumpNormalGuide;


    /* =========================================================
     * オーラ用マテリアル
     * ========================================================= */

    /// <summary>
    /// 通常状態で使用するPlayerオーラのマテリアル。
    ///
    /// ChargeLevel.Normalに対応する。
    /// </summary>
    [SerializeField]
    private Material _auraMaterialBlue;

    /// <summary>
    /// Charge1状態で使用するPlayerオーラのマテリアル。
    /// </summary>
    [SerializeField]
    private Material _auraMaterialOrange;

    /// <summary>
    /// Charge2状態で使用するPlayerオーラのマテリアル。
    /// </summary>
    [SerializeField]
    private Material _auraMaterialRed;


    /* =========================================================
     * 死亡演出設定
     * ========================================================= */

    /// <summary>
    /// Player死亡時に再生するParticleSystem。
    ///
    /// PlayDeadEffect内で再生され、
    /// ParticleSystemが完全に終了するまで待機する。
    /// </summary>
    [SerializeField]
    private ParticleSystem _deadEffect;

    /// <summary>
    /// Player本体を非表示へ変化させる死亡アニメーション。
    ///
    /// ParticleSystemと並行して開始され、
    /// アニメーション完了後もParticleSystemが終了するまで待機する。
    /// </summary>
    [SerializeField]
    private PlayerDisappearAnimation _playerDisappearAnimation;

    /*
     * Playerを継続的に回転させるアニメーション。
     *
     * 現在は使用されていないためコメントアウトされている。
     */
    //[SerializeField]
    //private ContinuousRotateAnimation _continuousRotateAnimation;


    /* =========================================================
     * 内部参照
     * ========================================================= */

    /// <summary>
    /// Playerのオーラを描画しているMeshRenderer。
    ///
    /// Awake時に子オブジェクトから取得し、
    /// SetAuraColorでマテリアルを切り替える。
    /// </summary>
    private MeshRenderer _auraRenderer;


    /* =========================================================
     * 公開プロパティ
     * ========================================================= */

    /// <summary>
    /// Playerが現在所属しているSplineを取得する。
    /// </summary>
    public ClosedSplineLine Spline => _spline;

    /// <summary>
    /// PlayerがローカルXY平面を使用する設定かどうかを取得する。
    /// </summary>
    public bool UseLocalPlaneXY => _useLocalPlaneXY;


    /* =========================================================
     * Unityライフサイクル
     * ========================================================= */

    /// <summary>
    /// GameObject生成時に一度だけ呼び出される。
    ///
    /// Playerの子オブジェクトから、
    /// オーラ表示に使用するMeshRendererを取得する。
    /// </summary>
    private void Awake()
    {
        /*
         * 子階層内で最初に見つかったMeshRendererを取得する。
         *
         * Playerの子に複数のMeshRendererがある場合、
         * 意図したAuraのRendererではないものが取得される可能性がある。
         *
         * 確実にAuraだけを操作したい場合は、
         * Inspectorから直接SerializeFieldで参照する方法もある。
         */
        _auraRenderer =
            GetComponentInChildren<MeshRenderer>();
    }


    /* =========================================================
     * Player座標の更新
     * ========================================================= */

    /// <summary>
    /// Playerのワールド座標を設定する。
    ///
    /// 描画順やZファイティングを避けるため、
    /// 渡された座標よりZ方向へ0.01だけ手前にずらして配置する。
    /// </summary>
    /// <param name="worldPos">
    /// Playerを配置する基準ワールド座標。
    /// </param>
    public void SetPosition(Vector3 worldPos)
    {
        /*
         * Splineや他の描画オブジェクトと完全に同じZ座標になると、
         * 描画順が不安定になる可能性がある。
         *
         * そのため、Z座標を0.01だけ小さくして
         * Playerをわずかに手前へ配置している。
         */
        worldPos = new Vector3(
            worldPos.x,
            worldPos.y,
            worldPos.z - 0.01f
        );

        // 計算後のワールド座標をTransformへ反映する。
        transform.position = worldPos;
    }


    /* =========================================================
     * 所属Splineの更新
     * ========================================================= */

    /// <summary>
    /// Playerが現在所属しているSplineを更新する。
    ///
    /// 別のSplineへ着地した際に、
    /// PlayerSplineMoverから呼び出される。
    /// </summary>
    /// <param name="spline">
    /// 新しく所属するSpline。
    /// </param>
    public void SetSpline(ClosedSplineLine spline)
    {
        _spline = spline;
    }


    /* =========================================================
     * オーラ表示
     * ========================================================= */

    /// <summary>
    /// 現在のチャージレベルに応じて、
    /// Playerオーラのマテリアルを変更する。
    ///
    /// Normal  ：青
    /// Charge1 ：オレンジ
    /// Charge2 ：赤
    /// </summary>
    /// <param name="chargeLevel">
    /// 現在のPlayerのチャージレベル。
    /// </param>
    internal void SetAuraColor(ChargeLevel chargeLevel)
    {
        /*
         * 想定外のChargeLevelが渡された場合でも
         * nullにならないよう、初期値は通常状態の青にする。
         */
        Material material = _auraMaterialBlue;

        // チャージレベルに対応するマテリアルを選択する。
        switch (chargeLevel)
        {
            case ChargeLevel.Normal:

                material = _auraMaterialBlue;
                break;

            case ChargeLevel.Charge1:

                material = _auraMaterialOrange;
                break;

            case ChargeLevel.Charge2:

                material = _auraMaterialRed;
                break;
        }

        /*
         * Rendererが正常に取得できている場合のみ、
         * マテリアルを変更する。
         *
         * sharedMaterialを使用しているため、
         * Renderer専用のMaterialインスタンスは生成されない。
         *
         * 同じMaterialを使用する他Rendererにも
         * Material自体の変更は共有されるが、
         * このコードでは参照先を切り替えているだけなので問題は起きにくい。
         */
        if (_auraRenderer != null)
        {
            _auraRenderer.sharedMaterial = material;
        }
    }


    /* =========================================================
     * ジャンプ方向ガイド
     * ========================================================= */

    /// <summary>
    /// Playerのジャンプ予定方向を示すガイドを表示する。
    ///
    /// Playerの現在位置と、
    /// Spline上の外向き法線方向をJumpNormalGuideへ渡す。
    /// </summary>
    /// <param name="normal">
    /// ジャンプ予定方向となる法線ベクトル。
    /// </param>
    public void ShowJumpNormalGuide(Vector3 normal)
    {
        /*
         * Playerの現在位置をガイドの開始地点とし、
         * normal方向へガイドを表示する。
         */
        _jumpNormalGuide.Show(
            transform.position,
            normal
        );
    }

    /// <summary>
    /// ジャンプ方向ガイドを非表示にする。
    ///
    /// チャージ終了時、ジャンプ開始時、死亡時などに呼び出される。
    /// </summary>
    public void HideJumpNormalGuide()
    {
        _jumpNormalGuide.Hide();
    }


    /* =========================================================
     * Spline着地エフェクト
     * ========================================================= */

    /// <summary>
    /// PlayerがSplineへ着地した際のエフェクトを再生する。
    ///
    /// 着地点にローカルなBurstエフェクトを再生し、
    /// 未訪問Splineへの初回着地であれば
    /// Spline全体へ広がるScatterエフェクトも再生する。
    /// </summary>
    /// <param name="spline">
    /// Playerが着地したSpline。
    /// </param>
    /// <param name="distance">
    /// Spline始点から着地点までの距離。
    /// </param>
    /// <param name="hitWorldPos">
    /// 接触判定によって求められた着地点のワールド座標。
    ///
    /// 現在の実装では使用されていないが、
    /// BurstAtWorldPosなどを使用する場合に利用できる。
    /// </param>
    public void PlaySplineAttachFx(
        ClosedSplineLine spline,
        float distance,
        Vector3 hitWorldPos)
    {
        // 着地先Splineが存在しなければ処理できない。
        if (spline == null)
            return;

        /*
         * 着地したSplineから、
         * 着地エフェクトを管理するSplineBurstEmitterを取得する。
         */
        var emitter =
            spline.GetComponent<SplineBurstEmitter>();

        // SplineBurstEmitterが付いていなければ何もしない。
        if (emitter == null)
            return;

        /*
         * Spline始点からdistance進んだ地点で、
         * ローカルな着地Burstエフェクトを再生する。
         */
        emitter.BurstLocalAtDistance(distance);

        /*
         * このSplineが未訪問かつ開始地点ではない場合、
         * Spline全体に広がるエフェクトを再生する。
         *
         * IsNewOrbitがいつfalseへ変更されるかによっては、
         * この条件が成立しない可能性があるため、
         * AttachEvent側の呼び出し順には注意が必要。
         */
        if (spline.IsNewOrbit && !spline.IsStartSpline)
        {
            emitter.ScatterGlobal();
        }

        /*
         * 接触地点のワールド座標を直接使って
         * Burstを再生する場合の候補。
         *
         * 現在は使用されていない。
         */
        //emitter.BurstAtWorldPos(hitWorldPos);
    }


    /* =========================================================
     * Player死亡演出
     * ========================================================= */

    /// <summary>
    /// Player死亡時のエフェクトと消失アニメーションを再生する。
    ///
    /// 処理の流れは以下。
    ///
    /// 1. GameObject破棄時にキャンセルされるCancellationTokenを取得
    /// 2. ParticleSystemを再生
    /// 3. PlayerDisappearAnimationを再生して完了を待つ
    /// 4. ParticleSystemが完全に終了するまで待つ
    /// 5. PlayerのGameObjectを非アクティブ化
    ///
    /// GameObjectが途中で破棄された場合は、
    /// OperationCanceledExceptionを捕捉して静かに終了する。
    /// </summary>
    public async UniTask PlayDeadEffect()
    {
        /*
         * このMonoBehaviourが破棄されたときに
         * 自動的にキャンセルされるCancellationTokenを取得する。
         *
         * シーン遷移中やGameObject破棄後も
         * 非同期処理が残り続けることを防ぐ。
         */
        var ct =
            this.GetCancellationTokenOnDestroy();

        try
        {
            /*
             * Unityオブジェクトは破棄されると
             * C#上では参照が残っていてもthis == nullになる場合がある。
             */
            if (this == null)
                return;

            // 死亡ParticleSystemが未設定なら再生できない。
            if (_deadEffect == null)
                return;

            // 死亡パーティクルを再生する。
            _deadEffect.Play();

            /*
             * Player本体の消失アニメーションが設定されている場合、
             * アニメーションを開始して完了まで待つ。
             */
            if (_playerDisappearAnimation != null)
            {
                await _playerDisappearAnimation
                    .PlayAsync()

                    /*
                     * GameObjectが破棄された場合に
                     * 待機処理をキャンセルできるようにする。
                     */
                    .AttachExternalCancellation(ct);
            }

            /*
             * ParticleSystemが完全に終了するまで待つ。
             *
             * IsAlive(true)のtrueは、
             * 子ParticleSystemも含めて再生中か確認する指定。
             *
             * ParticleSystem自体が途中で破棄された場合も
             * 待機を終了できるよう、nullチェックを含めている。
             */
            await UniTask.WaitUntil(
                () =>
                    _deadEffect == null
                    || !_deadEffect.IsAlive(true),
                cancellationToken: ct
            );

            // 待機中にこのGameObjectが破棄された場合は終了する。
            if (this == null)
                return;

            /*
             * 死亡演出がすべて終了したため、
             * PlayerのGameObject全体を非アクティブ化する。
             */
            gameObject.SetActive(false);
        }
        catch (OperationCanceledException)
        {
            /*
             * GameObject破棄やシーン遷移によって
             * CancellationTokenがキャンセルされた場合。
             *
             * 想定内の終了なので、エラーとして出力せず無視する。
             */
        }
        catch (Exception e)
        {
            /*
             * キャンセル以外の予期しない例外が発生した場合は、
             * Unityコンソールへスタックトレース付きで出力する。
             */
            Debug.LogException(e);
        }
    }
}

