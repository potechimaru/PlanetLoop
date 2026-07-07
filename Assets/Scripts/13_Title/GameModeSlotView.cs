using UnityEngine;

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