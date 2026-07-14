using UnityEngine;

/// <summary>
/// PlayerControllerから移動処理を分離したクラス。
///
/// 主に以下の処理を担当する。
/// ・PlayerをSpline上に配置する
/// ・Splineに沿ってPlayerを周回させる
/// ・SplineからPlayerをジャンプさせる
/// ・ジャンプ中に別のSplineとの接触を判定する
/// ・接触したSplineへPlayerを吸着させる
/// ・ゲーム開始時に障害物と重ならない初期位置を探す
/// </summary>
internal class PlayerSplineMover
{
    /* =========================================================
     * Spline吸着設定
     * ========================================================= */

    /// <summary>
    /// ジャンプ中にSplineへ接触したとみなす半径。
    ///
    /// Playerの移動線とSplineの距離がこの値以下になった場合、
    /// Splineへ接触したと判定される。
    /// </summary>
    private const float AttachRadius = 0.1f;

    /// <summary>
    /// Splineへ接触してから、実際のSpline上の位置へ
    /// 補間移動するまでの時間。
    ///
    /// 接触位置へ瞬間移動すると不自然に見えるため、
    /// ごく短時間だけ補間して吸着させる。
    /// </summary>
    private const float AttachDuration = 0.03f;


    /* =========================================================
     * 依存オブジェクト
     * ========================================================= */

    /// <summary>
    /// PlayerのTransformや見た目に関する操作を担当するView。
    /// Playerの座標変更や、現在のSpline情報の更新に使用する。
    /// </summary>
    private readonly PlayerView _view;

    /// <summary>
    /// Playerの移動速度、ジャンプ速度、周回方向などの
    /// 状態・数値を保持するModel。
    /// </summary>
    private readonly PlayerModel _model;

    /// <summary>
    /// Player機能の外部にあるシステムを利用するためのFacade。
    ///
    /// このクラスでは主に以下の処理に使用する。
    /// ・接触可能なSplineを検索する
    /// ・ブラックホールなどによってジャンプ方向を曲げる
    /// </summary>
    private readonly IPlayerExternalFacade _playerExternalFacade;

    /// <summary>
    /// Splineへの吸着が発生した際のイベント処理を担当する。
    ///
    /// 例えば以下の処理が含まれる。
    /// ・新しいSplineへ乗ったことの通知
    /// ・ロングジャンプ判定
    /// ・Spline着地時の演出
    /// </summary>
    private readonly AttachEvent _attachEvent;

    /// <summary>
    /// ゲーム開始時にPlayerがObstacleやPointObjectと
    /// 重なっていないかを調べるクラス。
    /// </summary>
    private readonly PlayerSpawnOverlapResolver _spawnOverlapResolver;


    /* =========================================================
     * 初期配置位置の検索設定
     * ========================================================= */

    /// <summary>
    /// 初期位置に障害物などが存在するかを判定するときの半径。
    /// </summary>
    private const float SpawnCheckRadius = 0.3f;

    /// <summary>
    /// 安全な初期位置を探す際に、
    /// Spline上を1回ごとにずらす距離。
    /// </summary>
    private const float SpawnSearchStep = 0.2f;

    /// <summary>
    /// 安全な初期位置を検索する最大回数。
    ///
    /// 前方向と後方向の両方を調べるため、
    /// 実際には最大でこの値の約2倍の位置を確認する。
    /// </summary>
    private const int SpawnSearchMaxStep = 50;


    /* =========================================================
     * 現在のSpline移動状態
     * ========================================================= */

    /// <summary>
    /// Playerが現在所属しているSpline。
    ///
    /// ジャンプ中も、次のSplineへ吸着するまでは
    /// ジャンプ元のSplineが保持される。
    /// </summary>
    private ClosedSplineLine _currentSpline;

    /// <summary>
    /// 現在のSplineの全長。
    ///
    /// Spline上の距離を循環させる処理や、
    /// Splineが正しく構築されているかの判定に使用する。
    /// </summary>
    private float _totalLen;

    /// <summary>
    /// 現在のSpline上におけるPlayerの位置。
    ///
    /// ワールド座標ではなく、
    /// 「Splineの始点から何m進んだ地点か」を表す。
    ///
    /// 0 ～ _totalLen の範囲で循環する。
    /// </summary>
    private float _distance;


