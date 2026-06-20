using TMPro;
using UnityEngine;

public interface IVisitedSplineCountView : IHUDCountView { }

public class VisitedSplineCountView : MonoBehaviour, IVisitedSplineCountView
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetCount(int current, int max)
    {
        if (_text == null) return;
        _text.text = $"{current}/{max}";
    }
}