using UnityEngine;

/// <summary>
/// SplineがPointの位置を把握するためのデータを保持するクラス。PointIbjectにアタッチされる。
/// Spline上でPointを回転させる際に、Splineの始点からの距離を保持することで、Spline上での位置を把握する。
/// </summary>
public class SplinePointData : MonoBehaviour
{
    [SerializeField]
    private float baseDistance;

    public float BaseDistance
    {
        get => baseDistance;
        set => baseDistance = value;
    }
}
