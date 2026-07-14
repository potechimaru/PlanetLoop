using UnityEngine;

/// <summary>
/// GameModeを意味するDummyPlayerにアタッチするクラス。順番管理とSplineの参照を保持する。
/// スロットを動かすために必要。
/// </summary>
public class GameModeSlotView : MonoBehaviour
{
    [SerializeField] private ClosedSplineLine _spline;
    [SerializeField] private int _order;

    public ClosedSplineLine Spline => _spline;
    public int Order => _order;

    private void Reset()
    {
        if (_spline == null)
        {
            _spline = GetComponent<ClosedSplineLine>();
        }
    }
}