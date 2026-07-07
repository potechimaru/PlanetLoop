using TMPro;
using UnityEngine;

public interface IVisitedSplineCountView : IHUDCountView { }

/// <summary>
/// 未訪問のSplineと訪問済みのSplineの数を表示するViewクラス。
/// </summary>
public class VisitedSplineCountView : MonoBehaviour, IVisitedSplineCountView
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetCount(int current, int max)
    {
        if (_text == null) return;
        _text.text = $"{current}/{max}";
    }
}