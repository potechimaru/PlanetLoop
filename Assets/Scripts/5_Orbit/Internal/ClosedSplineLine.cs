using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// 閉じたSpline軌道を生成・描画・管理するクラス。
///
/// 主に以下の処理を担当する。
///
/// ・制御点からCatmull-Rom曲線を生成する
/// ・生成した曲線をLineRendererで描画する
/// ・Spline全体の長さと累積距離を計算する
/// ・指定距離に対応するSpline上の座標を取得する
/// ・指定距離における外向き法線を取得する
/// ・Spline上のPointObjectを周回移動させる
/// ・Playerのジャンプ軌道とSplineの接触を判定する
/// ・開始Spline、新規Spline、着地状態を管理する
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
public class ClosedSplineLine : MonoBehaviour
{
    /* =========================================================
     * 基本設定
     * ========================================================= */

    /// <summary>
    /// Splineを識別するためのID。
    ///
    /// 複数のSplineを区別したり、
    /// スコア処理や訪問済み判定などで利用できる。
    /// </summary>
    [Header("Spline ID")]
    [SerializeField, Min(1)]
    private int splineID = 1;

    /// <summary>
    /// このSplineが現在のゲーム開始地点かどうか。
    ///
    /// OrbitManagerによって、開始候補の中から
    /// ランダムに1つだけtrueへ設定される。
    /// </summary>
    private bool _isStartSpline;

    /// <summary>
    /// このSplineがゲーム開始地点として選ばれているかを取得・設定する。
    /// </summary>
    public bool IsStartSpline
    {
        get => _isStartSpline;
        set => _isStartSpline = value;
    }

    /// <summary>
    /// このSplineを開始地点の候補に含めるかどうか。
    ///
    /// falseの場合、OrbitManagerによる
    /// ランダムな開始Spline選択の対象外になる。
    /// </summary>
    [SerializeField]
    private bool _canBeStartSpline = true;

    /// <summary>
    /// このSplineを開始地点として使用できるかどうか。
    /// </summary>
    public bool CanBeStartSpline => _canBeStartSpline;


    /* =========================================================
     * Spline制御点
     * ========================================================= */

    /// <summary>
    /// Spline形状を決める制御点。
    ///
    /// 各座標は、このGameObjectを基準としたローカル座標で保持される。
    ///
    /// Catmull-Rom補間によって、
    /// これらの制御点を滑らかにつないだ閉曲線を生成する。
    /// </summary>
    [Header("Spline Control Points (Local Space)")]
    [SerializeField]
    private List<Vector2> controlPoints = new()
    {
        new Vector2( 1f,  0f),
        new Vector2( 0f,  1f),
        new Vector2(-1f,  0f),
        new Vector2( 0f, -1f),
    };


    /* =========================================================
     * 自動形状生成設定
     * ========================================================= */

    /// <summary>
    /// 制御点を手動入力せず、
    /// 円または楕円として自動生成するかどうか。
    /// </summary>
    [Header("Auto Shape")]
    [SerializeField]
    private bool useAutoGenerateShape = false;

    /// <summary>
    /// 自動生成するSplineの形状。
    /// </summary>
    [SerializeField]
    private AutoShapeType autoShapeType = AutoShapeType.Circle;

    /// <summary>
    /// 円または楕円を構成する制御点の数。
    ///
    /// 3未満では閉じた形状を作れないため、最小値は3。
    /// 制御点数が多いほど円形に近くなる。
    /// </summary>
    [SerializeField, Min(3)]
    private int autoControlPointCount = 8;

    /// <summary>
    /// 円を自動生成するときの半径。
    /// </summary>
    [SerializeField, Min(0.01f)]
    private float circleRadius = 3f;

    /// <summary>
    /// 楕円を自動生成するときのX方向・Y方向の半径。
    ///
    /// xが横方向の半径、
    /// yが縦方向の半径を表す。
    /// </summary>
    [SerializeField]
    private Vector2 ellipseRadius = new Vector2(4f, 2f);


    /* =========================================================
     * 描画設定
     * ========================================================= */

    /// <summary>
    /// Spline全体を何個程度のサンプル点で近似するか。
    ///
    /// 値が大きいほど曲線が滑らかになるが、
    /// 計算量とLineRendererの頂点数が増える。
    /// </summary>
    [Header("Rendering")]
    [SerializeField, Range(8, 256)]
    private int resolution = 64;

    /// <summary>
    /// Unityエディター上でも毎フレームSplineを再構築するかどうか。
    ///
    /// trueの場合、Inspectorで制御点や半径を変更した際に
    /// 即座に見た目へ反映される。
    ///
    /// ExecuteAlwaysと組み合わせて使用される。
    /// </summary>
    [SerializeField]
    private bool updateInEditor = true;


    /* =========================================================
     * PointObjectの周回設定
     * ========================================================= */

    /// <summary>
    /// Splineの子に配置されたPointObjectを
    /// Splineに沿って周回させるかどうか。
    /// </summary>
    [Header("Point Rotation")]
    [SerializeField]
    private bool rotatePoints = false;

    /// <summary>
    /// PointObjectの周回処理が有効かどうか。
    /// </summary>
    public bool IsPointRotationEnabled => rotatePoints;

    /// <summary>
    /// PointObjectがSpline上を移動する速度。
    ///
    /// 単位は「Spline上の距離 / 秒」。
    /// </summary>
    [Tooltip("距離 / 秒")]
    [SerializeField]
    private float rotationSpeed = 1f;


    /* =========================================================
     * LineRendererのマテリアル
     * ========================================================= */

    /// <summary>
    /// 通常状態で使用するSplineのマテリアル。
    /// </summary>
    [Header("Line Materials")]
    [SerializeField]
    private Material normalMaterial;

    /// <summary>
    /// Playerが着地したときに使用するマテリアル。
    ///
    /// FlashLandingMaterialによって適用される。
    /// </summary>
    [SerializeField]
    private Material landingMaterial;

