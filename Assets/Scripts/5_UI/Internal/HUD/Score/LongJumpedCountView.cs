using UnityEngine;
using TMPro;

public interface ILongJumpedCountView : IHUDCountView { }

public class LongJumpedCountView : MonoBehaviour, ILongJumpedCountView
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetCount(int current, int max)
    {
        if (_text == null) return;
        _text.text = $"{current}/{max}";
    }
}