    /* =========================================================
     * Splineへの吸着状態
     * ========================================================= */

    /// <summary>
    /// 現在、Splineへの吸着補間中かどうか。
    /// </summary>
    private bool _isAttaching;

    /// <summary>
    /// 吸着補間の進行度。
    ///
    /// 0で吸着開始、1で吸着完了を表す。
    /// </summary>
    private float _attachT;

    /// <summary>
    /// 吸着補間を開始したときのPlayerのワールド座標。
    /// </summary>
    private Vector3 _attachFrom;

    /// <summary>
    /// 吸着先となるSpline上のワールド座標。
    /// </summary>
    private Vector3 _attachTo;


    /* =========================================================
     * ジャンプ状態
     * ========================================================= */

    /// <summary>
    /// 現在Playerがジャンプ中かどうか。
    /// </summary>
    private bool _isJumping;

    /// <summary>
    /// 現在のジャンプ方向。
    ///
    /// ジャンプ開始時はSplineの外向き法線になる。
    /// ジャンプ中はブラックホールの引力などによって
    /// 毎フレーム曲げられる可能性がある。
    /// </summary>
    private Vector3 _jumpDir;

    /// <summary>
    /// 現在のジャンプ速度。
    ///
    /// ジャンプ開始時にPlayerModelから取得する。
    /// </summary>
    private float _jumpSpeed;

    /// <summary>
    /// ジャンプ中のPlayerの現在ワールド座標。
    ///
    /// PlayerViewのTransformから毎回取得するのではなく、
    /// この変数を移動計算上の基準として使用する。
    /// </summary>
    private Vector3 _jumpPos;

    /// <summary>
    /// ジャンプを開始した地点のワールド座標。
    ///
    /// ジャンプ開始地点と着地点の直線距離を
    /// 計算する場合などに使用できる。
    /// </summary>
    private Vector3 _jumpStartPos;

    /// <summary>
    /// ジャンプ開始後にPlayerが実際に移動した累積距離。
    ///
    /// ブラックホールによって軌道が曲がった場合でも、
    /// 各フレームの移動距離を加算するため、
    /// 実際に通過した経路の長さを表す。
    /// </summary>
    private float _jumpTravelDistance;


    /* =========================================================
     * コンストラクタ
     * ========================================================= */

    /// <summary>
    /// PlayerSplineMoverを初期化する。
    /// </summary>
    /// <param name="view">
    /// Playerの座標やSpline情報を反映するView。
    /// </param>
    /// <param name="model">
    /// 移動速度やジャンプ速度などを保持するModel。
    /// </param>
    /// <param name="attachEvent">
    /// Spline吸着時のイベント処理。
    /// </param>
    /// <param name="playerExternalFacade">
    /// Spline検索やブラックホールの引力計算など、
    /// 外部機能へアクセスするFacade。
    /// </param>
    /// <param name="spawnOverlapResolver">
    /// 初期位置に障害物が重なっているかを判定するクラス。
    /// </param>
    internal PlayerSplineMover(
        PlayerView view,
        PlayerModel model,
        AttachEvent attachEvent,
        IPlayerExternalFacade playerExternalFacade,
        PlayerSpawnOverlapResolver spawnOverlapResolver)
    {
        _view = view;
        _model = model;
        _attachEvent = attachEvent;
        _playerExternalFacade = playerExternalFacade;
        _spawnOverlapResolver = spawnOverlapResolver;

        // PlayerViewに初期設定されているSplineを
        // 現在のSplineとして保持する。
        _currentSpline = _view.Spline;
    }


    /* =========================================================
     * 初期化
     * ========================================================= */

    /// <summary>
    /// PlayerSplineMoverの初期化処理。
    ///
    /// 現在のSplineから全長を取得し、
    /// 移動処理に必要な情報を準備する。
    /// </summary>
    public void Initialize()
    {
        InitializeMove();
    }