    /// <summary>
    /// ゲーム開始地点として選択されたSplineに使用するマテリアル。
    /// </summary>
    [SerializeField]
    private Material startSplineMaterial;


    /* =========================================================
     * PointObject周回用の内部状態
     * ========================================================= */

    /// <summary>
    /// PointObject全体をSpline上でどれだけ移動させたかを表す距離。
    ///
    /// 各PointObjectの初期距離にこの値を足すことで、
    /// 配置間隔を維持したまま一斉に周回させる。
    /// </summary>
    private float _distanceOffset = 0f;

    /// <summary>
    /// PointObjectの描画を切り替える対象となるMeshRendererのキャッシュ。
    ///
    /// 毎回GetComponentsInChildrenを呼ばないように保持する。
    /// </summary>
    private MeshRenderer[] _pointMeshRenderers;

    /// <summary>
    /// 現在PointObjectの描画が有効かどうか。
    ///
    /// 同じ状態を重複設定しないために保持する。
    /// </summary>
    private bool _isPointRenderingEnabled = true;

    /// <summary>
    /// Spline上に配置されているPointObjectの位置情報キャッシュ。
    ///
    /// 各要素のBaseDistanceを利用して、
    /// PointObjectをSpline上で周回させる。
    /// </summary>
    private SplinePointData[] _pointDataCache;


    /* =========================================================
     * Spline内部データ
     * ========================================================= */

    /// <summary>
    /// Splineを描画するLineRenderer。
    ///
    /// RequireComponentによって、
    /// このコンポーネントと同じGameObjectに存在することが保証される。
    /// </summary>
    private LineRenderer _lineRenderer;

    /// <summary>
    /// Catmull-Rom曲線をサンプリングして得られた座標一覧。
    ///
    /// すべてローカル座標で保持される。
    ///
    /// 最後の要素には先頭座標がもう一度追加され、
    /// 閉じた線分として扱われる。
    /// </summary>
    private readonly List<Vector3> _positions = new();

    /// <summary>
    /// 各_positions要素までの累積距離。
    ///
    /// 例えば、
    ///
    /// _cumLen[0] = 0
    /// _cumLen[1] = 先頭から2点目までの距離
    /// _cumLen[2] = 先頭から3点目までの距離
    ///
    /// という形で保持される。
    ///
    /// 指定距離からSpline上の座標を検索するときに使用する。
    /// </summary>
    private readonly List<float> _cumLen = new();

    /// <summary>
    /// Spline全体の長さ。
    ///
    /// _positionsの各線分の長さを合計した値。
    /// </summary>
    private float _totalLength;

    /// <summary>
    /// Splineを構成するサンプル点の平均位置。
    ///
    /// 厳密な図形の重心ではないが、
    /// 法線が外向きか内向きかを判定する基準点として使用する。
    ///
    /// ローカル座標で保持される。
    /// </summary>
    private Vector3 _localCenter;


    /* =========================================================
     * Spline訪問状態
     * ========================================================= */

    /// <summary>
    /// Playerがまだ一度もこのSplineへ着地していないかどうか。
    ///
    /// true  : 未訪問
    /// false : 訪問済み
    /// </summary>
    private bool _isNewOrbit = true;

    /// <summary>
    /// このSplineが未訪問かどうかを取得・設定する。
    ///
    /// trueからfalseへ変化した瞬間のみ、
    /// OnPlayerLandedを通知する。
    ///
    /// 同じSplineに何度着地しても、
    /// 初回訪問イベントは1回だけ発生する。
    /// </summary>
    public bool IsNewOrbit
    {
        get => _isNewOrbit;

        set
        {
            // 状態が変わらない場合は何もしない。
            if (_isNewOrbit == value)
                return;

            // 変更前の状態を記録する。
            bool wasNewOrbit = _isNewOrbit;

            // 新しい状態を設定する。
            _isNewOrbit = value;

            // 未訪問から訪問済みに変化した瞬間だけ通知する。
            if (wasNewOrbit && !_isNewOrbit)
            {
                _onPlayerLanded.OnNext(Unit.Default);
            }
        }
    }


    /* =========================================================
     * 子PointObjectの検索範囲
     * ========================================================= */

    /// <summary>
    /// PointObjectやSplinePointDataを検索する親Transform。
    ///
    /// nullの場合は、このClosedSplineLine自身のtransform以下を検索する。
    ///
    /// PointObjectを専用の子オブジェクトへまとめたい場合に指定する。
    /// </summary>
    [SerializeField]
    private Transform _spawnParent;


    /* =========================================================
     * 着地イベント
     * ========================================================= */

    /// <summary>
    /// Playerが初めてこのSplineへ着地したことを通知するSubject。
    /// </summary>
    private Subject<Unit> _onPlayerLanded = new();

    /// <summary>
    /// Playerが初めてこのSplineへ着地したときに通知されるObservable。
    ///
    /// 外部からOnNextを実行できないよう、
    /// IObservableとして公開している。
    /// </summary>
    public IObservable<Unit> OnPlayerLanded => _onPlayerLanded;


    /* =========================================================
     * 自動生成形状
     * ========================================================= */

    /// <summary>
    /// 自動生成可能なSpline形状。
    /// </summary>
    private enum AutoShapeType
    {
        /// <summary>
        /// 正円。
        /// </summary>
        Circle,

        /// <summary>
        /// 楕円。
        /// </summary>
        Ellipse
    }


    /* =========================================================
     * Unityライフサイクル
     * ========================================================= */

