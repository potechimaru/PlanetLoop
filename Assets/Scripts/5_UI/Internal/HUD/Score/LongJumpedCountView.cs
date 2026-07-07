using UnityEngine;
using TMPro;

public interface ILongJumpedCountView : IHUDCountView { }

/// <summary>
/// 長距離ジャンプの成功回数を表示するViewクラス。回数制限あり。
/// </summary>
public class LongJumpedCountView : MonoBehaviour, ILongJumpedCountView
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetCount(int current, int max)
    {
        if (_text == null) return;
        _text.text = $"{current}/{max}";
    }
}