    /// <summary>
    /// 現在のSplineの全長を取得し、
    /// Spline移動に必要な情報を初期化する。
    /// </summary>
    public void InitializeMove()
    {
        RebuildTable();

        // Splineの全長がほぼ0の場合、
        // Splineが正常に構築されていない可能性がある。
        if (_totalLen <= 0.0001f)
        {
            Debug.LogError(
                "[PlayerSplineMover] InitializeMove failed: " +
                "spline samples are empty");
        }
    }


    /* =========================================================
     * 通常の更新処理
     * ========================================================= */

    /// <summary>
    /// 毎フレームPlayerの移動処理を更新する。
    ///
    /// ジャンプ中の移動はTickJumpで処理するため、
    /// このメソッドでは更新しない。
    ///
    /// 吸着中の場合はSpline上の通常移動を停止し、
    /// 吸着補間のみを実行する。
    /// </summary>
    /// <param name="deltaTime">
    /// 前フレームからの経過時間。
    /// </param>
    public void Tick(float deltaTime)
    {
        // Splineの長さが無効な場合は移動できない。
        if (_totalLen <= 0.0001f)
            return;

        // ジャンプ中の移動はTickJump側で処理する。
        if (_isJumping)
            return;

        // Splineへの吸着補間中の場合は、
        // 通常のSpline移動を行わない。
        if (_isAttaching)
        {
            TickAttach(deltaTime);
            return;
        }

        // 通常時はSpline上を周回する。
        TickSplineMove(deltaTime);
    }


    /* =========================================================
     * Playerの初期配置
     * ========================================================= */

    /// <summary>
    /// Playerを指定されたSpline上へ配置する。
    ///
    /// 主にゲーム開始時やリトライ時の
    /// Player初期配置に使用する。
    ///
    /// 配置位置にObstacleやPointObjectが重なっている場合は、
    /// Spline上から安全な位置を検索する。
    /// </summary>
    /// <param name="spline">
    /// Playerを配置するSpline。
    /// </param>
    /// <param name="distance">
    /// Splineの始点からの配置距離。
    /// デフォルトでは始点である0を使用する。
    /// </param>
    public void SetPlayer(
        ClosedSplineLine spline,
        float distance = 0f)
    {
        // 配置先のSplineが存在しなければ処理できない。
        if (spline == null)
        {
            Debug.LogError(
                "[PlayerSplineMover] SetPlayer failed: spline is null");
            return;
        }

        // 現在のSplineを更新する。
        _currentSpline = spline;

        // PlayerView側にも所属Splineを設定する。
        _view.SetSpline(spline);

        // 新しいSplineの全長を取得する。
        _totalLen = _currentSpline.GetTotalLength();

        // Splineの長さがほぼ0なら、
        // 正常な位置評価や周回処理ができない。
        if (_totalLen <= 0.0001f)
        {
            Debug.LogError(
                "[PlayerSplineMover] SetPlayer failed: " +
                "spline length is invalid");
            return;
        }

        // distanceがSplineの全長を超えていた場合でも、
        // 0～Spline全長の範囲に循環させる。
        _distance = Mathf.Repeat(distance, _totalLen);

        // 指定された初期位置にObstacleやPointObjectがある場合、
        // 前後にずらしながら安全な位置を探す。
        _distance = FindSafeSpawnDistance(_distance);

        // 初期配置時はジャンプ状態と吸着状態を解除する。
        _isJumping = false;
        _isAttaching = false;

        // 計算したSpline上の位置を
        // PlayerViewの実際の座標へ反映する。
        ApplyPosition();
    }