    /// <summary>
    /// コンポーネントが有効になったときに呼び出される。
    ///
    /// LineRendererの初期化、
    /// Spline形状の再構築、
    /// PointObject情報のキャッシュを行う。
    /// </summary>
    private void OnEnable()
    {
        // LineRendererを取得し、
        // 閉曲線用の設定を適用する。
        EnsureRenderer();

        // 実行中かつ通常マテリアルが未設定の場合、
        // 現在LineRendererに設定されているマテリアルを
        // 通常マテリアルとして保存する。
        if (Application.isPlaying && normalMaterial == null)
        {
            normalMaterial = _lineRenderer.material;
        }

        // 制御点からSpline座標・累積距離・全長を再計算する。
        Rebuild();

        // 実行中のみPointObject情報をキャッシュする。
        if (Application.isPlaying)
        {
            CachePointData();
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Unityエディター環境での毎フレーム更新。
    ///
    /// Editモードでは必要に応じてSplineを再構築し、
    /// PlayモードではPointObjectを周回させる。
    /// </summary>
    private void Update()
    {
        // ゲーム未実行中かつエディター更新が有効な場合、
        // Inspector変更を即座に反映するためSplineを再構築する。
        if (!Application.isPlaying && updateInEditor)
        {
            Rebuild();
        }

        // ゲーム実行中かつPointObject周回が有効なら、
        // 子PointObjectの位置を更新する。
        if (Application.isPlaying && rotatePoints)
        {
            RotateChildPoints();
        }
    }

#else

    /// <summary>
    /// ビルド環境での毎フレーム更新。
    ///
    /// PointObjectの周回処理のみを実行する。
    /// </summary>
    private void Update()
    {
        if (rotatePoints)
        {
            RotateChildPoints();
        }
    }

#endif


    /* =========================================================
     * 子オブジェクトのキャッシュ
     * ========================================================= */

    /// <summary>
    /// Spline上のPointObjectに付いている
    /// SplinePointDataをまとめて取得し、キャッシュする。
    /// </summary>
    private void CachePointData()
    {
        // spawnParentが指定されている場合はその子を検索し、
        // 未指定の場合はこのGameObject以下を検索する。
        Transform parent =
            _spawnParent != null
                ? _spawnParent
                : transform;

        // 非アクティブな子オブジェクトも含めて取得する。
        _pointDataCache =
            parent.GetComponentsInChildren<SplinePointData>(true);
    }

    /// <summary>
    /// PointObjectの描画を切り替えるため、
    /// 子オブジェクトのMeshRendererをまとめて取得する。
    /// </summary>
    private void CachePointMeshRenderers()
    {
        Transform parent =
            _spawnParent != null
                ? _spawnParent
                : transform;

        // 非アクティブな子オブジェクトも含めて取得する。
        _pointMeshRenderers =
            parent.GetComponentsInChildren<MeshRenderer>(true);
    }


    /* =========================================================
     * PointObjectの描画・処理切り替え
     * ========================================================= */

    /// <summary>
    /// Spline上に配置されたPointObjectの描画を有効・無効にする。
    ///
    /// GameObject自体は無効化せず、
    /// MeshRenderer.enabledのみを切り替える。
    /// </summary>
    /// <param name="enabled">
    /// trueなら描画、falseなら非描画。
    /// </param>
    public void SetPointRenderingEnabled(bool enabled)
    {
        // 既に同じ状態なら更新処理を行わない。
        if (_isPointRenderingEnabled == enabled)
            return;

        _isPointRenderingEnabled = enabled;

        // Rendererがまだ取得されていなければキャッシュする。
        if (_pointMeshRenderers == null)
        {
            CachePointMeshRenderers();
        }

        // 取得できなかった場合は終了する。
        if (_pointMeshRenderers == null)
            return;

        // すべてのPointObjectのRendererを切り替える。
        for (int i = 0; i < _pointMeshRenderers.Length; i++)
        {
            if (_pointMeshRenderers[i] == null)
                continue;

            _pointMeshRenderers[i].enabled = enabled;
        }
    }

    /// <summary>
    /// PointObjectの処理と描画をまとめて有効・無効にする。
    ///
    /// Playerから遠いSplineを軽量化する目的で使用される。
    /// </summary>
    /// <param name="enabled">
    /// trueなら周回処理と描画を有効化する。
    /// falseなら両方を停止する。
    /// </param>
    public void SetPointActivityEnabled(bool enabled)
    {
        SetPointRotationEnabled(enabled);
        SetPointRenderingEnabled(enabled);
    }

    /// <summary>
    /// PointObjectの周回処理を有効・無効にする。
    /// </summary>
    /// <param name="enabled">
    /// trueなら周回し、falseなら停止する。
    /// </param>
    public void SetPointRotationEnabled(bool enabled)
    {
        rotatePoints = enabled;
    }


    /* =========================================================
     * LineRenderer初期化
     * ========================================================= */

    /// <summary>
    /// LineRendererを取得し、
    /// 閉じたローカル座標の線として使用する設定を適用する。
    /// </summary>
    private void EnsureRenderer()
    {
        // まだ取得していない場合のみGetComponentを実行する。
        if (_lineRenderer == null)
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        // 閉曲線として描画する。
        _lineRenderer.loop = true;

        // _positionsはローカル座標で保持しているため、
        // LineRendererもローカル座標を使用する。
        _lineRenderer.useWorldSpace = false;
    }


    /* =========================================================
     * マテリアル変更・状態リセット
     * ========================================================= */

    /// <summary>
    /// このSplineに開始地点用マテリアルを適用する。
    /// </summary>
    public void ApplyStartSplineMaterial()
    {
        EnsureRenderer();

        // 開始地点用マテリアルが未設定なら何もしない。
        if (startSplineMaterial == null)
            return;

        _lineRenderer.material = startSplineMaterial;
    }

    /// <summary>
    /// Splineのゲーム進行状態を初期状態へ戻す。
    ///
    /// ・開始Spline状態を解除
    /// ・未訪問状態へ戻す
    /// ・PointObjectの周回を停止
    /// ・通常マテリアルへ戻す
    /// </summary>
    public void ResetOrbitState()
    {
        IsStartSpline = false;
        IsNewOrbit = true;

        SetPointRotationEnabled(false);

        EnsureRenderer();

        if (normalMaterial != null)
        {
            _lineRenderer.material = normalMaterial;
        }
    }


    /* =========================================================
     * 自動形状生成
     * ========================================================= */

    /// <summary>
    /// CircleまたはEllipse設定に基づき、
    /// 制御点を等間隔に自動生成する。
    /// </summary>
    private void GenerateAutoControlPoints()
    {
        // 閉曲線を成立させるため、最低3点を保証する。
        int count = Mathf.Max(3, autoControlPointCount);

        // リストが存在しなければ生成し、
        // 既に存在する場合は以前の制御点を削除する。
        if (controlPoints == null)
        {
            controlPoints = new List<Vector2>();
        }
        else
        {
            controlPoints.Clear();
        }

        // 360度を制御点数で等分し、
        // 円周または楕円周上へ制御点を配置する。
        for (int i = 0; i < count; i++)
        {
            // 0～2πの範囲で角度を求める。
            float angle = (Mathf.PI * 2f * i) / count;

            float x;
            float y;

            switch (autoShapeType)
            {
                case AutoShapeType.Circle:

                    // 正円上の座標を計算する。
                    x = Mathf.Cos(angle) * circleRadius;
                    y = Mathf.Sin(angle) * circleRadius;
                    break;

                case AutoShapeType.Ellipse:

                    // X軸とY軸で別々の半径を使用して
                    // 楕円上の座標を計算する。
                    x = Mathf.Cos(angle) * ellipseRadius.x;
                    y = Mathf.Sin(angle) * ellipseRadius.y;
                    break;

                default:

                    // 想定外の値の場合は円として生成する。
                    x = Mathf.Cos(angle) * circleRadius;
                    y = Mathf.Sin(angle) * circleRadius;
                    break;
            }

            controlPoints.Add(new Vector2(x, y));
        }
    }


    /* =========================================================
     * Spline再構築
     * ========================================================= */

    /// <summary>
    /// 制御点からCatmull-Rom曲線を生成し、
    /// Splineの内部データとLineRendererを更新する。
    ///
    /// ここで以下のデータが作成される。
    ///
    /// ・_positions：Splineを構成するサンプル座標
    /// ・_cumLen：各座標までの累積距離
    /// ・_totalLength：Spline全長
    /// ・_localCenter：外向き法線判定用の中心座標
    /// </summary>
    public void Rebuild()
    {
        // 自動生成が有効なら、
        // 既存の制御点を円または楕円の制御点に置き換える。
        if (useAutoGenerateShape)
        {
            GenerateAutoControlPoints();
        }

        // 閉曲線には最低3つの制御点が必要。
        if (controlPoints == null || controlPoints.Count < 3)
            return;

        EnsureRenderer();

        // 前回生成したSplineデータを削除する。
        _positions.Clear();
        _cumLen.Clear();

        int count = controlPoints.Count;

        // Spline全体のresolutionを制御点数で分割し、
        // 1区間あたりのサンプル数を決める。
        //
        // 最低でも各区間1サンプルは生成する。
        int segmentResolution =
            Mathf.Max(1, resolution / count);

        // 現在までの累積距離。
        float acc = 0f;

        // 各制御点間の曲線区間を生成する。
        for (int i = 0; i < count; i++)
        {
            /*
             * Catmull-Rom曲線では、1区間を計算するために
             * 連続する4つの制御点を使用する。
             *
             * 実際に補間される区間はp1からp2。
             *
             * 閉曲線なので、配列の先頭・末尾をまたぐ場合は
             * %演算で循環させる。
             */

            Vector2 p0 =
                controlPoints[(i - 1 + count) % count];

            Vector2 p1 =
                controlPoints[i];

            Vector2 p2 =
                controlPoints[(i + 1) % count];

            Vector2 p3 =
                controlPoints[(i + 2) % count];

            // p1～p2の区間を複数の点へ分割する。
            for (int j = 0; j < segmentResolution; j++)
            {
                // 区間内の補間率。
                //
                // 0に近いほどp1側、
                // 1に近いほどp2側。
                float t = j / (float)segmentResolution;

                // Catmull-Rom曲線上のローカル2D座標を取得する。
                Vector2 local2D =
                    CatmullRom(p0, p1, p2, p3, t);

                // 2DゲームなのでZ座標は0とする。
                Vector3 local3D =
                    new Vector3(local2D.x, local2D.y, 0f);

                // 2点目以降の場合、
                // 直前のサンプル点との距離を累積する。
                if (_positions.Count > 0)
                {
                    acc += Vector3.Distance(
                        _positions[_positions.Count - 1],
                        local3D
                    );
                }

                // サンプル座標と、その座標までの累積距離を保存する。
                _positions.Add(local3D);
                _cumLen.Add(acc);
            }
        }

        // 最後の点から最初の点へ戻る線分を追加し、
        // Splineを閉じる。
        if (_positions.Count > 0)
        {
            acc += Vector3.Distance(
                _positions[^1],
                _positions[0]
            );

            // 先頭座標を末尾へ再度追加する。
            _positions.Add(_positions[0]);

            // Spline全長を最後の累積距離として追加する。
            _cumLen.Add(acc);
        }

        // 計算した累積距離をSpline全長として保存する。
        _totalLength = acc;

        /*
         * Splineサンプル点の平均位置を計算する。
         *
         * 末尾には先頭点が重複しているため、
         * _positions.Count - 1個を対象にする。
         */
        _localCenter = Vector3.zero;

        int n = Mathf.Max(1, _positions.Count - 1);

        for (int i = 0; i < n; i++)
        {
            _localCenter += _positions[i];
        }

        _localCenter /= n;

        // 計算した座標をLineRendererへ反映する。
        _lineRenderer.positionCount = _positions.Count;
        _lineRenderer.SetPositions(_positions.ToArray());
    }


    /* =========================================================
     * PointObject周回処理
     * ========================================================= */

    /// <summary>
    /// SplinePointDataを持つ子PointObjectを
    /// Spline上で1フレーム分移動させる。
    /// </summary>
    private void RotateChildPoints()
    {
        // Spline全長が無効なら位置を計算できない。
        if (_totalLength <= 0f)
            return;

        // PointObjectが存在しなければ処理しない。
        if (_pointDataCache == null ||
            _pointDataCache.Length == 0)
        {
            return;
        }

        // 経過時間と速度に応じて、
        // 全PointObject共通の移動距離を加算する。
        _distanceOffset += rotationSpeed * Time.deltaTime;

        // Spline全長の範囲で循環させる。
        _distanceOffset =
            Mathf.Repeat(_distanceOffset, _totalLength);

        // 各PointObjectの座標と回転を更新する。
        for (int i = 0; i < _pointDataCache.Length; i++)
        {
            SplinePointData data = _pointDataCache[i];

            if (data == null)
                continue;

            /*
             * 各PointObject固有の初期距離に
             * 全体の移動距離を加算する。
             *
             * これにより、PointObject間の配置間隔を維持したまま
             * 一斉に周回させられる。
             */
            float d =
                data.BaseDistance + _distanceOffset;

            // Spline上の距離からワールド座標を取得する。
            Vector3 pos =
                EvaluateByDistance(d);

            // その位置における外向き法線を取得する。
            Vector3 normal =
                EvaluateNormalByDistance(d);

            Transform pointTransform = data.transform;

            // PointObjectをSpline上へ配置する。
            pointTransform.position = pos;

            /*
             * PointObjectの上方向をSplineの外向き法線へ合わせる。
             *
             * Vector3.upからnormalへ向く回転を作成している。
             */
            pointTransform.rotation =
                Quaternion.FromToRotation(
                    Vector3.up,
                    normal
                );
        }
    }


    /* =========================================================
     * 距離ベースのSpline API
     * ========================================================= */

    /// <summary>
    /// Spline全体の長さを取得する。
    /// </summary>
    /// <returns>
    /// Splineの全長。
    /// </returns>
    public float GetTotalLength()
    {
        return _totalLength;
    }

    /// <summary>
    /// Splineの始点から指定距離だけ進んだ地点の
    /// ワールド座標を取得する。
    /// </summary>
    /// <param name="distance">
    /// Spline始点からの距離。
    ///
    /// Spline全長を超えた場合や負数の場合も、
    /// Mathf.RepeatによってSpline内へ循環される。
    /// </param>
    /// <returns>
    /// 指定距離に対応するSpline上のワールド座標。
    /// </returns>
    public Vector3 EvaluateByDistance(float distance)
    {
        // Splineを構成する点が不足している場合、
        // GameObject自身の座標を返す。
        if (_positions.Count < 2)
            return transform.position;

        // 指定距離を0～Spline全長の範囲へ循環させる。
        distance = Mathf.Repeat(distance, _totalLength);

        /*
         * _cumLenは小さい順に並んでいるため、
         * 二分探索を使ってdistanceを含む線分を探す。
         */
        int lo = 0;
        int hi = _cumLen.Count - 1;

        while (lo < hi)
        {
            // 右シフト1回は2で割ることと同じ。
            int mid = (lo + hi) >> 1;

            if (_cumLen[mid] < distance)
            {
                // 中央点が指定距離より手前なら、
                // 後半を検索する。
                lo = mid + 1;
            }
            else
            {
                // 中央点が指定距離以上なら、
                // 前半を検索する。
                hi = mid;
            }
        }

        /*
         * loはdistance以上となる最初の累積距離のインデックス。
         *
         * 実際に使用する線分は、
         * _positions[i - 1]～_positions[i]。
         */
        int i =
            Mathf.Clamp(lo, 1, _cumLen.Count - 1);

        // 対象線分の始点までの累積距離。
        float l0 = _cumLen[i - 1];

        // 対象線分の終点までの累積距離。
        float l1 = _cumLen[i];

        /*
         * distanceが対象線分内のどの位置にあるかを
         * 0～1の補間率として求める。
         *
         * 線分長がほぼ0の場合は0として扱う。
         */
        float t =
            Mathf.Abs(l1 - l0) < 1e-6f
                ? 0f
                : Mathf.InverseLerp(l0, l1, distance);

        // 線分上を補間してローカル座標を求める。
        Vector3 local =
            Vector3.LerpUnclamped(
                _positions[i - 1],
                _positions[i],
                t
            );

        // ローカル座標をワールド座標へ変換して返す。
        return transform.TransformPoint(local);
    }

    /// <summary>
    /// Splineの始点から指定距離だけ進んだ地点の
    /// ローカル座標を取得する。
    ///
    /// EvaluateByDistanceとほぼ同じ処理だが、
    /// TransformPointを行わずローカル座標のまま返す。
    ///
    /// 主に接線・法線計算で使用する。
    /// </summary>
    /// <param name="distance">
    /// Spline始点からの距離。
    /// </param>
    /// <returns>
    /// 指定距離に対応するローカル座標。
    /// </returns>
    private Vector3 EvaluateLocalByDistance(float distance)
    {
        if (_positions.Count < 2)
            return Vector3.zero;

        distance = Mathf.Repeat(distance, _totalLength);

        // 累積距離配列を二分探索する。
        int lo = 0;
        int hi = _cumLen.Count - 1;

        while (lo < hi)
        {
            int mid = (lo + hi) >> 1;

            if (_cumLen[mid] < distance)
            {
                lo = mid + 1;
            }
            else
            {
                hi = mid;
            }
        }

        int i =
            Mathf.Clamp(lo, 1, _cumLen.Count - 1);

        float l0 = _cumLen[i - 1];
        float l1 = _cumLen[i];

        float t =
            Mathf.Abs(l1 - l0) < 1e-6f
                ? 0f
                : Mathf.InverseLerp(l0, l1, distance);

        return Vector3.LerpUnclamped(
            _positions[i - 1],
            _positions[i],
            t
        );
    }

    /// <summary>
    /// 指定したSpline上の距離における
    /// 外向き法線ベクトルを取得する。
    ///
    /// 主にPlayerのジャンプ方向や、
    /// PointObjectの向きを決めるために使用する。
    /// </summary>
    /// <param name="distance">
    /// Spline始点からの距離。
    /// </param>
    /// <returns>
    /// Splineの外側を向く正規化済みワールド法線。
    /// </returns>
    public Vector3 EvaluateNormalByDistance(float distance)
    {
        // Splineデータが不足している場合は、
        // GameObjectの上方向を返す。
        if (_positions.Count < 2 ||
            _totalLength <= 1e-6f)
        {
            return transform.up;
        }

        /*
         * 指定地点の少し手前と少し先を取得し、
         * その差から接線を近似する。
         *
         * Spline全長に応じてepsを変えるが、
         * 最低値は0.001とする。
         */
        float eps =
            Mathf.Max(0.001f, _totalLength * 0.001f);

        // 指定位置より少し手前のローカル座標。
        Vector3 lp0 =
            EvaluateLocalByDistance(distance - eps);

        // 指定位置より少し先のローカル座標。
        Vector3 lp1 =
            EvaluateLocalByDistance(distance + eps);

        // 手前から先へ向かうベクトルを接線とする。
        Vector3 tangent =
            (lp1 - lp0).normalized;

        /*
         * 2D平面上で接線を90度回転し、
         * 法線候補を作る。
         *
         * tangent = (x, y) に対し、
         * normal = (-y, x)。
         */
        Vector3 localNormal =
            new Vector3(
                -tangent.y,
                tangent.x,
                0f
            ).normalized;

        // ローカル方向をワールド方向へ変換する。
        Vector3 worldNormal =
            transform
                .TransformDirection(localNormal)
                .normalized;

        // 指定距離に対応するワールド座標。
        Vector3 worldPos =
            transform.TransformPoint(
                EvaluateLocalByDistance(distance)
            );

        // Spline中心のワールド座標。
        Vector3 worldCenter =
            transform.TransformPoint(_localCenter);

        // 中心から指定位置へ向かう方向。
        //
        // これをSplineの外側方向として扱う。
        Vector3 toOutside =
            (worldPos - worldCenter).normalized;

        /*
         * 法線候補と外側方向の内積が負の場合、
         * 法線が内側を向いている。
         *
         * その場合は符号を反転して外向きにする。
         */
        if (Vector3.Dot(worldNormal, toOutside) < 0f)
        {
            worldNormal = -worldNormal;
        }

        return worldNormal;
    }

    /// <summary>
    /// 指定されたワールド座標に最も近い
    /// Spline上の距離を取得する。
    /// </summary>
    /// <param name="worldPos">
    /// 最近接位置を探す対象のワールド座標。
    /// </param>
    /// <returns>
    /// Splineの始点から最近接地点までの距離。
    /// </returns>
    public float FindNearestDistance(Vector3 worldPos)
    {
        if (_positions.Count < 2)
            return 0f;

        // 現時点で見つかった最小距離の二乗。
        float minSqrDist = float.MaxValue;

        // 最近接地点に対応するSpline上の距離。
        float nearestDistance = 0f;

        // Splineを構成する各線分を確認する。
        for (int i = 0; i < _positions.Count - 1; i++)
        {
            // 線分始点をワールド座標へ変換する。
            Vector3 a =
                transform.TransformPoint(_positions[i]);

            // 線分終点をワールド座標へ変換する。
            Vector3 b =
                transform.TransformPoint(_positions[i + 1]);

            // 線分の方向ベクトル。
            Vector3 ab = b - a;

            // 線分長の二乗。
            float abSqr = ab.sqrMagnitude;

            // 極端に短い線分は計算から除外する。
            if (abSqr < 1e-6f)
                continue;

            /*
             * worldPosを線分abへ射影し、
             * 線分内の位置を0～1で求める。
             */
            float t =
                Vector3.Dot(worldPos - a, ab) / abSqr;

            // 線分外へ出ないよう0～1へ制限する。
            t = Mathf.Clamp01(t);

            // 線分上の最近接点。
            Vector3 nearestPoint =
                a + ab * t;

            // worldPosと最近接点の距離の二乗。
            float sqrDist =
                (worldPos - nearestPoint).sqrMagnitude;

            // これまでより近い場合は結果を更新する。
            if (sqrDist < minSqrDist)
            {
                minSqrDist = sqrDist;

                // 現在の線分の長さ。
                float segLen = Mathf.Sqrt(abSqr);

                // 現在の線分の始点までの累積距離。
                float baseLen = _cumLen[i];

                /*
                 * 線分始点までの累積距離に、
                 * 線分内の距離を足す。
                 */
                nearestDistance =
                    baseLen + segLen * t;
            }
        }

        return nearestDistance;
    }


    /* =========================================================
     * Catmull-Rom補間
     * ========================================================= */

    /// <summary>
    /// 4つの制御点からCatmull-Rom曲線上の座標を計算する。
    ///
    /// 実際に補間される区間はp1からp2。
    /// p0とp3は曲線の傾きを決めるために使用する。
    /// </summary>
    /// <param name="p0">
    /// p1の1つ前の制御点。
    /// </param>
    /// <param name="p1">
    /// 補間区間の始点。
    /// </param>
    /// <param name="p2">
    /// 補間区間の終点。
    /// </param>
    /// <param name="p3">
    /// p2の1つ後の制御点。
    /// </param>
    /// <param name="t">
    /// p1からp2までの補間率。通常は0～1。
    /// </param>
    /// <returns>
    /// Catmull-Rom曲線上のローカル2D座標。
    /// </returns>
    private Vector2 CatmullRom(
        Vector2 p0,
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        float t)
    {
        // tの2乗と3乗を先に計算し、
        // 式内での重複計算を避ける。
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0
                - 5f * p1
                + 4f * p2
                - p3) * t2 +
            (-p0
                + 3f * p1
                - 3f * p2
                + p3) * t3
        );
    }


    /* =========================================================
     * 着地マテリアル
     * ========================================================= */

    /// <summary>
    /// PlayerがこのSplineへ着地した際に、
    /// 着地用マテリアルへ変更する。
    ///
    /// 開始地点のSplineでは実行しない。
    /// </summary>
    public void FlashLandingMaterial()
    {
        // 開始地点のSplineは着地演出の対象外。
        if (IsStartSpline)
            return;

        // Editモードではマテリアルを変更しない。
        if (!Application.isPlaying)
            return;

        EnsureRenderer();

        // 着地用マテリアルが未設定なら何もしない。
        if (landingMaterial == null)
            return;

        // 通常マテリアルが未保存なら、
        // 現在のマテリアルを通常状態として保存する。
        if (normalMaterial == null)
        {
            normalMaterial = _lineRenderer.material;
        }

        // 着地用マテリアルへ変更する。
        _lineRenderer.material = landingMaterial;
    }


    /* =========================================================
     * Unityエディター用ContextMenu
     * ========================================================= */

#if UNITY_EDITOR

    /// <summary>
    /// InspectorのContextMenuからSplineを再構築する。
    /// </summary>
    [ContextMenu("Rebuild Spline")]
    public void EditorRebuild()
    {
        Rebuild();
    }

    /// <summary>
    /// InspectorのContextMenuから
    /// 円形の制御点を生成してSplineを再構築する。
    /// </summary>
    [ContextMenu("Generate Circle Control Points")]
    private void EditorGenerateCircle()
    {
        autoShapeType = AutoShapeType.Circle;

        GenerateAutoControlPoints();
        Rebuild();
    }