    /// <summary>
    /// PlayerがObstacleやPointObjectと重ならない、
    /// 安全な初期位置をSpline上から検索する。
    ///
    /// 最初に指定位置を確認し、
    /// 塞がっている場合は前方向、後方向の順で
    /// 少しずつ距離をずらして検索する。
    /// </summary>
    /// <param name="startDistance">
    /// 検索開始位置となるSpline上の距離。
    /// </param>
    /// <returns>
    /// 安全だと判定されたSpline上の距離。
    ///
    /// 安全な位置が見つからなかった場合は
    /// 元のstartDistanceを返す。
    /// </returns>
    private float FindSafeSpawnDistance(float startDistance)
    {
        // 重なり判定を担当するResolverが存在しない場合は、
        // 安全位置検索を行わず指定位置をそのまま使用する。
        if (_spawnOverlapResolver == null)
            return startDistance;

        // 最初に指定された位置が塞がっていなければ、
        // その位置をそのまま使用する。
        if (!IsDistanceBlocked(startDistance))
            return startDistance;

        // 指定位置が塞がっていた場合、
        // 検索距離を少しずつ広げながら前後を確認する。
        for (int i = 1; i <= SpawnSearchMaxStep; i++)
        {
            // 開始位置からSplineの進行方向側へずらした位置。
            float forwardDistance = Mathf.Repeat(
                startDistance + SpawnSearchStep * i,
                _totalLen
            );

            // 前方向の位置が塞がっていなければ採用する。
            if (!IsDistanceBlocked(forwardDistance))
                return forwardDistance;

            // 開始位置からSplineの逆方向側へずらした位置。
            float backwardDistance = Mathf.Repeat(
                startDistance - SpawnSearchStep * i,
                _totalLen
            );

            // 後方向の位置が塞がっていなければ採用する。
            if (!IsDistanceBlocked(backwardDistance))
                return backwardDistance;
        }

        // 最大回数まで検索しても安全な位置が見つからなかった場合、
        // 警告を出して元の位置を使用する。
        Debug.LogWarning(
            "[PlayerSplineMover] 安全な初期位置が見つかりませんでした。");

        return startDistance;
    }

    /// <summary>
    /// 指定したSpline上の距離に、
    /// Playerの配置を妨げるオブジェクトがあるかを判定する。
    /// </summary>
    /// <param name="distance">
    /// 判定対象となるSpline上の距離。
    /// </param>
    /// <returns>
    /// 障害物などが存在する場合はtrue。
    /// 配置可能な場合はfalse。
    /// </returns>
    private bool IsDistanceBlocked(float distance)
    {
        // Spline上の距離をワールド座標へ変換する。
        Vector3 pos =
            _currentSpline.EvaluateByDistance(distance);

        // 指定した座標の周囲にObstacleやPointObjectが
        // 存在するかをResolverへ問い合わせる。
        return _spawnOverlapResolver.IsBlocked(
            pos,
            SpawnCheckRadius
        );
    }


    /* =========================================================
     * ジャンプ処理
     * ========================================================= */

    /// <summary>
    /// 現在のSplineからジャンプを開始する。
    ///
    /// ジャンプ方向には、現在地点における
    /// Splineの外向き法線を使用する。
    /// </summary>
    public void StartJump()
    {
        // ジャンプ状態へ移行する。
        _isJumping = true;

        // 現在のPlayer座標をジャンプ計算の開始位置にする。
        _jumpPos = _view.transform.position;

        // ロングジャンプなどの判定に使用するため、
        // ジャンプ開始地点を保存する。
        _jumpStartPos = _jumpPos;

        // 新しいジャンプなので累積移動距離をリセットする。
        _jumpTravelDistance = 0f;

        // 現在地点のSpline外向き法線を
        // ジャンプの初期方向として設定する。
        _jumpDir = GetOuterNormal().normalized;

        // 現在のPlayerModelからジャンプ速度を取得する。
        _jumpSpeed = _model.CurrentJumpspeed;
    }