    /// <summary>
    /// InspectorのContextMenuから
    /// 楕円形の制御点を生成してSplineを再構築する。
    /// </summary>
    [ContextMenu("Generate Ellipse Control Points")]
    private void EditorGenerateEllipse()
    {
        autoShapeType = AutoShapeType.Ellipse;

        GenerateAutoControlPoints();
        Rebuild();
    }

#endif


    /* =========================================================
     * 公開プロパティ
     * ========================================================= */

    /// <summary>
    /// このSplineの識別ID。
    /// </summary>
    public int SplineID => splineID;


    /* =========================================================
     * ジャンプ移動とSplineの接触判定
     * ========================================================= */

    /// <summary>
    /// Playerが1フレームで移動した線分と、
    /// このSplineとの接触を判定する。
    ///
    /// Playerの移動を単一の点ではなく
    /// fromWorldからtoWorldまでの線分として調べるため、
    /// 高速移動時のすり抜けを防ぎやすい。
    /// </summary>
    /// <param name="fromWorld">
    /// Playerの移動前ワールド座標。
    /// </param>
    /// <param name="toWorld">
    /// Playerの移動後ワールド座標。
    /// </param>
    /// <param name="radius">
    /// PlayerとSplineが接触したとみなす半径。
    /// </param>
    /// <param name="hitDistanceOnSpline">
    /// 接触した場合に返される、
    /// Spline始点から接触地点までの距離。
    /// </param>
    /// <param name="hitPointOnSpline">
    /// 接触した場合に返される、
    /// Spline上の接触地点のワールド座標。
    /// </param>
    /// <returns>
    /// 接触した場合はtrue、接触しなかった場合はfalse。
    /// </returns>
    public bool TrySweepHit(
        Vector3 fromWorld,
        Vector3 toWorld,
        float radius,
        out float hitDistanceOnSpline,
        out Vector3 hitPointOnSpline)
    {
        // 接触しなかった場合に備え、
        // out引数へ初期値を設定する。
        hitDistanceOnSpline = 0f;
        hitPointOnSpline = Vector3.zero;

        // Splineの線分を作れない場合は接触なし。
        if (_positions.Count < 2)
            return false;

        // Vector3.Distanceではなく二乗距離で比較するため、
        // 判定半径も二乗しておく。
        float radiusSqr = radius * radius;

        // 1つ以上の接触候補が見つかったかどうか。
        bool found = false;

        /*
         * Playerの移動線分上で、
         * 最も移動開始地点に近い接触位置。
         *
         * moveTは0～1で、
         * 0がfromWorld、1がtoWorld。
         */
        float bestMoveT = float.MaxValue;

        /*
         * 接触したSpline線分上の位置。
         *
         * 0が線分始点、1が線分終点。
         */
        float bestSplineT = 0f;

        // 接触したSpline線分のインデックス。
        int bestSegIndex = -1;

        // 接触したSpline上のワールド座標。
        Vector3 bestPointOnSpline = Vector3.zero;

        // Splineを構成するすべての線分と比較する。
        for (int i = 0; i < _positions.Count - 1; i++)
        {
            // Spline線分の始点をワールド座標へ変換する。
            Vector3 a =
                transform.TransformPoint(_positions[i]);

            // Spline線分の終点をワールド座標へ変換する。
            Vector3 b =
                transform.TransformPoint(_positions[i + 1]);

            /*
             * 次の2本の線分間で最近接点を計算する。
             *
             * 線分1：Playerの移動線分
             * fromWorld ～ toWorld
             *
             * 線分2：Splineの現在の線分
             * a ～ b
             *
             * moveT：
             * Player移動線分上の最近接位置
             *
             * splineT：
             * Spline線分上の最近接位置
             *
             * c1：
             * Player移動線分上の最近接点
             *
             * c2：
             * Spline線分上の最近接点
             */
            ClosestPtSegmentSegment(
                fromWorld,
                toWorld,
                a,
                b,
                out float moveT,
                out float splineT,
                out Vector3 c1,
                out Vector3 c2
            );

            // 2本の線分間の最短距離の二乗。
            float sqrDist =
                (c1 - c2).sqrMagnitude;

            // 最短距離が判定半径を超えていれば接触していない。
            if (sqrDist > radiusSqr)
                continue;

            /*
             * 最初の接触候補、または
             * Playerの移動開始地点からより近い接触候補なら更新する。
             *
             * これにより、1フレーム中に複数地点へ接触した場合でも、
             * Playerが最初に触れた地点を採用する。
             */
            if (!found || moveT < bestMoveT)
            {
                found = true;

                bestMoveT = moveT;
                bestSplineT = splineT;
                bestSegIndex = i;
                bestPointOnSpline = c2;
            }
        }

        // どのSpline線分とも接触しなかった。
        if (!found)
            return false;

        /*
         * 接触したSpline線分の長さをローカル座標から計算する。
         *
         * _cumLenもローカル座標基準で作られているため、
         * ここでもローカル長を使用している。
         */
        Vector3 segLocalA =
            _positions[bestSegIndex];

        Vector3 segLocalB =
            _positions[bestSegIndex + 1];

        float segLen =
            Vector3.Distance(segLocalA, segLocalB);

        /*
         * 接触した線分の始点までの累積距離に、
         * 線分内の接触位置を加算する。
         *
         * bestSplineTが
         * 0なら線分始点、
         * 1なら線分終点。
         */
        hitDistanceOnSpline =
            _cumLen[bestSegIndex]
            + segLen * bestSplineT;

        // Spline上の接触点をワールド座標で返す。
        hitPointOnSpline = bestPointOnSpline;

        return true;
    }