    /// <summary>
    /// ジャンプ中のPlayerを1フレーム分移動させる。
    ///
    /// 移動後、移動前座標から移動後座標までの範囲で
    /// 別のSplineに接触していないかを調べる。
    /// </summary>
    /// <param name="dt">
    /// 前フレームからの経過時間。
    /// </param>
    /// <returns>
    /// 別のSplineへ接触し、吸着処理を開始した場合はtrue。
    /// 接触しなかった場合、またはジャンプ中でない場合はfalse。
    /// </returns>
    public bool TickJump(float dt)
    {
        // ジャンプ中でなければ更新する必要がない。
        if (!_isJumping)
            return false;

        // ブラックホールなどの影響を反映し、
        // 現在のジャンプ方向を更新する。
        BendJumpDirection(dt);

        // 移動前の位置を保存する。
        //
        // この位置から移動後の位置までを線分として、
        // Splineとの接触判定に使用する。
        Vector3 prevPos = _jumpPos;

        // ジャンプ方向、速度、経過時間から
        // 新しいPlayer座標を計算する。
        _jumpPos += _jumpDir * _jumpSpeed * dt;

        // このフレームで実際に移動した距離を
        // ジャンプの累積移動距離へ加算する。
        //
        // ジャンプ方向が曲がった場合でも、
        // 実際に通った経路の長さを計測できる。
        _jumpTravelDistance +=
            Vector3.Distance(prevPos, _jumpPos);

        // 計算したジャンプ位置をPlayerViewへ反映する。
        _view.SetPosition(_jumpPos);

        // 移動前から移動後までの線分に接触したSplineを探す。
        //
        // _currentSplineはジャンプ元なので、
        // 再び同じSplineへ即座に接触しないよう検索対象から除外する。
        if (!_playerExternalFacade.TryFindTouchedSpline(
                prevPos,
                _jumpPos,
                AttachRadius,
                _currentSpline,
                out var touchedSpline,
                out float hitDistanceOnSpline,
                out Vector3 hitPointOnSpline))
        {
            // 接触したSplineがなければジャンプを継続する。
            return false;
        }

        // ジャンプ開始地点から接触地点までの直線距離。
        //
        // 現在のコードではこの値は以降使用されていない。
        // 直線距離による判定が不要なら削除できる。
        float jumpDistance =
            Vector3.Distance(_jumpStartPos, hitPointOnSpline);

        // 接触したSplineを現在のSplineとして設定し、
        // Spline上の接触地点へ吸着させる。
        AttachToSpline(
            touchedSpline,
            hitDistanceOnSpline,
            hitPointOnSpline
        );

        // 実際に移動した累積経路距離を使って、
        // ロングジャンプだったかを判定する。
        _attachEvent.CheckLongJumped(_jumpTravelDistance);

        // Splineへ接触したためジャンプ状態を終了する。
        _isJumping = false;

        return true;
    }

    /// <summary>
    /// ブラックホールなどの外部要素によって、
    /// ジャンプ方向を曲げる。
    /// </summary>
    /// <param name="dt">
    /// 前フレームからの経過時間。
    /// </param>
    private void BendJumpDirection(float dt)
    {
        // Facadeが存在しなければ方向を変更しない。
        if (_playerExternalFacade == null)
            return;

        // 現在位置と現在方向を外部システムへ渡し、
        // 曲げられた後の方向を取得する。
        _jumpDir = _playerExternalFacade.BendDirection(
            _jumpPos,
            _jumpDir,
            dt
        );
    }


    /* =========================================================
     * Spline吸着処理
     * ========================================================= */

    /// <summary>
    /// ジャンプ中に接触した新しいSplineへPlayerを吸着させる。
    /// </summary>
    /// <param name="newSpline">
    /// 接触した新しいSpline。
    /// </param>
    /// <param name="hitDistanceOnSpline">
    /// 新しいSplineの始点から接触地点までの距離。
    ///
    /// この値を_currentSpline.EvaluateByDistanceへ渡すことで、
    /// Spline上の正確な吸着位置を取得できる。
    /// </param>
    /// <param name="hitPointWorld">
    /// 接触判定によって求められた、
    /// Spline上の接触地点のワールド座標。
    ///
    /// AttachEventへの通知に使用する。
    /// </param>
    private void AttachToSpline(
        ClosedSplineLine newSpline,
        float hitDistanceOnSpline,
        Vector3 hitPointWorld)
    {
        // PlayerViewが保持する所属Splineを更新する。
        _view.SetSpline(newSpline);

        // このクラスが管理する現在のSplineも更新する。
        _currentSpline = newSpline;

        // 新しいSpline上の接触距離を現在位置として保持する。
        _distance = hitDistanceOnSpline;

        // 新しいSplineの全長を取得する。
        _totalLen = _currentSpline.GetTotalLength();

        // 吸着補間の開始位置には、
        // 接触判定後の現在のPlayer座標を使用する。
        _attachFrom = _view.transform.position;

        // Spline上の距離をワールド座標へ変換し、
        // 吸着補間の終了位置として使用する。
        _attachTo =
            _currentSpline.EvaluateByDistance(_distance);

        // 吸着補間の進行度を最初に戻す。
        _attachT = 0f;

        // 吸着状態を開始する。
        _isAttaching = true;

        // Splineへ吸着したことをイベント側へ通知する。
        //
        // 新規Spline判定、着地演出、スコア処理などは
        // AttachEvent側で実行される。
        _attachEvent.OnSplineAttached(
            _currentSpline,
            _distance,
            hitPointWorld
        );
    }

    /// <summary>
    /// Spline接触地点から、Spline上の正確な位置まで
    /// Playerを短時間で補間移動させる。
    /// </summary>
    /// <param name="deltaTime">
    /// 前フレームからの経過時間。
    /// </param>
    private void TickAttach(float deltaTime)
    {
        // AttachDuration秒で0から1になるように
        // 補間の進行度を加算する。
        _attachT += deltaTime / AttachDuration;

        // SmoothStepを使って、
        // 開始時と終了時が滑らかになる補間値へ変換する。
        float t = Mathf.SmoothStep(
            0f,
            1f,
            _attachT
        );

        // 接触時の位置からSpline上の正確な位置まで補間する。
        Vector3 pos = Vector3.Lerp(
            _attachFrom,
            _attachTo,
            t
        );

        // 補間後の位置をPlayerViewへ反映する。
        _view.SetPosition(pos);

        // 補間が完了したら吸着状態を終了する。
        //
        // 次のフレームから通常のSpline周回処理へ移行する。
        if (_attachT >= 1f)
        {
            _isAttaching = false;
        }
    }


    /* =========================================================
     * Spline上の周回処理
     * ========================================================= */

    /// <summary>
    /// Playerを現在のSplineに沿って移動させる。
    /// </summary>
    /// <param name="deltaTime">
    /// 前フレームからの経過時間。
    /// </param>
    private void TickSplineMove(float deltaTime)
    {
        // Clockwiseがtrueなら距離を減少させ、
        // falseなら距離を増加させる。
        //
        // Splineの生成順によって時計回り・反時計回りの
        // 実際の見え方が逆になる可能性はある。
        float dir = _model.Clockwise
            ? -1f
            : 1f;

        // 移動方向、現在速度、経過時間から
        // Spline上の距離を更新する。
        //
        // Mathf.Repeatによって、
        // Spline終端まで進んだら始点へ循環する。
        _distance = Mathf.Repeat(
            _distance
            + dir
            * _model.CurrentMoveSpeed
            * deltaTime,
            _totalLen
        );

        // 更新したSpline上の距離を
        // Playerのワールド座標へ反映する。
        ApplyPosition();
    }


    /* =========================================================
     * Spline情報の更新・座標反映
     * ========================================================= */

    /// <summary>
    /// 現在のSplineから全長を取得する。
    ///
    /// Splineが設定されていない場合は
    /// 全長を0として扱う。
    /// </summary>
    private void RebuildTable()
    {
        if (_currentSpline == null)
        {
            _totalLen = 0f;
            return;
        }

        _totalLen = _currentSpline.GetTotalLength();
    }

    /// <summary>
    /// 現在のSplineと_distanceからワールド座標を計算し、
    /// PlayerViewへ反映する。
    /// </summary>
    private void ApplyPosition()
    {
        // Splineの始点から_distance進んだ地点の
        // ワールド座標を取得する。
        Vector3 pos =
            _currentSpline.EvaluateByDistance(_distance);

        // 計算した座標へPlayerを移動させる。
        _view.SetPosition(pos);
    }

    /// <summary>
    /// Playerの現在位置における、
    /// Splineの外向き法線を取得する。
    ///
    /// 主にジャンプ開始方向として使用する。
    /// </summary>
    /// <returns>
    /// 現在地点における外向き法線ベクトル。
    /// </returns>
    public Vector3 GetOuterNormal()
    {
        return _currentSpline
            .EvaluateNormalByDistance(_distance);
    }


    /* =========================================================
     * 公開状態
     * ========================================================= */

    /// <summary>
    /// Playerが現在ジャンプ中かどうか。
    /// </summary>
    public bool IsJumping => _isJumping;
}