    /* =========================================================
     * 2本の線分間の最近接点計算
     * ========================================================= */

    /// <summary>
    /// 2本の線分上で互いに最も近い点を計算する。
    ///
    /// 線分1：p1～q1
    /// 線分2：p2～q2
    ///
    /// 各線分上の最近接位置を0～1のパラメータと
    /// ワールド座標の両方で返す。
    /// </summary>
    /// <param name="p1">
    /// 1本目の線分の始点。
    /// </param>
    /// <param name="q1">
    /// 1本目の線分の終点。
    /// </param>
    /// <param name="p2">
    /// 2本目の線分の始点。
    /// </param>
    /// <param name="q2">
    /// 2本目の線分の終点。
    /// </param>
    /// <param name="s">
    /// 1本目の線分上の最近接位置。
    ///
    /// 0ならp1、1ならq1。
    /// </param>
    /// <param name="t">
    /// 2本目の線分上の最近接位置。
    ///
    /// 0ならp2、1ならq2。
    /// </param>
    /// <param name="c1">
    /// 1本目の線分上の最近接点。
    /// </param>
    /// <param name="c2">
    /// 2本目の線分上の最近接点。
    /// </param>
    private static void ClosestPtSegmentSegment(
        Vector3 p1,
        Vector3 q1,
        Vector3 p2,
        Vector3 q2,
        out float s,
        out float t,
        out Vector3 c1,
        out Vector3 c2)
    {
        // 浮動小数点誤差を考慮して、
        // 線分長を0とみなす基準値。
        const float EPS = 1e-6f;

        // 1本目の線分の方向ベクトル。
        Vector3 d1 = q1 - p1;

        // 2本目の線分の方向ベクトル。
        Vector3 d2 = q2 - p2;

        // 2本の線分の始点間ベクトル。
        Vector3 r = p1 - p2;

        // 1本目の線分長の二乗。
        float a = Vector3.Dot(d1, d1);

        // 2本目の線分長の二乗。
        float e = Vector3.Dot(d2, d2);

        // d2と始点間ベクトルの内積。
        float f = Vector3.Dot(d2, r);

        /*
         * 両方の線分がほぼ点の場合。
         *
         * それぞれの始点を最近接点として返す。
         */
        if (a <= EPS && e <= EPS)
        {
            s = 0f;
            t = 0f;

            c1 = p1;
            c2 = p2;

            return;
        }

        /*
         * 1本目だけがほぼ点の場合。
         *
         * p1に最も近い2本目の線分上の位置を求める。
         */
        if (a <= EPS)
        {
            s = 0f;

            // p1を2本目の線分へ射影し、
            // 線分内へ収める。
            t = Mathf.Clamp01(f / e);
        }
        else
        {
            // d1と始点間ベクトルの内積。
            float c = Vector3.Dot(d1, r);

            /*
             * 2本目だけがほぼ点の場合。
             *
             * p2に最も近い1本目の線分上の位置を求める。
             */
            if (e <= EPS)
            {
                t = 0f;

                s = Mathf.Clamp01(-c / a);
            }
            else
            {
                // 2本の線分方向ベクトル同士の内積。
                float b = Vector3.Dot(d1, d2);

                /*
                 * 連立方程式の分母。
                 *
                 * 0に近い場合は2本の線分が
                 * 平行に近いことを表す。
                 */
                float denom =
                    a * e - b * b;

                // 平行でなければ、
                // 無限直線同士の最近接位置sを求める。
                if (denom != 0f)
                {
                    s = Mathf.Clamp01(
                        (b * f - c * e) / denom
                    );
                }
                else
                {
                    // 平行の場合は1本目の始点側を仮採用する。
                    s = 0f;
                }

                /*
                 * 求めたsに対応する地点から、
                 * 2本目の直線上の位置を計算するための分子。
                 */
                float tNom = b * s + f;

                if (tNom < 0f)
                {
                    /*
                     * 2本目の線分始点より外側に最近接点がある場合。
                     *
                     * tを始点の0に固定し、
                     * その位置に対するsを再計算する。
                     */
                    t = 0f;
                    s = Mathf.Clamp01(-c / a);
                }
                else if (tNom > e)
                {
                    /*
                     * 2本目の線分終点より外側に最近接点がある場合。
                     *
                     * tを終点の1に固定し、
                     * その位置に対するsを再計算する。
                     */
                    t = 1f;
                    s = Mathf.Clamp01((b - c) / a);
                }
                else
                {
                    // 最近接点が2本目の線分内にある場合。
                    t = tNom / e;
                }
            }
        }

        // sから1本目の線分上の最近接点を求める。
        c1 = p1 + d1 * s;

        // tから2本目の線分上の最近接点を求める。
        c2 = p2 + d2 * t;
    }
}